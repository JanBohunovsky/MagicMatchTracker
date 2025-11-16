namespace MagicMatchTracker.Features.Matches.Models;

public sealed record MatchError(string Message, IReadOnlyCollection<MatchParticipation> AffectedParticipations);