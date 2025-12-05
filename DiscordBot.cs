using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordMusicBot.Services;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordMusicBot
{
	internal class DiscordBot : IHostedService
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandHandler _commandHandler;
		private readonly InteractionHandler _interactionHandler;
		private readonly IServiceProvider _serviceProvider;
		private readonly IConfiguration _config;
		private readonly ILogger<DiscordBot> _logger;

		public DiscordBot(DiscordSocketClient client, CommandHandler commandHandler, 
						  InteractionHandler interactionHandler, IServiceProvider serviceProvider, 
						  IConfiguration config, ILogger<DiscordBot> logger)
		{
			_client = client
			?? throw new ArgumentNullException(nameof(client));
			_commandHandler = commandHandler
			?? throw new ArgumentNullException(nameof(commandHandler));
			_interactionHandler = interactionHandler
			?? throw new ArgumentNullException(nameof(interactionHandler));
			_serviceProvider = serviceProvider
			?? throw new ArgumentNullException(nameof(serviceProvider));
			_config = config;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			string token = _config["Discord:Token"];
			if (string.IsNullOrEmpty(token))
			{
				_logger.LogCritical("Error: Discord Bot token is missing or invalid.");
				return;
			}

			_client.Log += Log;
			_client.Ready += Ready;

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			await _commandHandler.InstallCommandsAsync();
			await _interactionHandler.InitializeInteractionsAsync();
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_client.Log -= Log;
			_client.Ready -= Ready;
			await _client.LogoutAsync();
			await _client.StopAsync();
		}

		private Task Log (LogMessage log)
		{
			_logger.LogInformation(log.ToString());
			return Task.CompletedTask;
		}

		private Task Ready()
		{
			_logger.LogInformation($"Bot connected as {_client.CurrentUser}");
			return Task.CompletedTask;
		}
	}
}
