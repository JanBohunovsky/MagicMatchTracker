namespace MagicMatchTracker.Features.Shared.Options;

public sealed class ChangelogOptions
{
	public const string SectionName = "Changelog";

	public string UrlFormat { get; set; } = string.Empty;
}