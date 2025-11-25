using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot.Modules
{
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
