using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MagicMatchTracker.Infrastructure.Extensions;

// Taken from Microsoft.AspNetCore.Components.Form.AttributeUtilities because for some reason that class is internal
public static class AttributeUtilities
{
	[return: NotNullIfNotNull(nameof(classNames))]
	public static string? CombineClassNames(IReadOnlyDictionary<string, object>? additionalAttributes, string? classNames)
	{
		if (additionalAttributes is null || !additionalAttributes.TryGetValue("class", out var @class))
		{
			return classNames;
		}

		var classAttributeValue = Convert.ToString(@class, CultureInfo.InvariantCulture);

		if (classAttributeValue.IsEmpty())
		{
			return classNames;
		}

		if (classNames.IsEmpty())
		{
			return classAttributeValue;
		}

		return $"{classAttributeValue} {classNames}";
	}
}