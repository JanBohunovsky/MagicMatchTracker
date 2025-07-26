using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Services;

public sealed class PlayerListingState(Database database, PlayerEditState editState) : StateBase
{
	private List<Player>? _players;

	public IReadOnlyList<Player>? Players => _players;

	public async Task LoadPlayersAsync(CancellationToken cancellationToken = default)
	{
		if (_players is not null)
			return;

		_players = await database.Players
			.OrderBy(p => p.CreatedAt)
			.ToListAsync(cancellationToken);
	}

	public async Task AddNewPlayerAsync(CancellationToken cancellationToken = default)
	{
		if (_players is null)
			return;

		var player = new Player
		{
			Name = "",
		};
		var success = await editState.ShowDialogAsync(player, cancellationToken);
		if (!success)
			return;

		_players.Add(player);
		NotifyStateChanged();
	}

	public async Task EditPlayerAsync(Player player, CancellationToken cancellationToken = default)
	{
		var success = await editState.ShowDialogAsync(player, cancellationToken);
		if (success)
			NotifyStateChanged();
	}
}