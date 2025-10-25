using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchDeckSearchProvider(Database database)
{
	public async Task<IReadOnlyList<Deck>> SearchAsync(string searchTerm, Player? owner, CancellationToken cancellationToken = default)
	{
		searchTerm = searchTerm.Trim();

		IQueryable<Deck> query = database.Decks
			.Where(d => !d.IsArchived)
			.OrderBy(d => d.Owner.Name)
			.ThenBy(d => d.Name ?? d.Commander);

		if (owner is not null)
		{
			query = query.Where(d => d.Owner.Id == owner.Id);
		}

		if (searchTerm.IsEmpty())
			return await query.ToListAsync(cancellationToken);

		var pattern = searchTerm.Split(' ')
			.Select(s => $"%{s}%")
			.JoinToString(' ');

		query = query.Where(d => EF.Functions.ILike(d.Name!, pattern)
			|| EF.Functions.ILike(d.Commander, pattern)
			|| EF.Functions.ILike(d.Partner!, pattern));

		// TODO: Hide decks which are already selected by other players
		return await query.ToListAsync(cancellationToken);
	}
}