namespace MagicMatchTracker.Infrastructure.Services;

public abstract class StateBase
{
	public event Action? StateChanged;

	public bool IsBusy
	{
		get;
		protected set
		{
			field = value;
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