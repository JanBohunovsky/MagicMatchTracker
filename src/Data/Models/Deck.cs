namespace MagicMatchTracker.Data.Models;

public sealed class Deck : IEntity
{
	public Guid Id { get; private init; }

	public required Player Owner { get; set; }

	public string? Name { get; set; }

	public required string Commander { get; set; }

	public string? Partner { get; set; }

	public Colours ColourIdentity { get; set; }

	public string? ImageUri { get; set; }

	public string? DeckUri { get; set; }

	public bool IsArchived { get; set; }

	public List<MatchParticipation> Matches { get; set; } = [];

	public DateTimeOffset CreatedAt { get; private init; } = DateTimeOffset.Now;
}