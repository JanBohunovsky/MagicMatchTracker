namespace MagicMatchTracker.Infrastructure.Components;

public sealed class CheckboxBindingProxy<T>(T value, Func<bool> getter, Action<bool> setter)
{
	public T Value { get; } = value;

	public bool IsChecked
	{
		get => getter();
		set => setter(value);
	}
}