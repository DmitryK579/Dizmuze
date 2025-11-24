using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot
{
	internal class CommandHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly AppConfig _config;

		public CommandHandler(DiscordSocketClient client, CommandService commands, AppConfig config)
		{
			_commands = commands;
			_client = client;
			_config = config;
		}

		public async Task InstallCommandsAsync()
		{
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
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
			if (userMessage.HasStringPrefix(_config.Discord.Prefix, ref argPos))
			{
				var context = new SocketCommandContext(_client, userMessage);
				var result = await _commands.ExecuteAsync(context, argPos, null);
				if (!result.IsSuccess)
					Console.WriteLine(result.ErrorReason);
			}
		}
	}
	//Modules need to be public to be discovered by AddModulesAsync
	public class TestModule : ModuleBase<SocketCommandContext>
	{
		[Command("info")]
		[Summary("Provides info of the bot")]
		public async Task ReplyInfoAsync()
		{
			var embed = new EmbedBuilder()
			.WithTitle("Bot Info")
			.WithDescription("Dizmuze - a music bot built using C# and Discord.Net!")
			.WithColor(Color.Blue)
			.AddField("Developer", "DmitryK579")
			.AddField("Commands", "`info`")
			.Build();

			await Context.Channel.SendMessageAsync(embed: embed);
		}
	}
}
