using Microsoft.AspNetCore.Authentication;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

public sealed class DiscordAuthenticationHandlerProvider(IAuthenticationHandlerProvider provider)
{
	public async Task<DiscordAuthenticationHandler> GetHandlerAsync(HttpContext context)
	{
		var authHandler = await provider.GetHandlerAsync(context, DiscordAuthenticationDefaults.AuthenticationScheme);
		if (authHandler is not DiscordAuthenticationHandler discordAuthHandler)
			throw new InvalidOperationException("Failed to get Discord authentication handler, make sure it's registered under the Discord authentication scheme.");

		return discordAuthHandler;
	}
}