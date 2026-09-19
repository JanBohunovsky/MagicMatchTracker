namespace MagicMatchTracker.Features.Matches.Pages.Detail;

public sealed record MatchError(string Message, IReadOnlyCollection<MatchParticipation> AffectedParticipations);