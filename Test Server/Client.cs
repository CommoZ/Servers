using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Test_Server
{
	class Client
	{
		public static int dataBufferSize = 4096;
		public int id;
		public TCP tcp;
		public UDP udp;


		public Client(int _clientId)
		{
			id = _clientId;

			tcp = new TCP(id);
			udp = new UDP(id);
		}


		public class TCP
		{
			public TcpClient socket;

			private readonly int id;
			private NetworkStream stream;
			private Packet recieveData;
			private byte[] recieveBuffer;

			public TCP(int _id)
			{
				id = _id;
			}

			public void Connect(TcpClient _socket)
			{
				socket = _socket;
				socket.ReceiveBufferSize = dataBufferSize;
				socket.SendBufferSize = dataBufferSize;

				stream = socket.GetStream();

				recieveData = new Packet();
				recieveBuffer = new byte[dataBufferSize];
				stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);

				//TODO: Send welcome packet
				ServerSend.Welcome(id, "Welcome to the Server ");
			}

			public void SendData(Packet _packet)
			{
				try
				{
					if (socket != null)
					{
						stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
					}
				}
				catch (Exception _ex)
				{
					Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
				}

			}

		


			private void RecieveCallback(IAsyncResult _result)
			{
				try
				{
					int _byteLength = stream.EndRead(_result);
					if (_byteLength <= 0)
					{
						//TODO: disconnect
						Server.clients[id].Disconnect();

						return; 
					}

					byte[] _data = new byte[_byteLength];
					Array.Copy(recieveBuffer, _data, _byteLength);

					//TODO: handle data
					recieveData.Reset(HandleData(_data));
					
					stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);

				}
				catch(Exception _ex)
				{
					Console.WriteLine($"Error receiving TCP data: {_ex}");
					Server.clients[id].Disconnect();
				}



			}
			private bool HandleData(byte[] _data)
			{
				int _packetLength = 0;

				recieveData.SetBytes(_data);
				if (recieveData.UnreadLength() >= 4)
				{
					_packetLength = recieveData.ReadInt();
					if (_packetLength <= 0)
						return true;
				}

				while (_packetLength > 0 && _packetLength <= recieveData.UnreadLength())
				{
					byte[] _packetBytes = recieveData.ReadBytes(_packetLength);
					ThreadManager.ExecuteOnMainThread(() =>
					{
						using (Packet _packet = new Packet(_packetBytes))
						{
							int _packetId = _packet.ReadInt();
							Server.packetHandlers[_packetId](id, _packet);
						}
					});

					_packetLength = 0;
					if (recieveData.UnreadLength() >= 4)
					{
						_packetLength = recieveData.ReadInt();
						if (_packetLength <= 0)
							return true;
					}


				}
				if (_packetLength <= 1)
				{
					return true;
				}

				return false;
			}

			public void Disconnect()
			{
				socket.Close();
				stream = null;
				recieveData = null;
				recieveBuffer = null;
				socket = null;

				Console.WriteLine($"{ Server.playFabIDArray[id] }" +
				$" With the Username :{ Server.playFabUsernameArray[id] } Has disconnected" +
				$"\nCurrent Players in Server is at { Server.CurrentPlayers }");

				Server.playFabIDArray[id] = null;
				Server.playFabUsernameArray[id] = null;

			

			}

		}
	
		public class UDP
		{
			public IPEndPoint endPoint;

			private int id;

			public UDP(int _id)
			{
				id = _id;
			}

			public void Connect(IPEndPoint _endPoint)
			{
				endPoint = _endPoint;
				ServerSend.UDPTest(id);
			}

			public void SendData(Packet _packet)
			{
				Server.SendUDPData(endPoint, _packet);
			}

			public void HandleData(Packet _packetData)
			{
				int _packetLength = _packetData.ReadInt();
				byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

				ThreadManager.ExecuteOnMainThread(() =>
				{
					using (Packet _packet = new Packet(_packetBytes))
					{
						int _packetId = _packet.ReadInt();
						Server.packetHandlers[_packetId](id, _packet);
					}
				});
			}

			public void Disconnect()
			{
				endPoint = null;
			}
		}


		private void Disconnect()
		{
			Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
			Server.CurrentPlayers--;
			tcp.Disconnect();
			udp.Disconnect();
		}
	}
}
