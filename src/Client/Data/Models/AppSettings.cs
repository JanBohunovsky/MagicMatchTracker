namespace MagicMatchTracker.Client.Data.Models;

public sealed class AppSettings
{
	public string Name { get; set; } = "Magic Match Tracker";
	public int MinMatchesForWinrate { get; set; } = 10;
}