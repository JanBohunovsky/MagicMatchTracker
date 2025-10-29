namespace MagicMatchTracker.Infrastructure.Services;

public abstract class StateBase
{
	private bool _isBusy;

	public event Action? StateChanged;

	public bool IsBusy
	{
		get => _isBusy;
		protected set
		{
			_isBusy = value;
			NotifyStateChanged();
		}
	}

	protected void NotifyStateChanged()
	{
		StateChanged?.Invoke();
	}

	protected void ExecuteWithStateChange(Action action)
	{
		action();
		NotifyStateChanged();
	}
}