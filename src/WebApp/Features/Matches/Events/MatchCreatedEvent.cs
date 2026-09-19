namespace MagicMatchTracker.Features.Matches.Events;

public sealed record MatchCreatedEvent(Match Match) : IEvent;