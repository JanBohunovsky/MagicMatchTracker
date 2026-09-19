using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Features.Shared.Extensions;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Players.Pages.Detail;

public abstract class PlayerDetailStateBase : StateBase
{
	private readonly PlayerEditDialogState _playerEditDialogState;

	public bool IsLoading { get; private set; }
	public Player? Player { get; private set; }
	public Stats? PlayerStats { get; private set; }

	protected Database Database { get; }

	// ReSharper disable once ConvertToPrimaryConstructor - Issue with capturing `database` twice
	protected PlayerDetailStateBase(Database database, PlayerEditDialogState playerEditDialogState)
	{
		Database = database;
		_playerEditDialogState = playerEditDialogState;
	}

	public async Task LoadPlayerAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Player?.Id == id)
			return;

		ExecuteWithStateChange(() => IsLoading = true);

		Player = await LoadPlayerCoreAsync(id, cancellationToken);
		PlayerStats = await Database.QueryPlayerStats()
			.FirstOrDefaultAsync(p => p.PlayerId == id, cancellationToken);

		ExecuteWithStateChange(() => IsLoading = false);
	}

	public async Task EditPlayerAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var success = await _playerEditDialogState.ShowDialogAsync(Player, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	protected abstract Task<Player?> LoadPlayerCoreAsync(Guid id, CancellationToken cancellationToken);
}