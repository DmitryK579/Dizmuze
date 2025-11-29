using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot.Services
{
	internal class InteractionHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly InteractionService _interactions;
		private readonly AppConfig _config;
		private readonly IServiceProvider _serviceProvider;

		public InteractionHandler(DiscordSocketClient client, InteractionService interactions, AppConfig config,
							  IServiceProvider serviceProvider)
		{
			_interactions = interactions;
			_client = client;
			_config = config;
			_serviceProvider = serviceProvider;
		}

		public async Task InitializeInteractionsAsync()
		{
			_client.Ready += ReadyAsync;
			_interactions.Log += LogAsync;

			await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
			_client.InteractionCreated += HandleInteractionAsync;
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log);
			return Task.CompletedTask;
		}

		private async Task ReadyAsync()
		{
			await _interactions.RegisterCommandsGloballyAsync();
		}

		private async Task HandleInteractionAsync(SocketInteraction interaction)
		{
			try
			{
				var context = new SocketInteractionContext(_client, interaction);
				var result = await _interactions.ExecuteCommandAsync(context, _serviceProvider);
			}
			catch
			{
				//Delete the bot's interaction acknowledgement if something goes wrong
				if (interaction.Type is InteractionType.ApplicationCommand)
					await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
			}
		}
	}
}