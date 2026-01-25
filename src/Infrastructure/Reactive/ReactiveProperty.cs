using System.Reactive.Subjects;

namespace MagicMatchTracker.Infrastructure.Reactive;

public sealed class ReactiveProperty<T>(T initialValue) : IObservable<T>, IDisposable
{
	private readonly BehaviorSubject<T> _subject = new(initialValue);

	public T Value
	{
		get => _subject.Value;
		set => _subject.OnNext(value);
	}

	public void NotifyPropertyChanged()
		=> _subject.OnNext(_subject.Value);

	public IDisposable Subscribe(IObserver<T> observer)
		=> _subject.Subscribe(observer);

	public void Dispose()
	{
		_subject.Dispose();
	}
}