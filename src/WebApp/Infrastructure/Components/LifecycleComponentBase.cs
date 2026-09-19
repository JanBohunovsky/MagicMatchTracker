using Microsoft.AspNetCore.Components;

namespace MagicMatchTracker.Infrastructure.Components;

/// <summary>
/// Base class for components with a <see cref="CancellationToken"/> tied to the lifecycle of the component.
/// </summary>
public abstract class LifecycleComponentBase : ComponentBase, IDisposable
{
	private readonly CancellationTokenSource _componentLifetimeCts = new();

	/// <summary>
	/// A cancellation token that is tied to the lifecycle of the component.
	/// </summary>
	/// <remarks>This token is cancelled when the component is disposed of.</remarks>
	protected CancellationToken CancellationToken => _componentLifetimeCts.Token;

	protected virtual void DisposeCore() { }

	public void Dispose()
	{
		DisposeCore();
		_componentLifetimeCts.Cancel();
		_componentLifetimeCts.Dispose();
		GC.SuppressFinalize(this);
	}
}