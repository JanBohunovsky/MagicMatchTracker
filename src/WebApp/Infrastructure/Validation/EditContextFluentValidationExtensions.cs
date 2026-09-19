using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace MagicMatchTracker.Infrastructure.Validation;

public static class EditContextFluentValidationExtensions
{
	/// <summary>
	/// Enables FluentValidation validation support for the <see cref="EditContext"/>.
	/// </summary>
	/// <returns>A disposable object whose disposal will remove FluentValidation validation support from the <see cref="EditContext"/>.</returns>
	public static IDisposable EnableFluentValidation(this EditContext editContext, IServiceProvider serviceProvider)
	{
		return new FluentValidationEventSubscriptions(editContext, serviceProvider);
	}

	private sealed class FluentValidationEventSubscriptions : IDisposable
	{
		private readonly EditContext _editContext;
		private readonly ValidationMessageStore _messages;
		private readonly FieldIdentifierConverter _converter;
		private readonly IValidator _validator = null!;

		public FluentValidationEventSubscriptions(EditContext editContext, IServiceProvider serviceProvider)
		{
			_editContext = editContext;
			_messages = new ValidationMessageStore(_editContext);
			_converter = new FieldIdentifierConverter(editContext);

			var validatorType = typeof(IValidator<>).MakeGenericType(_editContext.Model.GetType());
			if (serviceProvider.GetService(validatorType) is not IValidator validator)
				return;

			_validator = validator;
			_editContext.OnFieldChanged += OnFieldChanged;
			_editContext.OnValidationRequested += OnValidationRequested;
		}

		private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
		{
			var propertyPath = _converter.ConvertToPropertyPath(e.FieldIdentifier);
			if (propertyPath is null)
				return;

			var context = ValidationContext<object>.CreateWithOptions(_editContext.Model, strategy =>
			{
				strategy.IncludeProperties(propertyPath);
			});

			var result = _validator.Validate(context);
			var errors = result.Errors
				.Where(f => f.PropertyName == propertyPath)
				.Select(f => f.ErrorMessage)
				.Distinct();

			_messages.Clear(e.FieldIdentifier);
			_messages.Add(e.FieldIdentifier, errors);

			_editContext.NotifyValidationStateChanged();
		}

		private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
		{
			var context = new ValidationContext<object>(_editContext.Model);
			var result = _validator.Validate(context);

			_messages.Clear();
			foreach (var failure in result.Errors)
			{
				var fieldIdentifier = _converter.ConvertFromPropertyPath(failure.PropertyName);
				_messages.Add(fieldIdentifier, failure.ErrorMessage);
			}

			_editContext.NotifyValidationStateChanged();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_messages.Clear();
			_editContext.OnFieldChanged -= OnFieldChanged;
			_editContext.OnValidationRequested -= OnValidationRequested;
			_editContext.NotifyValidationStateChanged();
		}
	}
}