using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Dizmuze.Services;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dizmuze.Settings;

namespace Dizmuze
{
    internal sealed class Program
    {
		public static async Task Main(string[] args)
		{
			var builder = new HostApplicationBuilder(args);

			//Config section
			builder.Configuration.AddJsonFile("Config.json", optional:false);
			builder.Services.AddOptions<DiscordBotSettings>().Bind(builder.Configuration.GetSection("Discord"));

			//Discord.NET library services
			builder.Services.AddSingleton<DiscordSocketClient>();
			builder.Services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

			//Lavalink4NET library services
			builder.Services.AddLavalink();
			builder.Services.ConfigureLavalink(config =>
			{
				config.BaseAddress = new Uri(builder.Configuration["Lavalink:BaseAddress"]);
				config.Passphrase = builder.Configuration["Lavalink:Passphrase"];
			});

			//Own services
			builder.Services.AddHostedService<DiscordBot>();
			builder.Services.AddSingleton<InteractionHandler>();

			//Logging section
			builder.Services.AddLogging(x => x.AddConsole().AddConfiguration(builder.Configuration.GetSection("Logging")));

			await builder.Build().RunAsync();
		}
	}
}
