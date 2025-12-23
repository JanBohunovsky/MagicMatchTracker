using Microsoft.AspNetCore.Authentication;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

public static class DiscordAuthenticationExtensions
{
	extension(AuthenticationBuilder builder)
	{
		public AuthenticationBuilder AddDiscord()
			=> builder.AddDiscord(_ => { });

		public AuthenticationBuilder AddDiscord(Action<DiscordAuthenticationOptions> configureOptions)
			=> builder.AddDiscord(DiscordAuthenticationDefaults.AuthenticationScheme, configureOptions);

		public AuthenticationBuilder AddDiscord(string scheme, Action<DiscordAuthenticationOptions> configureOptions)
			=> builder.AddDiscord(scheme, DiscordAuthenticationDefaults.DisplayName, configureOptions);

		public AuthenticationBuilder AddDiscord(string scheme, string? displayName, Action<DiscordAuthenticationOptions> configureOptions)
			=> builder.AddOAuth<DiscordAuthenticationOptions, DiscordAuthenticationHandler>(scheme, displayName!, configureOptions);
	}
}