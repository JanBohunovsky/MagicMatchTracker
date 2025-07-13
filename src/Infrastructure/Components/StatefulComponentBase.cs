using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace MagicMatchTracker.Infrastructure.Components;

public abstract class StatefulComponentBase<TState> : ComponentBase, IDisposable where TState : StateBase
{
	[Inject]
	protected TState State { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		State.StateChanged += StateHasChanged;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		State.StateChanged -= StateHasChanged;
		GC.SuppressFinalize(this);
	}
}