namespace MagicMatchTracker.Infrastructure.Components;

public sealed class CheckableItem<T>(T value, bool isChecked)
{
	private readonly bool _wasChecked = isChecked;

	public T Value { get; } = value;

	public bool IsChecked { get; set; } = isChecked;

	public bool IsChanged => IsChecked != _wasChecked;
}