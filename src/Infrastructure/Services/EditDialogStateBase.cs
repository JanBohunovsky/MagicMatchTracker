namespace MagicMatchTracker.Infrastructure.Services;

public abstract class EditDialogStateBase<TEditModel, TEntity> : StateBase where TEditModel : class where TEntity : IEntity
{
	private TaskCompletionSource<bool>? _dialogTaskSource = new();

	public TEditModel? Model { get; private set; }
	public bool IsNew { get; private set; }

	protected abstract TEditModel CreateEditModel(TEntity entity);

	protected abstract Task SaveCoreAsync(TEditModel model, CancellationToken cancellationToken);

	public void ShowDialog(TEntity entity)
	{
		IsNew = entity.Id == Guid.Empty;
		Model = CreateEditModel(entity);
		NotifyStateChanged();
	}

	public async Task<bool> ShowDialogAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		_dialogTaskSource = new TaskCompletionSource<bool>();
		await using var cancellationRegistration = cancellationToken.Register(Cancel);

		ShowDialog(entity);
		var result = await _dialogTaskSource.Task;

		_dialogTaskSource = null;
		return result;
	}

	public void Cancel()
	{
		HideDialog(cancelled: true);
	}

	public async Task SaveAsync(CancellationToken cancellationToken = default)
	{
		if (Model is null)
			return;

		await WithBusyAsync(() => SaveCoreAsync(Model, cancellationToken));

		HideDialog(cancelled: false);
	}

	private void HideDialog(bool cancelled)
	{
		Model = null;
		NotifyStateChanged();

		_dialogTaskSource?.SetResult(!cancelled);
	}
}