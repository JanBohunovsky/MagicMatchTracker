using System.Collections;
using Raffinert.FuzzySharp;
using Raffinert.FuzzySharp.PreProcess;

namespace MagicMatchTracker.Features.Matches.Dialogs.DeckSelect;

// TODO: Improve name? Extract the search functionality somewhere else?
public sealed class SearchableDecks(Action stateChangedCallback) : IEnumerable<Deck>
{
	private List<Deck>? _sourceDecks;
	private List<Deck>? _visibleDecks;
	private string _searchTerm = string.Empty;

	public int Count => _visibleDecks?.Count ?? 0;
	public bool HasSource => _sourceDecks is not null;

	public void UpdateSource(IReadOnlyList<Deck>? decks)
	{
		_sourceDecks = decks?.OrderBy(d => d.Owner.Name)
			.ThenBy(d => d.Name ?? d.Commander)
			.ToList();

		UpdateDecks();
	}

	public void UpdateSearchTerm(string searchTerm)
	{
		_searchTerm = searchTerm.ToLowerInvariant();
		UpdateDecks();
	}

	public void Reset()
	{
		_sourceDecks = null;
		_visibleDecks = null;
		_searchTerm = string.Empty;
		stateChangedCallback();
	}

	private void UpdateDecks()
	{
		if (_sourceDecks is null || _searchTerm.IsEmpty())
		{
			_visibleDecks = _sourceDecks;
			stateChangedCallback();
			return;
		}

		_visibleDecks = _sourceDecks.Select(ApplySearch)
			.Where(x => x.Score > 60)
			.OrderByDescending(x => x.Score)
			.ThenBy(x => x.Deck.Owner.Name)
			.ThenBy(x => x.Deck.Name ?? x.Deck.Commander)
			.Select(x => x.Deck)
			.ToList();

		stateChangedCallback();
		return;

		(Deck Deck, int Score) ApplySearch(Deck deck)
		{
			int[] scores =
			[
				CalculateScore(deck.Commander),
				CalculateScore(deck.Partner),
				CalculateScore(deck.Name),
			];

			return (deck, scores.Max());
		}

		int CalculateScore(string? value)
		{
			if (value is null)
				return 0;

			return Fuzz.WeightedRatio(_searchTerm, value, PreprocessMode.Full);
		}
	}

	public IEnumerator<Deck> GetEnumerator()
	{
		if (_visibleDecks is null)
			yield break;

		foreach (var deck in _visibleDecks)
			yield return deck;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}