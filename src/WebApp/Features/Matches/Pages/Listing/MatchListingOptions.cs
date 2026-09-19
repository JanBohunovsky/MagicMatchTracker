namespace MagicMatchTracker.Features.Matches.Pages.Listing;

public sealed record MatchListingOptions
{
	public const string SectionName = "MatchListing";

	public int DaysPerPage { get; init; } = 5;
}