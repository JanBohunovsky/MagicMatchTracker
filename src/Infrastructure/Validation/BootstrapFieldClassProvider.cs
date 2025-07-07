using Microsoft.AspNetCore.Components.Forms;

namespace MagicMatchTracker.Infrastructure.Validation;

public sealed class BootstrapFieldClassProvider : FieldCssClassProvider
{
	public static BootstrapFieldClassProvider Instance { get; } = new();

	/// <inheritdoc />
	public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
	{
		var isValid = editContext.IsValid(fieldIdentifier);
		return isValid ? string.Empty : "is-invalid";
	}
}