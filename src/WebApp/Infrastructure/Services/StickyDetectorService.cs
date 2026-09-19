using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MagicMatchTracker.Infrastructure.Services;

public sealed class StickyDetectorService(IJSRuntime jsRuntime) : JavaScriptServiceBase(jsRuntime, "sticky-detector.js")
{
	public async Task TrackAsync(ElementReference element, string className)
	{
		var module = await GetModuleAsync();
		await module.InvokeVoidAsync("track", element, className);
	}

	public async Task UntrackAsync(ElementReference element)
	{
		var module = await GetModuleAsync();
		await module.InvokeVoidAsync("untrack", element);
	}
}