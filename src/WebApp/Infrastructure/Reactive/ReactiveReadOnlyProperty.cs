using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MagicMatchTracker.Infrastructure.Reactive;

public static class ReactiveReadOnlyProperty
{
	public static ReactiveReadOnlyProperty<T> Empty<T>() => new(Observable.Empty<T>(), default!);
}

public sealed class ReactiveReadOnlyProperty<T> : IObservable<T>, IDisposable
{
	private readonly BehaviorSubject<T> _subject;
	private readonly CompositeDisposable _disposables = new();

	public T Value => _subject.Value;

	public ReactiveReadOnlyProperty(IObservable<T> observable, T initialValue)
	{
		_subject = new BehaviorSubject<T>(initialValue);

		observable.StartWith(initialValue)
			.DistinctUntilChanged()
			.Subscribe(_subject)
			.DisposeWith(_disposables);
	}

	public void NotifyPropertyChanged()
		=> _subject.OnNext(_subject.Value);

	public IDisposable Subscribe(IObserver<T> observer)
		=> _subject.Subscribe(observer);

	public void Dispose()
	{
		_subject.Dispose();
		_disposables.Dispose();
	}
}