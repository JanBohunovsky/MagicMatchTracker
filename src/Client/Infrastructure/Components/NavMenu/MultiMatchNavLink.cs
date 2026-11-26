using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MagicMatchTracker.Client.Infrastructure.Components.NavMenu;

/// <summary>
/// A <see cref="NavLink"/> that is also active when the current location is equal to or nested under
/// one of <see cref="AdditionalPrefixes"/> (base-relative paths without leading slash).
/// </summary>
public sealed class MultiMatchNavLink : NavLink
{
	[Parameter]
	public IReadOnlyList<string> AdditionalPrefixes { get; set; } = [];

	[Inject]
	private NavigationManager Navigation { get; set; } = null!;

	protected override bool ShouldMatch(string uriAbsolute)
	{
		if (base.ShouldMatch(uriAbsolute))
			return true;

		if (AdditionalPrefixes.Count == 0)
			return false;

		var path = Navigation.ToBaseRelativePath(uriAbsolute);
		var end = path.IndexOfAny(['?', '#']);
		if (end >= 0)
			path = path[..end];

		return AdditionalPrefixes.Any(prefix =>
			path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
			|| path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase));
	}
}