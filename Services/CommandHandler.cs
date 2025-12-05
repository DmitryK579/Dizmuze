using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot.Services
{
	internal class CommandHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly IConfiguration _config;
		private readonly IServiceProvider _serviceProvider;

		public CommandHandler(DiscordSocketClient client, CommandService commands, IConfiguration config, 
							  IServiceProvider serviceProvider)
		{
			_commands = commands;
			_client = client;
			_config = config;
			_serviceProvider = serviceProvider;
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log);
			return Task.CompletedTask;
		}

		public async Task InstallCommandsAsync()
		{
			_commands.Log += LogAsync;
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
			_client.MessageReceived += HandleCommandAsync;
		}

		private async Task HandleCommandAsync(SocketMessage socketMessage)
		{
			// Don't process the command if it was a system userMessage
			var userMessage = socketMessage as SocketUserMessage;
			if (userMessage == null)
				return;

			//Do not respond to own messages to avoid an infinite loop
			if (userMessage.Author.IsBot)
				return;

			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;
			if (userMessage.HasStringPrefix(_config["Discord:Prefix"], ref argPos))
			{
				var context = new SocketCommandContext(_client, userMessage);
				var result = await _commands.ExecuteAsync(context, argPos, null);
				if (!result.IsSuccess)
					Console.WriteLine(result.ErrorReason);
			}
		}
	}
}
