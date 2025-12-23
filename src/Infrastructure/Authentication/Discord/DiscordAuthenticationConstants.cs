namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

public static class DiscordAuthenticationConstants
{
	public static class Claims
	{
		public const string GlobalName = "urn:discord:user:global_name";
		public const string Discriminator = "urn:discord:user:discriminator";
		public const string AvatarHash = "urn:discord:avatar:hash";
	}

	public static class Prompts
	{
		public const string None = "none";
		public const string Consent = "consent";
	}
}