using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Shared.Extensions;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Pages.Listing;

public sealed class PlayerListingState(Database database, PlayerEditDialogState editDialogState) : StateBase
{
	private List<Player>? _players;

	public IReadOnlyList<Player>? Players => _players;
	public IReadOnlyDictionary<Guid, Stats> PlayerStats { get; private set; } = new Dictionary<Guid, Stats>();

	public async Task LoadPlayersAsync(CancellationToken cancellationToken = default)
	{
		if (_players is not null)
			return;

		_players = await database.Players.ToListAsync(cancellationToken);
	}

	public async Task LoadStatsAsync(CancellationToken cancellationToken = default)
	{
		PlayerStats = await database.QueryPlayerStats()
			.ToDictionaryAsync(p => p.PlayerId, p => (Stats)p, cancellationToken);
	}

	public async Task AddNewPlayerAsync(CancellationToken cancellationToken = default)
	{
		if (_players is null)
			return;

		var player = new Player
		{
			Name = "",
		};
		var success = await editDialogState.ShowDialogAsync(player, cancellationToken);
		if (!success)
			return;

		_players.Add(player);
		NotifyStateChanged();
	}

	public async Task EditPlayerAsync(Player player, CancellationToken cancellationToken = default)
	{
		var success = await editDialogState.ShowDialogAsync(player, cancellationToken);
		if (success)
			NotifyStateChanged();
	}
}