using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MagicMatchTracker.Infrastructure.Reactive;
using Microsoft.EntityFrameworkCore;
using Raffinert.FuzzySharp;
using Raffinert.FuzzySharp.PreProcess;

namespace MagicMatchTracker.Features.Matches.Dialogs.DeckSelect;

public sealed class MatchDecksProvider : IDisposable
{
	private readonly Database _database;

	private CompositeDisposable _disposables = null!;
	private BehaviorSubject<Unit> _reloadRequested = null!;
	private Subject<Player?> _player = null!;
	private BehaviorSubject<string> _searchTerm = null!;
	private BehaviorSubject<IReadOnlyList<Deck>?> _decks = null!;

	public bool IsLoading => _decks.Value is null;
	public ReactiveReadOnlyProperty<IReadOnlyList<Deck>> Decks { get; private set; } = null!;

	public MatchDecksProvider(Database database)
	{
		_database = database;
		Initialise();
	}

	public void ReloadData()
	{
		_reloadRequested.OnNext(Unit.Default);
	}

	public void SetPlayer(Player? player)
	{
		_player.OnNext(player);
	}

	public void SetSearchTerm(string searchTerm)
	{
		_searchTerm.OnNext(searchTerm);
	}

	public void Reset()
	{
		_disposables.Dispose();
		Initialise();
	}

	private void Initialise()
	{
		_disposables = new CompositeDisposable();

		_reloadRequested = new BehaviorSubject<Unit>(Unit.Default)
			.DisposeWith(_disposables);

		_player = new Subject<Player?>()
			.DisposeWith(_disposables);

		_searchTerm = new BehaviorSubject<string>(string.Empty)
			.DisposeWith(_disposables);

		_decks = new BehaviorSubject<IReadOnlyList<Deck>?>(null)
			.DisposeWith(_disposables);

		_player.DistinctUntilChanged()
			.CombineLatest(_reloadRequested, (p, _) => p)
			.Do(_ => _decks.OnNext(null))
			.SelectLatestAsync(LoadDecksAsync)
			.Subscribe(_decks)
			.DisposeWith(_disposables);

		Decks = _decks.DistinctUntilChanged()
			.CombineLatest(_searchTerm.DistinctUntilChanged(), FilterDecks)
			.ToProperty([])
			.DisposeWith(_disposables);
	}

	private async Task<IReadOnlyList<Deck>> LoadDecksAsync(Player? player, CancellationToken cancellationToken)
	{
		IQueryable<Deck> query = _database.Decks
			.Where(d => !d.IsArchived)
			.OrderBy(d => d.Owner.Name)
			.ThenBy(d => d.Commander);

		if (player is not null)
		{
			query = query.Where(d => d.Owner == player);
		}

		return await query.ToListAsync(cancellationToken);
	}

	private IReadOnlyList<Deck> FilterDecks(IReadOnlyList<Deck>? decks, string searchTerm)
	{
		if (decks is null || searchTerm.IsEmpty())
			return decks ?? [];

		return decks.Select(CalculateDeckScore)
			.Where(x => x.Score > 60)
			.OrderByDescending(x => x.Score)
			.ThenBy(x => x.Deck.Owner.Name)
			.ThenBy(x => x.Deck.Commander)
			.Select(x => x.Deck)
			.ToList();

		(Deck Deck, int Score) CalculateDeckScore(Deck deck)
		{
			int[] scores =
			[
				CalculateValueScore(deck.Commander),
				CalculateValueScore(deck.Partner),
				CalculateValueScore(deck.Name),
			];

			return (deck, scores.Max());
		}

		int CalculateValueScore(string? value)
		{
			if (value is null)
				return 0;

			return Fuzz.WeightedRatio(searchTerm, value, PreprocessMode.Full);
		}
	}

	public void Dispose()
	{
		_disposables.Dispose();
	}
}