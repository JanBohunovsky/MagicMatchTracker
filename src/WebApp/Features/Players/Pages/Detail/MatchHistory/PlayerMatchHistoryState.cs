using MagicMatchTracker.Features.Players.Dialogs.Edit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MagicMatchTracker.Features.Players.Pages.Detail.MatchHistory;

public sealed class PlayerMatchHistoryState : PlayerDetailStateBase
{
	private readonly IOptions<PlayerMatchHistoryOptions> _options;

	private List<MatchParticipation> _finishedMatchParticipations = [];
	private Match? _nextMatch;

	// ReSharper disable once ConvertToPrimaryConstructor - Issue with capturing `database` twice
	public PlayerMatchHistoryState(
		Database database,
		IOptions<PlayerMatchHistoryOptions> options,
		PlayerEditDialogState playerEditDialogState)
		: base(database, playerEditDialogState)
	{
		_options = options;
	}

	public IReadOnlyList<MatchParticipation> FinishedMatchParticipations => _finishedMatchParticipations;
	public bool HasMoreMatches => _nextMatch is not null;

	protected override async Task<Player?> LoadPlayerCoreAsync(Guid id, CancellationToken cancellationToken)
	{
		var player = await Database.Players.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		_nextMatch = null;
		var matchParticipations = await LoadMoreMatchesCoreAsync(player, cancellationToken);
		_finishedMatchParticipations = matchParticipations.ToList();

		return player;
	}

	public async Task LoadMoreMatchesAsync(CancellationToken cancellationToken = default)
	{
		if (Player is null || _nextMatch is null)
			return;

		IsBusy = true;

		var moreMatchParticipations = await LoadMoreMatchesCoreAsync(Player, cancellationToken);
		_finishedMatchParticipations.AddRange(moreMatchParticipations);

		IsBusy = false;
	}

	private async Task<IEnumerable<MatchParticipation>> LoadMoreMatchesCoreAsync(Player? player, CancellationToken cancellationToken = default)
	{
		if (player is null)
		{
			_nextMatch = null;
			return [];
		}

		var query = Database.Matches
			.Include(m => m.Participations)
			.Where(m => m.TimeStarted != null && m.TimeEnded != null && m.Participations.Any(mp => mp.Player == player));

		if (_nextMatch is not null)
		{
			var nextMatchDate = _nextMatch.GetEffectiveDate();
			var nextMatchNumber = _nextMatch.MatchNumber;
			query = query.Select(m => new
				{
					Match = m,
					EffectiveDate = DateOnly.FromDateTime(m.TimeEnded!.Value.DateTime),
				})
				.Where(x => x.EffectiveDate < nextMatchDate || (x.EffectiveDate == nextMatchDate && x.Match.MatchNumber <= nextMatchNumber))
				.Select(x => x.Match);
		}

		var matchesPerPage = _options.Value.MatchesPerPage;
		query = query.OrderByDescending(m => DateOnly.FromDateTime(m.TimeEnded!.Value.DateTime))
			.ThenByDescending(m => m.MatchNumber)
			.Take(matchesPerPage + 1);

		var matches = await query.ToListAsync(cancellationToken);

		_nextMatch = matches.Count > matchesPerPage
			? matches[^1]
			: null;

		if (_nextMatch is not null)
		{
			matches.Remove(_nextMatch);
		}

		return matches.Select(m => m.Participations.First(mp => mp.Player == player));
	}
}