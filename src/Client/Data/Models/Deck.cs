using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data.Models;

public sealed class Deck : IStats
{
	public string Id { get; private set; } = string.Empty;

	public required string OwnerId { get; set; }

	[JsonIgnore]
	public Player Owner
	{
		get;
		set
		{
			field = value;
			OwnerId = value.Id;
		}
	}

	public required string Name { get; set; }

	public required string Commander
	{
		get;
		set
		{
			field = value;
			UpdateId();
		}
	}

	public string? Partner
	{
		get;
		set
		{
			field = value;
			UpdateId();
		}
	}

	public string? ImageUrl { get; set; }
	public string? DeckUrl { get; set; }
	public bool IsRetired { get; set; }

	public int Matches { get; set; }
	public int Wins { get; set; }

	private void UpdateId()
	{
		var commanderSlug = Commander.ToSlug();
		Id = Partner is not null ? $"{commanderSlug}--{Partner.ToSlug()}" : commanderSlug;
	}
}