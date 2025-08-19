using MagicMatchTracker.Features.Matches.Models;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchPlayerSearchProvider(Database database) : ISearchProvider<Player>
{
	public IReadOnlyList<SearchFilter> Filters { get; } = [];

	public MatchParticipationEditModel? MatchParticipation { get; set; }

	public async Task<IReadOnlyList<Player>> SearchAsync(string searchTerm, SearchFilter? filter, CancellationToken cancellationToken = default)
	{
		if (MatchParticipation is null)
			return [];

		IQueryable<Player> query = database.Players.OrderBy(p => p.Name);

		if (searchTerm.IsNotEmpty())
		{
			// TODO: Turn into an extension method
			var pattern = searchTerm.Split(' ')
				.Select(s => $"%{s}%")
				.JoinToString(' ');

			query = query.Where(p => EF.Functions.ILike(p.Name, pattern))
				// Prefer matches that start with the search term
				.OrderByDescending(p => EF.Functions.ILike(p.Name, $"{searchTerm}%"))
				.ThenBy(p => p.Name);
		}
		else if (MatchParticipation.Player is not null)
		{
			// Keep the original player selected when there's no search term
			query = query.OrderByDescending(p => p == MatchParticipation.Player)
				.ThenBy(p => p.Name);
		}

		var existingPlayers = MatchParticipation.Match.Participations
			.Where(mp => mp != MatchParticipation)
			.Choose(mp => mp.Player)
			.ToList();
		query = query.Where(p => !existingPlayers.Contains(p));

		return await query.ToListAsync(cancellationToken);
	}
}