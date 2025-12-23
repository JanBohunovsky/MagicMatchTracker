using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using static MagicMatchTracker.Infrastructure.Authentication.Discord.DiscordAuthenticationConstants;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

public sealed class DiscordAuthenticationOptions : OAuthOptions
{
	public string? OwnerId { get; set; }
	public string? Prompt { get; set; }

	public DiscordAuthenticationOptions()
	{
		ClaimsIssuer = DiscordAuthenticationDefaults.ClaimsIssuer;
		CallbackPath = DiscordAuthenticationDefaults.CallbackPath;
		AuthorizationEndpoint = DiscordAuthenticationDefaults.AuthorizationEndpoint;
		TokenEndpoint = DiscordAuthenticationDefaults.TokenEndpoint;
		UserInformationEndpoint = DiscordAuthenticationDefaults.UserInformationEndpoint;

		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
		ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
		ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
		ClaimActions.MapJsonKey(Claims.GlobalName, "global_name");
		ClaimActions.MapJsonKey(Claims.Discriminator, "discriminator");
		ClaimActions.MapJsonKey(Claims.AvatarHash, "avatar");

		Scope.Add("identify");
	}
}