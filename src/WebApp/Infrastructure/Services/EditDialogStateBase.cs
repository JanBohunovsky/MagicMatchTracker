namespace MagicMatchTracker.Infrastructure.Services;

public abstract class EditDialogStateBase<TEditModel, TEntity> : StateBase where TEditModel : class
{
	private TaskCompletionSource<bool>? _dialogClosedCompletionSource = new();

	public TEditModel? Model { get; private set; }

	protected abstract TEditModel CreateEditModel(TEntity entity);

	protected abstract Task SaveCoreAsync(TEditModel model, CancellationToken cancellationToken);

	public void ShowDialog(TEntity entity)
	{
		Model = CreateEditModel(entity);
		NotifyStateChanged();
	}

	public async Task<bool> ShowDialogAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		_dialogClosedCompletionSource = new TaskCompletionSource<bool>();
		await using var cancellationRegistration = cancellationToken.Register(Cancel);

		ShowDialog(entity);
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

		IsBusy = true;
		await SaveCoreAsync(Model, cancellationToken);
		IsBusy = false;

		HideDialog(success: true);
	}

	protected void HideDialog(bool success)
	{
		Model = null;
		NotifyStateChanged();

		_dialogClosedCompletionSource?.SetResult(success);
	}
}