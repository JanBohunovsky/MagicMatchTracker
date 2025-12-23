namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

public static class DiscordAuthenticationDefaults
{
	public const string AuthenticationScheme = "Discord";
	public const string DisplayName = "Discord";
	public const string ClaimsIssuer = "Discord";
	public const string CallbackPath = "/signin-discord";
	public const string AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
	public const string TokenEndpoint = "https://discord.com/api/oauth2/token";
	public const string TokenRevocationEndpoint = "https://discord.com/api/oauth2/token/revoke";
	public const string UserInformationEndpoint = "https://discord.com/api/users/@me";
}