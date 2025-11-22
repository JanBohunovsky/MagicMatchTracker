namespace MagicMatchTracker.Infrastructure.Components;

public sealed record DropdownItem
{
	public string? Icon { get; init; }
	public required string Label { get; init; }
	public required Func<Task> Action { get; init; }
}