using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MagicMatchTracker.Infrastructure.Validation;

public sealed class FluentValidator : ComponentBase, IDisposable
{
	private IDisposable? _subscriptions;
	private EditContext? _originalEditContext;

	[CascadingParameter]
	private EditContext? EditContext { get; set; }

	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (EditContext is null)
		{
			throw new NullReferenceException($"{nameof(FluentValidator)} requires a cascading parameter of type EditContext. "
				+ $"For example, you can use {nameof(FluentValidator)} inside an EditForm.");
		}

		_subscriptions = EditContext.EnableFluentValidation(ServiceProvider);
		_originalEditContext = EditContext;
	}

	/// <inheritdoc />
	protected override void OnParametersSet()
	{
		if (_originalEditContext != EditContext)
			throw new InvalidOperationException($"{nameof(FluentValidator)} does not support changing the EditContext dynamically.");
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_subscriptions?.Dispose();
		_subscriptions = null;
	}
}