using MagicMatchTracker.Features.Matches.Dialogs.PlayersEdit;
using MagicMatchTracker.Features.Matches.Events;
using MagicMatchTracker.Features.Matches.Extensions;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MagicMatchTracker.Features.Matches.Pages.Listing;

public sealed class MatchListingState : StateBase, IDisposable
{
	private readonly Database _database;
	private readonly MatchPlayersEditDialogState _playersEditDialogState;
	private readonly NavigationManager _navigationManager;
	private readonly IMessageHub _messageHub;
	private readonly MatchListingOptions _options;
	private readonly Guid[] _eventSubscriptions;

	private List<Match>? _matches;
	private DateOnly? _nextPageDate;

	public IReadOnlyList<Match>? Matches => _matches;

	public bool HasMoreMatches => _nextPageDate is not null;

	public MatchListingState(Database database,
		MatchPlayersEditDialogState playersEditDialogState,
		NavigationManager navigationManager,
		IMessageHub messageHub,
		IOptions<MatchListingOptions> options)
	{
		_database = database;
		_playersEditDialogState = playersEditDialogState;
		_navigationManager = navigationManager;
		_messageHub = messageHub;
		_options = options.Value;

		_eventSubscriptions =
		[
			messageHub.Subscribe<MatchCreatedEvent>(OnMatchCreated),
			messageHub.Subscribe<MatchDeletedEvent>(OnMatchDeleted),
		];
	}

	public async Task LoadMatchesAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is not null)
			return;

		_matches = await LoadMoreMatchesCoreAsync(cancellationToken);
	}

	public async Task LoadMoreMatchesAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is null || _nextPageDate is null)
			return;

		IsBusy = true;

		var moreMatches = await LoadMoreMatchesCoreAsync(cancellationToken);
		_matches.AddRange(moreMatches);

		IsBusy = false;
	}

	private async Task<List<Match>> LoadMoreMatchesCoreAsync(CancellationToken cancellationToken = default)
	{
		var datesQuery = _database.Matches
			.QueryMatchDates();

		if (_nextPageDate is not null)
		{
			datesQuery = datesQuery.Where(d => d <= _nextPageDate);
		}

		datesQuery = datesQuery.Take(_options.DaysPerPage + 1);

		var dates = await datesQuery.ToListAsync(cancellationToken);

		_nextPageDate = dates.Count > _options.DaysPerPage
			? dates[^1]
			: null;

		if (_nextPageDate is not null)
		{
			dates.Remove(_nextPageDate.Value);
		}

		var today = DateOnly.Today;
		return await _database.Matches
			.Include(m => m.Participations)
			.Where(m => dates.Contains(m.TimeStarted != null ? DateOnly.FromDateTime(m.TimeStarted.Value.DateTime) : today))
			.ToListAsync(cancellationToken);
	}

	public async Task AddNewMatchAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is null)
			return;

		IsBusy = true;

		var match = new Match
		{
			MatchNumber = await _database.Matches.GetNewMatchNumberAsync(DateOnly.Today, cancellationToken),
		};

		IsBusy = false;

		var success = await _playersEditDialogState.ShowDialogAsync(match, cancellationToken);
		if (!success)
			return;

		_navigationManager.NavigateTo($"/matches/{match.Id}");
	}

	public void Dispose()
	{
		foreach (var subscriptionToken in _eventSubscriptions)
		{
			_messageHub.Unsubscribe(subscriptionToken);
		}
	}

	private void OnMatchCreated(MatchCreatedEvent eventData)
	{
		_matches?.Add(eventData.Match);
	}

	private void OnMatchDeleted(MatchDeletedEvent eventData)
	{
		_matches?.Remove(eventData.Match);
	}
}