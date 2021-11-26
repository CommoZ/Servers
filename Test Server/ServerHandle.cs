using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Server
{
	class ServerHandle
	{
		public static void WelcomeRecieved(int _fromClient, Packet _packet)
		{
			int _clientIdCheck = _packet.ReadInt();
			string _username = _packet.ReadString();

			Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now a player {_fromClient}.");
			if(_fromClient != _clientIdCheck)
			{
				Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
			}
			//TODO sent player into game
		}


		public static void UDPTestReceived(int _fromClient, Packet _packet)
		{
			string _msg = _packet.ReadString();

			Console.WriteLine($"Received packet via UDP. Contains Message: { _msg }");
		}

		//To Receive and send back message to the server
		public static void MessageReceived(int _fromClient, Packet _packet)
		{
			//int _clientIdCheck = _packet.ReadInt();
			//string _username = _packet.ReadString();
			int _clientId = _packet.ReadInt();
			string _username = _packet.ReadString();
			string _msg = _packet.ReadString();


			Console.WriteLine($"Message from client username: {_username} Reads: {_msg}");
			
			
			//Sending bac to all players
			ServerSend.SendMessagePublic(_clientId, _username, _msg);
	
		}

	}
}
