using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Server
{
	class ServerSend
	{
		//This dumb shit try not to touch, cuz no need to change anything tbh
		//Actually all of these are overloaders lmao


		private static void SendTCPData(int _toClient, Packet _packet)
		{
			_packet.WriteLength();
			Server.clients[_toClient].tcp.SendData(_packet);
		}



		private static void SendTCPDataToAll(Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i < Server.MaxPlayers; i++)
			{
				Server.clients[i].tcp.SendData(_packet);
			}


		}

		private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i < Server.MaxPlayers; i++)
			{
				if (i != _exceptClient)
					Server.clients[i].tcp.SendData(_packet);
			}
		}

		private static void SendUDPData(int _toClient, Packet _packet)
		{
			_packet.WriteLength();
			Server.clients[_toClient].udp.SendData(_packet);
		}

		private static void SendUDPDataToAll(Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i < Server.MaxPlayers; i++)
			{
				Server.clients[i].udp.SendData(_packet);
			}


		}
		private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i < Server.MaxPlayers; i++)
			{
				if (i != _exceptClient)
					Server.clients[i].udp.SendData(_packet);
			}
		}

	

		#region Packets
		public static void Welcome(int _toClient, string _msg)
		{
			using (Packet _packet = new Packet((int)ServerPackets.welcome))
			{
				_packet.Write(_msg);
				_packet.Write(_toClient);

				SendTCPData(_toClient, _packet);
			}

		}

		public static void UDPTest(int _toClient)
		{
			using (Packet _packet = new Packet((int)ServerPackets.udpTest))
			{
				_packet.Write("Your Mom Gei");

				SendUDPData(_toClient, _packet);
			}
		}

		public static void SendMessagePublic(int _toClient, string _msg)
		{
			using (Packet _packet = new Packet((int)ServerPackets.publicMessageSent))
			{
				_packet.Write(_msg);
				_packet.Write(_toClient);
				
				//Actually for this purpose i dont need send back the _toClient id
				
				Console.WriteLine($"Sending back this msg to everyone: {_msg} Except for :{_toClient}");
				SendTCPDataToAll(_toClient ,_packet);
			}
		}

		#endregion
	}
}
