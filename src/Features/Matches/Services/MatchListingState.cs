using MagicMatchTracker.Features.Matches.Events;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Features.Matches.Services;

public sealed class MatchListingState : StateBase, IDisposable
{
	private readonly Database _database;
	private readonly MatchPlayersEditDialogState _playersEditDialogState;
	private readonly MatchNumberingHelper _matchNumberingHelper;
	private readonly NavigationManager _navigationManager;
	private readonly IMessageHub _messageHub;
	private readonly Guid[] _eventSubscriptions;

	private List<Match>? _matches;

	public IReadOnlyList<Match>? Matches => _matches;

	public MatchListingState(Database database,
		MatchPlayersEditDialogState playersEditDialogState,
		MatchNumberingHelper matchNumberingHelper,
		NavigationManager navigationManager,
		IMessageHub messageHub)
	{
		_database = database;
		_playersEditDialogState = playersEditDialogState;
		_matchNumberingHelper = matchNumberingHelper;
		_navigationManager = navigationManager;
		_messageHub = messageHub;

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

		var today = DateOnly.Today;
		_matches = await _database.Matches
			.Include(m => m.Participations)
			.OrderByDescending(m => m.TimeStarted != null ? DateOnly.FromDateTime(m.TimeStarted.Value.Date) : today)
			.ThenByDescending(m => m.MatchNumber)
			.ToListAsync(cancellationToken);
	}

	public async Task AddNewMatchAsync(CancellationToken cancellationToken = default)
	{
		if (_matches is null)
			return;

		IsBusy = true;

		var match = new Match
		{
			MatchNumber = await _matchNumberingHelper.GetNextMatchNumberAsync(DateOnly.Today, cancellationToken),
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
		_matches?.Insert(0, eventData.Match);
	}

	private void OnMatchDeleted(MatchDeletedEvent eventData)
	{
		_matches?.Remove(eventData.Match);
	}
}