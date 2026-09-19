using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace MagicMatchTracker.Infrastructure.Components;

public abstract class StatefulComponentBase<TState> : LifecycleComponentBase where TState : StateBase
{
	[Inject]
	protected TState State { get; set; } = null!;

	protected override void OnInitialized()
	{
		State.StateChanged += StateHasChanged;
	}

	protected override void DisposeCore()
	{
		State.StateChanged -= StateHasChanged;
	}
}