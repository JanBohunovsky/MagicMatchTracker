using MagicMatchTracker.Features.Players.Models;
using MagicMatchTracker.Infrastructure.Services;

namespace MagicMatchTracker.Features.Players.Services;

public sealed class PlayerEditState(Database database) : StateBase
{
	private TaskCompletionSource<bool>? _dialogTaskSource = new();

	public PlayerEditModel? Model { get; private set; }
	public bool IsNew { get; private set; }

	public void ShowDialog(Player player)
	{
		IsNew = player.Id == Guid.Empty;
		Model = new PlayerEditModel(player);
		NotifyStateChanged();
	}

	public async Task<bool> ShowDialogAsync(Player player)
	{
		_dialogTaskSource = new TaskCompletionSource<bool>();

		ShowDialog(player);
		var result = await _dialogTaskSource.Task;

		_dialogTaskSource = null;
		return result;
	}

	public void Cancel()
	{
		HideDialog(cancelled: true);
	}

	public async Task SaveAsync()
	{
		if (Model is null)
			return;

		await WithBusyAsync(SaveAsyncImpl);

		HideDialog(cancelled: false);
		return;

		async Task SaveAsyncImpl()
		{
			var player = Model.ApplyChanges();
			if (IsNew)
				database.Players.Add(player);

			await database.SaveChangesAsync();
		}
	}

	private void HideDialog(bool cancelled)
	{
		Model = null;
		NotifyStateChanged();

		_dialogTaskSource?.SetResult(!cancelled);
	}
}