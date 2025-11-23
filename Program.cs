using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordMusicBot
{
    internal class Program
    {
		static async Task Main(string[] args)
		{
			var bot = new DiscordBot();
			await bot.RunBotAsync();
		}
	}
	public class DiscordBot
	{
		private readonly DiscordSocketClient _client;
		private AppConfig _config;

		public DiscordBot()
		{
			_config = ReadConfig();
			_client = new DiscordSocketClient();
			_client.Log += Log;
		}

		public async Task RunBotAsync()
		{
			// Load the bot token from the appconfig.json file
			if (string.IsNullOrEmpty(_config.Discord.Token))
			{
				Console.WriteLine("Error: Bot token is missing or invalid.");
				return;
			}
			// Log in and start the bot
			await _client.LoginAsync(TokenType.Bot, _config.Discord.Token);
			await _client.StartAsync();
			// Block the program until it is closed
			await Task.Delay(-1);
		}

		private AppConfig ReadConfig()
		{
			try
			{
				string jsonContent = File.ReadAllText("Config.json");
				var config = JsonSerializer.Deserialize<AppConfig>(jsonContent);
				return config;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading config: {ex.Message}");
				return null;
			}
		}

		private Task Log(LogMessage log)
		{
			Console.WriteLine(log);
			return Task.CompletedTask;
		}
	}
}
