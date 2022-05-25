using System;
using System.IO;
using Newtonsoft.Json;

namespace Server.Data
{
	[Serializable]
	public class ServerConfig
	{
		public string dataPath;
		public string connectionString;
	}

	public class ConfigManager
	{
		public static ServerConfig Config { get; private set; }

		public static void LoadConfig()
		{
			string text = File.ReadAllText("config.json");
			Config = JsonConvert.DeserializeObject<ServerConfig>(text);
		}
	}
}
