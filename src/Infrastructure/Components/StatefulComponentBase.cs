using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace MagicMatchTracker.Infrastructure.Components;

public abstract class StatefulComponentBase<TState> : ComponentBase, IDisposable where TState : StateBase
{
	private readonly CancellationTokenSource _componentLifetimeCts = new();

	[Inject]
	protected TState State { get; set; } = null!;

	/// <summary>
	/// A cancellation token that is tied to the lifecycle of the component.
	/// </summary>
	/// <remarks>This token is cancelled when the component is disposed.</remarks>
	protected CancellationToken CancellationToken => _componentLifetimeCts.Token;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		State.StateChanged += StateHasChanged;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		State.StateChanged -= StateHasChanged;
		_componentLifetimeCts.Cancel();
		_componentLifetimeCts.Dispose();
		GC.SuppressFinalize(this);
	}
}