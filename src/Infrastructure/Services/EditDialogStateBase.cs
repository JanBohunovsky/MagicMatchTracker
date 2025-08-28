namespace MagicMatchTracker.Infrastructure.Services;

public abstract class EditDialogStateBase<TEditModel, TEntity> : StateBase where TEditModel : class where TEntity : IEntity
{
	private TaskCompletionSource<bool>? _dialogClosedCompletionSource = new();

	public TEditModel? Model { get; private set; }
	public bool IsNew { get; private set; }

	protected abstract TEditModel CreateEditModel(TEntity entity);

	protected abstract Task SaveCoreAsync(TEditModel model, CancellationToken cancellationToken);

	protected virtual Task InitialiseAsync(TEntity entity, CancellationToken cancellationToken)
		=> Task.CompletedTask;

	public async Task ShowDialogAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		IsNew = entity.Id == Guid.Empty;
		Model = CreateEditModel(entity);
		NotifyStateChanged();

		await InitialiseAsync(entity, cancellationToken);
		NotifyStateChanged();
	}

	public async Task<bool> ShowDialogAndWaitAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		_dialogClosedCompletionSource = new TaskCompletionSource<bool>();
		await using var cancellationRegistration = cancellationToken.Register(Cancel);

		// TODO: Cancellation token does not include dialog closing
		await ShowDialogAsync(entity, cancellationToken);
		var result = await _dialogClosedCompletionSource.Task;

		_dialogClosedCompletionSource = null;
		return result;
	}

	public void Cancel()
	{
		HideDialog(success: false);
	}

	public async Task SaveAsync(CancellationToken cancellationToken = default)
	{
		if (Model is null)
			return;

		await WithBusyAsync(() => SaveCoreAsync(Model, cancellationToken));

		HideDialog(success: true);
	}

	protected void HideDialog(bool success)
	{
		Model = null;
		NotifyStateChanged();

		_dialogClosedCompletionSource?.SetResult(success);
	}
}