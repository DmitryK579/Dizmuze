using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Dizmuze.Services;
using Dizmuze.Settings;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dizmuze
{
	internal sealed class DiscordBot : IHostedService
	{
		private readonly DiscordSocketClient _client;
		private readonly InteractionHandler _interactionHandler;
		private readonly IServiceProvider _serviceProvider;
		private readonly IOptions<DiscordBotSettings> _settings;
		private readonly ILogger<DiscordBot> _logger;

		public DiscordBot(DiscordSocketClient client, 
						  InteractionHandler interactionHandler, IServiceProvider serviceProvider,
						  IOptions<DiscordBotSettings> settings, ILogger<DiscordBot> logger)
		{
			_client = client;
			_interactionHandler = interactionHandler;
			_serviceProvider = serviceProvider;
			_settings = settings;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			//Bot cannot login without a valid token
			if (string.IsNullOrEmpty(_settings.Value.Token))
			{
				_logger.LogCritical("Error: Discord Bot token is missing or invalid.");
				return;
			}

			_client.Log += Log;
			_client.Ready += Ready;

			await _client.LoginAsync(TokenType.Bot, _settings.Value.Token);
			await _client.StartAsync();

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
