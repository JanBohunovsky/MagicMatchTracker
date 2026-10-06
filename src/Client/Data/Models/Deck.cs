namespace MagicMatchTracker.Client.Data.Models;

public sealed class Deck : IStats
{
	public string Slug { get; private set; } = string.Empty;

	public required string OwnerSlug { get; init; }

	public required string Commander
	{
		get;
		set
		{
			field = value;
			UpdateValues();
		}
	}

	public string? Partner
	{
		get;
		set
		{
			field = value;
			UpdateValues();
		}
	}

	public required string Title { get; set; }
	public required string SubTitle { get; set; }

	public string? ImageUri { get; set; }
	public bool IsArchived { get; set; }

	public int Matches { get; set; }
	public int Wins { get; set; }

	private void UpdateValues()
	{
		var commanderSlug = Commander.ToSlug();
		Slug = Partner is not null ? $"{commanderSlug}--{Partner.ToSlug()}" : commanderSlug;
	}
}