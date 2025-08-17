namespace MagicMatchTracker.Data.Models;

public sealed class Player : IEntity
{
	public Guid Id { get; private init; }

	public required string Name { get; set; }

	public string? AvatarUri { get; set; }

	public List<Deck> Decks { get; set; } = [];

	public List<MatchParticipation> Matches { get; set; } = [];

	public DateTimeOffset CreatedAt { get; private init; } = DateTimeOffset.Now;
}