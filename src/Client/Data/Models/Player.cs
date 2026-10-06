namespace MagicMatchTracker.Client.Data.Models;

public sealed class Player : IStats
{
	public string Slug { get; private set; } = string.Empty;

	public required string Username
	{
		get;
		set
		{
			field = value;
			Slug = value.ToSlug();
		}
	}

	public string? AvatarUri { get; set; }

	public int Decks { get; set; }
	public int Matches { get; set; }
	public int Wins { get; set; }
}