namespace MagicMatchTracker.Client.Data.Models;

public sealed class AppSettings
{
	public string Name { get; set; } = "Magic Match Tracker";
	public int MinMatchesForWinrate { get; set; } = 10;
	public int SeasonStartMonth { get; set; } = 1;
	public int SeasonStartDay { get; set; } = 1;

	public DateOnly CurrentSeasonStartDate => new(DateTime.Now.Year, SeasonStartMonth, SeasonStartDay);
}