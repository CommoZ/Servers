using System;
using System.Threading;
namespace Test_Server
{
	class Program
	{
		//This random commit is not the only thing i want to commit
		private static bool isRunning = false;
		static void Main(string[] args)
		{
			Console.Title = "Game Server";

			isRunning = true;
			Server.Start(50, 7777);

			Thread mainThread = new Thread(new ThreadStart(MainThread));
			mainThread.Start();

			//Console.ReadKey();
		}

		private static void MainThread()
		{
			Console.WriteLine($"Main Thread started. Running at {Constants.TICKS_PER_SECOND} ticks per second.");
			DateTime _nextLoop = DateTime.Now;

			while (isRunning)
			{
				while (_nextLoop < DateTime.Now)
				{
					GameLogic.Update();

					_nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

					if (_nextLoop > DateTime.Now)
					{
						Thread.Sleep(_nextLoop - DateTime.Now);
					}
				}
			}

		}
	}
}
