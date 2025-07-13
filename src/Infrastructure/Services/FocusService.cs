using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MagicMatchTracker.Infrastructure.Services;

public sealed class FocusService : IAsyncDisposable
{
	private readonly Lazy<ValueTask<IJSObjectReference>> _module;

	public FocusService(IJSRuntime jsRuntime)
	{
		_module = new Lazy<ValueTask<IJSObjectReference>>(() =>
			jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/focus.js"));
	}

	/// <summary>
	/// Focuses the first descendant of the <paramref name="parent"/> with <c>autofocus</c> attribute.
	/// </summary>
	/// <param name="parent">The parent element. Defaults to <c>window.document</c> if <see langword="null"/>.</param>
	/// <remarks>Make sure the <paramref name="parent"/> has already been rendered.</remarks>
	public async Task AutoFocusAsync(ElementReference? parent)
	{
		await FocusFirstAsync("[autofocus]", parent);
	}

	/// <summary>
	/// Focuses the first descendant of the <paramref name="parent"/> that matches the <paramref name="selector"/>.
	/// </summary>
	/// <param name="selector">The selector for the element to be focused.</param>
	/// <param name="parent">The parent elements. Defaults to <c>window.document</c> if <see langword="null"/>.</param>
	public async Task FocusFirstAsync(string selector, ElementReference? parent = null)
	{
		var module = await _module.Value;
		await module.InvokeVoidAsync("focusFirst", parent, selector);
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (!_module.IsValueCreated)
			return;

		var module = await _module.Value;
		await module.DisposeAsync();
	}
}