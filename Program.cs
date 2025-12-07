using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordMusicBot.Services;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DiscordMusicBot.Settings;

namespace DiscordMusicBot
{
    internal sealed class Program
    {
		public static async Task Main(string[] args)
		{
			var builder = new HostApplicationBuilder(args);

			builder.Configuration.AddJsonFile("Config.json", optional:false);
			builder.Services.AddOptions<DiscordBotSettings>().Bind(builder.Configuration.GetSection("Discord"));

			builder.Services.AddSingleton<DiscordSocketClient>();
			builder.Services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

			builder.Services.AddLavalink();
			builder.Services.ConfigureLavalink(config =>
			{
				config.BaseAddress = new Uri(builder.Configuration["Lavalink:BaseAddress"]);
				config.Passphrase = builder.Configuration["Lavalink:Passphrase"];
			});

			builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

			builder.Services.AddHostedService<DiscordBot>();
			builder.Services.AddSingleton<InteractionHandler>();

			await builder.Build().RunAsync();
		}
	}
}
