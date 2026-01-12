namespace MagicMatchTracker.Features.Players.Pages.Listing;

public enum PlayerSortOption
{
	Name,
	Matches,
	WinRate,
	LastPlayed,
}

public static class PlayerSortOptionExtensions
{
	extension(PlayerSortOption sortOption)
	{
		public string DisplayName => sortOption switch
		{
			PlayerSortOption.Name => "Name",
			PlayerSortOption.Matches => "Matches",
			PlayerSortOption.WinRate => "Win rate",
			PlayerSortOption.LastPlayed => "Last played",
			_ => throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null),
		};

		public string Icon => sortOption switch
		{
			PlayerSortOption.Name => "person",
			PlayerSortOption.Matches => "swords",
			PlayerSortOption.WinRate => "trophy",
			PlayerSortOption.LastPlayed => "calendar_today",
			_ => throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null),
		};
	}
}