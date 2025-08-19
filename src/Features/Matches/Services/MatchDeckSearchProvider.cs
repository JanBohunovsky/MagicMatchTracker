using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDeckSearchProvider(Database database) : ISearchProvider<Deck>
{
	private static readonly SearchFilter FilterAll = new("All");
	private static readonly SearchFilter FilterOwned = new("Owned", IsDefault: true);
	private static readonly SearchFilter FilterBorrow = new("Borrow");

	public IReadOnlyList<SearchFilter> Filters { get; } = [FilterAll, FilterOwned, FilterBorrow];

	public MatchParticipationEditModel? MatchParticipation { get; set; }

	public async Task<IReadOnlyList<Deck>> SearchAsync(string searchTerm, SearchFilter? filter, CancellationToken cancellationToken = default)
	{
		if (MatchParticipation is null)
			return [];

		filter ??= FilterOwned;

		// Prefer decks owned by the selected player
		IQueryable<Deck>? query = database.Decks.OrderByDescending(d => d.Owner == MatchParticipation.Player)
			.ThenBy(d => d.Owner.Name)
			.ThenBy(d => d.Name ?? d.Commander);

		query = filter.Name switch
		{
			"All" => GetAllDecks(query),
			"Owned" => GetOwnedDecks(query),
			"Borrow" => GetBorrowableDecks(query),
			_ => throw new ArgumentException($"Unexpected search filter: {filter}", nameof(filter)),
		};

		if (query is null)
			return [];

		if (searchTerm.IsNotEmpty())
		{
			var pattern = searchTerm.Split(' ')
				.Select(s => $"%{s}%")
				.JoinToString(' ');

			query = query.Where(d => EF.Functions.ILike(d.Name!, pattern)
					|| EF.Functions.ILike(d.Commander, pattern)
					|| EF.Functions.ILike(d.Partner!, pattern))
				// Prefer matches that start with the search term
				.OrderByDescending(d => EF.Functions.ILike(d.Name!, $"{searchTerm}%")
					|| EF.Functions.ILike(d.Commander, $"{searchTerm}%")
					|| EF.Functions.ILike(d.Partner!, $"{searchTerm}%"))
				.ThenByDescending(d => d.Owner == MatchParticipation.Player)
				.ThenBy(d => d.Owner.Name)
				.ThenBy(d => d.Name ?? d.Commander);
		}
		else if (MatchParticipation.Deck is not null)
		{
			// Keep the original deck selected when there's no search term
			query = query.OrderByDescending(d => d == MatchParticipation.Deck)
				.ThenByDescending(d => d.Owner == MatchParticipation.Player)
				.ThenBy(d => d.Owner.Name)
				.ThenBy(d => d.Name ?? d.Commander);
		}

		var existingDecks = MatchParticipation.Match.Participations
			.Where(mp => mp != MatchParticipation)
			.Choose(mp => mp.Deck)
			.ToList();
		query = query.Where(d => !existingDecks.Contains(d));

		return await query.ToListAsync(cancellationToken);
	}

	private IQueryable<Deck> GetAllDecks(IQueryable<Deck> decks)
	{
		return decks;
	}

	private IQueryable<Deck>? GetOwnedDecks(IQueryable<Deck> decks)
	{
		if (MatchParticipation?.Player is null)
			return null;

		return decks.Where(d => d.Owner == MatchParticipation.Player);
	}

	private IQueryable<Deck>? GetBorrowableDecks(IQueryable<Deck> decks)
	{
		if (MatchParticipation is null)
			return null;

		var otherPlayers = MatchParticipation.Match.Participations
			.Where(mp => mp != MatchParticipation)
			.Choose(mp => mp.Player)
			.ToList();

		if (otherPlayers.Count == 0)
			return null;

		return decks.Where(d => otherPlayers.Contains(d.Owner));
	}
}