using Microsoft.JSInterop;

namespace MagicMatchTracker.Infrastructure.Services;

public abstract class JavaScriptServiceBase : IAsyncDisposable
{
	private readonly Lazy<ValueTask<IJSObjectReference>> _module;

	protected JavaScriptServiceBase(IJSRuntime jsRuntime, string moduleFileName)
	{
		_module = new Lazy<ValueTask<IJSObjectReference>>(() =>
			jsRuntime.InvokeAsync<IJSObjectReference>("import", $"./js/{moduleFileName}"));
	}

	protected ValueTask<IJSObjectReference> GetModuleAsync()
		=> _module.Value;

	protected virtual ValueTask DisposeCoreAsync()
	{
		return ValueTask.CompletedTask;
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeCoreAsync();

		if (!_module.IsValueCreated)
			return;

		var module = await _module.Value;
		await module.DisposeAsync();
		GC.SuppressFinalize(this);
	}
}