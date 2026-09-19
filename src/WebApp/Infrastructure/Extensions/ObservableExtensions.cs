using System.Reactive.Linq;
using MagicMatchTracker.Infrastructure.Reactive;

namespace MagicMatchTracker.Infrastructure.Extensions;

public static class ObservableExtensions
{
	extension<T>(IObservable<T> observable)
	{
		public IDisposable SubscribeAsync(Func<T, Task> actionAsync)
		{
			return observable.Select(x => Observable.FromAsync(async () => await actionAsync(x).ConfigureAwait(false)))
				.Concat()
				.Subscribe();
		}

		public IDisposable SubscribeAsync(Func<T, CancellationToken, Task> actionAsync)
		{
			return observable.Select(x => Observable.FromAsync(async ct => await actionAsync(x, ct).ConfigureAwait(false)))
				.Concat()
				.Subscribe();
		}

		/// <summary>
		/// Projects each element of an observable sequence into a new form by performing an asynchronous operation.
		/// If a new value is produced by the source observable before the previous asynchronous operation completes,
		/// the <see cref="CancellationToken"/> for the previous operation is cancelled.
		/// </summary>
		/// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
		/// <param name="selectorTask">An asynchronous transform function to apply to each element; the second parameter of the function represents a cancellation token for the current operation.</param>
		/// <returns>An observable sequence whose elements are the result of invoking the transform function on each element of source.</returns>
		public IObservable<TResult> SelectLatestAsync<TResult>(Func<T, CancellationToken, Task<TResult>> selectorTask)
		{
			return observable.Select(x => Observable.DeferAsync(async ct =>
				{
					var result = await selectorTask(x, ct).ConfigureAwait(false);
					return Observable.Return(result);
				}))
				.Switch();
		}

		public ReactiveReadOnlyProperty<T> ToProperty(T initialValue = default!)
		{
			return new ReactiveReadOnlyProperty<T>(observable, initialValue);
		}
	}
}