using System.Security.Claims;
using System.Security.Principal;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

public static class DiscordIdentityExtensions
{
	extension(IIdentity identity)
	{
		public string? DisplayName => identity is ClaimsIdentity claimsIdentity
			? claimsIdentity.FindFirst(DiscordAuthenticationConstants.Claims.GlobalName)?.Value
			: null;

		public string? DiscordId => identity is ClaimsIdentity claimsIdentity
			? claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value
			: null;
	}
}