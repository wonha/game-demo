using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		static List<Timer> _timers = new List<Timer>();

		static void TickRoom(GameRoom room, int tick = 100)
		{
			var timer = new Timer();
			timer.Interval = tick;
			timer.Elapsed += ((s, e) => { room.Update(); });
			timer.AutoReset = true;
			timer.Enabled = true;

			_timers.Add(timer);
		}

		static void Main(string[] args)
		{
			ConfigManager.LoadConfig();
			DataManager.LoadData();

			GameRoom room = RoomManager.Instance.Add(1);
			TickRoom(room, 50);

			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 8080);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine($"Listening: ${endPoint}");


			while (true)
			{
				DbTransaction.Instance.Flush();
			}
		}
	}
}
