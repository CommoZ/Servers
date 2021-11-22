using System;

namespace Test_Server
{
	class Program
	{
		//This random commit is not the only thing i want to commit

		static void Main(string[] args)
		{
			Console.Title = "Game Server";

			Server.Start(50, 7777);

			Console.ReadKey();
		}
	}
}
