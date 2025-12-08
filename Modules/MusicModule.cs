using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;

namespace Dizmuze.Modules
{
	[RequireContext(ContextType.Guild)]
	public sealed class MusicModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly IAudioService _audioService;

		public MusicModule(IAudioService audioService)
		{
			ArgumentNullException.ThrowIfNull(audioService);

			_audioService = audioService;
		}

		[SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
		public async Task Play(string query)
		{
			await DeferAsync();
			var player = await GetPlayerAsync(connectToVoiceChannel: true);
			if (player is null)
			{
				return;
			}

			var track = await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.YouTube);
			if (track is null)
			{
				await FollowupAsync("No results.");
				return;
			}

			await player.PlayAsync(track);
			await FollowupAsync($"Playing: {track.Uri}");
		}

		[SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
		public async Task Stop()
		{
			var player = await GetPlayerAsync(connectToVoiceChannel: false);
			if (player is null)
			{
				return;
			}

			if (player.CurrentItem is null)
			{
				await RespondAsync("Nothing is playing!");
				return;
			}

			await player.StopAsync();
			await RespondAsync("Stopped playing.");
		}

		private async ValueTask<LavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
		{
			var retrieveOptions = new PlayerRetrieveOptions(
				ChannelBehavior: connectToVoiceChannel ? 
				PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

			var result = await _audioService.Players
				.RetrieveAsync(Context, playerFactory: PlayerFactory.Default, retrieveOptions);

			if (!result.IsSuccess)
			{
				var errorMessage = result.Status switch
				{
					PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
					PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
					_ => "Unknown error.",
				};

				await FollowupAsync(errorMessage);
				return null;
			}

			return result.Player;
		}
	}
}