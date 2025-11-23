using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiscordMusicBot
{
	public class AppConfig
	{
		//Treat config variables like constants
		public readonly struct DiscordConfig
		{
			public string Token { get; init; }
			public string Prefix { get; init; }

			[JsonConstructor]
			public DiscordConfig (string token, string prefix)
			{
				Token = token;
				Prefix = prefix;
			}
		}

		[JsonPropertyName("Discord")]
		public DiscordConfig Discord { get; init; }
	}
}
