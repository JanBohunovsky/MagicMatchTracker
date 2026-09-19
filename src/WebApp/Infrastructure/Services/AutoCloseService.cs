using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MagicMatchTracker.Infrastructure.Services;

public sealed class AutoCloseService(IJSRuntime jsRuntime) : JavaScriptServiceBase(jsRuntime, "auto-close.js")
{
	public async Task<IAsyncDisposable> SubscribeAsync(ElementReference element, Func<Task> onCloseCallback)
	{
		var module = await GetModuleAsync();

		var onCloseHelper = DotNetObjectReference.Create(new OnCloseEventHelper(onCloseCallback));
		await module.InvokeVoidAsync("subscribe", element, onCloseHelper);

		return new OnCloseEventSubscription(UnsubscribeAsync, onCloseHelper);

		async Task UnsubscribeAsync()
		{
			await module.InvokeVoidAsync("unsubscribe", element);
		}
	}
}

file class OnCloseEventHelper(Func<Task> callback)
{
	[JSInvokable]
	public Task OnClose() => callback();
}

file class OnCloseEventSubscription(Func<Task> unsubscribeFunc, DotNetObjectReference<OnCloseEventHelper> onCloseHelper) : IAsyncDisposable
{
	public async ValueTask DisposeAsync()
	{
		await unsubscribeFunc();
		onCloseHelper.Dispose();
	}
}