namespace MagicMatchTracker.Features.Matches.Events;

public sealed record MatchDeletedEvent(Match Match) : IEvent;