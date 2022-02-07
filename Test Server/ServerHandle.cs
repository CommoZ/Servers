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
			//string _playFabId = _packet.ReadString();

			Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now a player {_fromClient}.");
			//Console.WriteLine($"\n Clients PlayFabID {_playFabId}");
			if (_fromClient != _clientIdCheck)
			{
				Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
			}
			//TODO sent player into game
		}

		//Old stuff that didnt work
		//public static void DisconnectPlayer(int _fromClient, Packet _packet)
		//{
		//	int _clientId = _packet.ReadInt();
		//	string _playFabId = _packet.ReadString();
		//	string _username = _packet.ReadString();

		//	//Wait this will jsut work on my quit wtf
		//	Console.WriteLine($"Disconnecting player of clientID: {_fromClient} from active players");
		//	Server.clients.Remove(_fromClient);

		//	for (int i = 1; i < Server.MaxPlayers; i++)
		//	{
		//		if (i > _fromClient)
		//			Server.clients[i].id--;
		//		//Console.WriteLine($"After disconnecting player of id: {_fromClient} from active players");
		//		//Console.Write($"this is the servers ids left{Server.clients[i].id}");
		//	}
		//}

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
			string _playFabID = _packet.ReadString();
			string _username = _packet.ReadString();
			string _msg = _packet.ReadString();


			Console.WriteLine($"Message from client username: {_username} Reads: {_msg}");
			
			
			//Sending bac to all players
			ServerSend.SendMessagePublic(_clientId, _playFabID, _username, _msg);
	
		}

		public static void ClientPlayFabIDRecieved(int _fromClient, Packet _packet)
		{
			int _clientId = _packet.ReadInt();
			string _playFabID = _packet.ReadString();
			string _playFabUsername = _packet.ReadString();

			//This tells me which player is online and in which spot they are online in
			//Right after that they will set the array and use it 
			//Remember Limit is at 50 players
			Server.playFabIDArray[_clientId] = _playFabID;
			Server.playFabUsernameArray[_clientId] = _playFabUsername;

			Console.WriteLine($"Players ClientID { _clientId }" +
				$"\nPlayers Current Server ID { _fromClient }" +
				$"\nPlayFab ID: { _playFabID } " +
				$"\nPlayFab Username: { _playFabUsername }");
		}

		public static void ClientAddFriendRequest(int _fromClient, Packet _packet)
		{
			int _clientId = _packet.ReadInt();
			string _playFabIDToAdd = _packet.ReadString();

			Console.WriteLine($"Recieved Friend Request from {Server.playFabUsernameArray[_clientId]}" +
				$"\nPlayerID to add {_playFabIDToAdd} ");

			//wow i cant believe i wrote wrong code for 2 hours
			for (int i = 0; i <= Server.MaxPlayers; i++)
			{
				if (Server.playFabIDArray[i] == null)
					continue;
				else
					Console.WriteLine($"This slot has an ID and it is {Server.playFabIDArray[i]}");
				if (_playFabIDToAdd == Server.playFabIDArray[i])
				{
					//TODO: send a friend request
					ServerSend.SendFriendRequest(i, _clientId);


					Console.WriteLine($"Player of ID {_playFabIDToAdd} has been sent request");
					break;
				}
				//Console.WriteLine($"Current ID in for loop: {Server.playFabIDArray[i]}");
			}
			//Console.WriteLine($"Player of ID {_playFabIDToAdd} is not online");


		}

		public static void ClientAddFriendAccept(int _fromClient, Packet _packet)
		{
			int _clientId = _packet.ReadInt();
			string _playFabIDToAdd = _packet.ReadString();

			Console.WriteLine($"Friend request accepted from {Server.playFabUsernameArray[_clientId]}" +
				$"\nPlayerID to add {_playFabIDToAdd} ");

			for (int i = 0; i <= Server.MaxPlayers; i++)
			{
				if (Server.playFabIDArray[i] == null)
					continue;
				else
					Console.WriteLine($"This slot has an ID and it is {Server.playFabIDArray[i]}");
				if (_fromClient == i)
				{
					ServerSend.SendFriendAccept(_fromClient, i);

				}
				if (_playFabIDToAdd == Server.playFabIDArray[i])
				{
					//TODO: send a friend request
					ServerSend.SendFriendAccept(i, _clientId);
					
					Console.WriteLine($"Player of ID {_playFabIDToAdd} has been sent Friend Acceptance message");
					break;
				}
				
			}

		}

		public static void ClientAddFriendDeclined (int _fromClient, Packet _packet)
		{
			int _clientId = _packet.ReadInt();
			string _playFabIDToAdd = _packet.ReadString();

			Console.WriteLine($"Friend request declined from {Server.playFabUsernameArray[_clientId]}" +
				$"\nPlayerID to add {_playFabIDToAdd} ");

			for (int i = 0; i <= Server.MaxPlayers; i++)
			{
				if (Server.playFabIDArray[i] == null)
					continue;
				else
					Console.WriteLine($"This slot has an ID and it is {Server.playFabIDArray[i]}");
				if (_playFabIDToAdd == Server.playFabIDArray[i])
				{
					//TODO: send a friend request
					ServerSend.SendFriendDeclined(i, _clientId);

					Console.WriteLine($"Player of ID {_playFabIDToAdd} has been sent Friend Declined message");
					break;
				}
			}


		}


	}
}
