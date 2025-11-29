using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordMusicBot.Services;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordMusicBot
{
    internal class Program
    {
		public static async Task Main(string[] args)
		{
			var bot = new DiscordBot();
			await bot.RunBotAsync();
		}
	}
	internal class DiscordBot
	{
		private DiscordSocketClient _client;
		private CommandHandler _commandHandler;
		private InteractionHandler _interactionHandler;
		private readonly AppConfig _config;

		public DiscordBot()
		{
			_config = ReadConfig();
		}

		public async Task RunBotAsync()
		{
			//Bot cannot run if config failed to load
			if (_config == null)
				return;

			using (var services = ConfigureServices())
			{
				_client = services.GetRequiredService<DiscordSocketClient>();
				_client.Log += Log;
				_commandHandler = services.GetRequiredService<CommandHandler>();
				_interactionHandler = services.GetRequiredService<InteractionHandler>();

				// Load the bot token from the appconfig.json file
				if (string.IsNullOrEmpty(_config.Discord.Token))
				{
					Console.WriteLine("Error: Bot token is missing or invalid.");
					return;
				}
				// Log in and start the bot
				await _client.LoginAsync(TokenType.Bot, _config.Discord.Token);
				await _client.StartAsync();
				await _commandHandler.InstallCommandsAsync();
				await _interactionHandler.InitializeInteractionsAsync();
				// Block the program until it is closed
				await Task.Delay(-1);
			}
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

		private ServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
				.AddSingleton(_config)
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandHandler>()
				.AddSingleton<CommandService>()
				.AddSingleton<InteractionHandler>()
				.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
				.AddLavalink()
				.ConfigureLavalink(config =>
				{
					config.BaseAddress = new Uri(_config.Lavalink.BaseAddress);
					config.Passphrase = _config.Lavalink.Passphrase;
				})
				.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace))
				.BuildServiceProvider();
		}
	}
}
