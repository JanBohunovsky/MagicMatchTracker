namespace MagicMatchTracker.Infrastructure.Services;

public abstract class StateBase
{
	public event Action? StateChanged;

	public bool IsBusy { get; protected set; }

	protected void NotifyStateChanged()
	{
		StateChanged?.Invoke();
	}

	protected void WithBusy(Action action)
	{
		IsBusy = true;
		try
		{
			action();
		}
		finally
		{
			IsBusy = false;
		}
	}

	protected async Task WithBusyAsync(Func<Task> action)
	{
		IsBusy = true;
		try
		{
			await action();
		}
		finally
		{
			IsBusy = false;
		}
	}
}