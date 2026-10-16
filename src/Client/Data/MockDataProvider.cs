using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MagicMatchTracker.Client.Data;

public sealed class MockDataProvider(ILogger<MockDataProvider> logger, IHttpClientFactory httpClientFactory)
{
	private bool _isInitialized;

	public List<Player> Players { get; private set; } = [];
	public List<Deck> Decks { get; private set; } = [];
	public List<Session> Sessions { get; private set; } = [];

	public async Task InitializeAsync()
	{
		if (_isInitialized)
			return;

		var serializerOptions = new JsonSerializerOptions();
		serializerOptions.Converters.Add(new JsonStringEnumConverter());

		var httpClient = httpClientFactory.CreateClient(string.Empty);
		Players = await httpClient.GetFromJsonAsync<List<Player>>("mock_data/players.json", serializerOptions) ?? [];
		logger.LogInformation("Loaded {Count} players", Players.Count);

		Decks = await httpClient.GetFromJsonAsync<List<Deck>>("mock_data/decks.json", serializerOptions) ?? [];
		logger.LogInformation("Loaded {Count} decks", Decks.Count);

		Sessions = await httpClient.GetFromJsonAsync<List<Session>>("mock_data/sessions.json", serializerOptions) ?? [];
		logger.LogInformation("Loaded {Count} sessions", Sessions.Count);

		FillModelDetails();

		_isInitialized = true;
		logger.LogInformation("Initialized mock data provider");
	}

	private void FillModelDetails()
	{
		foreach (var session in Sessions)
		{
			FillSessionDetails(session);
		}

		foreach (var player in Players)
		{
			FillPlayerDetails(player);
		}
	}

	private void FillSessionDetails(Session session)
	{
		foreach (var sessionPlayer in session.Players)
		{
			sessionPlayer.Player = Players.FindBySlug(sessionPlayer.PlayerSlug, p => p.Slug)
				?? throw new InvalidOperationException($"Player '{sessionPlayer.PlayerSlug}' not found");
		}

		foreach (var match in session.Matches)
		{
			match.Session = session;
			FillMatchDetails(match);
		}
	}

	private void FillMatchDetails(Match match)
	{
		foreach (var participation in match.Participations)
		{
			participation.Match = match;
			FillMatchParticipationDetails(participation);
		}
	}

	private void FillMatchParticipationDetails(MatchParticipation participation)
	{
		var session = participation.Match.Session;

		participation.Player = FindPlayerInSession(participation.PlayerSlug);

		var ownerSlug = participation.PlayerSlug;
		var deckSlug = participation.DeckSlug;
		if (participation.DeckSlug.Contains('/'))
		{
			var split = participation.DeckSlug.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			ownerSlug = split[0];
			deckSlug = split[1];
		}

		participation.Deck = Decks.FindAllBySlug(ownerSlug, d => d.OwnerSlug)
				.FindBySlug(deckSlug, d => d.Slug)
			?? throw new InvalidOperationException($"Deck '{participation.DeckSlug}' not found");

		if (participation.EliminatedByPlayerSlug is not null)
		{
			participation.EliminatedBy = FindPlayerInSession(participation.EliminatedByPlayerSlug);
		}

		return;

		Player FindPlayerInSession(string playerSlug)
		{
			var sessionPlayer = session.Players.FindBySlug(playerSlug, p => p.PlayerSlug);

			if (sessionPlayer?.Player is null)
				throw new InvalidOperationException($"Player '{playerSlug}' is not in the session '{session.Slug}'");

			return sessionPlayer.Player;
		}
	}

	private void FillPlayerDetails(Player player)
	{
		var ownedDecks = Decks.FindAllBySlug(player.Slug, d => d.OwnerSlug).ToList();
		player.Decks = ownedDecks.Count(d => !d.IsArchived);
		player.Matches = ownedDecks.Sum(d => d.Matches);
		player.Wins = ownedDecks.Sum(d => d.Wins);

		foreach (var deck in ownedDecks)
		{
			deck.Owner = player;
		}
	}
}