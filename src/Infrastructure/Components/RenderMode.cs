using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MagicMatchTracker.Infrastructure.Components;

[PublicAPI]
public static class RenderMode
{
	/// <summary>
	/// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the server via Blazor Server hosting
	/// without server-side prerendering.
	/// </summary>
	public static InteractiveServerRenderMode InteractiveServerNoPreRender { get; } = new(prerender: false);

	/// <summary>
	/// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the client via Blazor WebAssembly hosting
	/// without server-side prerendering.
	/// </summary>
	public static InteractiveWebAssemblyRenderMode InteractiveWebAssemblyNoPreRender { get; } = new(prerender: false);

	/// <summary>
	/// Gets an <see cref="IComponentRenderMode"/> that means the render mode will be determined automatically based on a policy
	/// without server-side prerendering.
	/// </summary>
	public static InteractiveAutoRenderMode InteractiveAutoNoPreRender { get; } = new(prerender: false);
}