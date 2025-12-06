using MagicMatchTracker.Features.Players.Dialogs.Edit;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Players.Pages.Detail;

public abstract class PlayerDetailStateBase(PlayerEditDialogState playerEditDialogState) : StateBase
{
	public bool IsLoading { get; private set; }
	public Player? Player { get; private set; }

	public async Task LoadPlayerAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Player?.Id == id)
			return;

		ExecuteWithStateChange(() => IsLoading = true);

		Player = await LoadPlayerCoreAsync(id, cancellationToken);

		ExecuteWithStateChange(() => IsLoading = false);
	}

	public async Task EditPlayerAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null)
			return;

		var success = await playerEditDialogState.ShowDialogAsync(Player, cancellationToken);
		if (success)
			NotifyStateChanged();
	}

	protected abstract Task<Player?> LoadPlayerCoreAsync(Guid id, CancellationToken cancellationToken);
}