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
	public AppSettings Settings { get; private set; } = new();

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
		foreach (var sessionPlayer in session.Seats)
		{
			sessionPlayer.Player = Players.FindById(sessionPlayer.PlayerId, p => p.Id)
				?? throw new InvalidOperationException($"Player '{sessionPlayer.PlayerId}' not found");
		}

		foreach (var match in session.Matches)
		{
			match.Session = session;
			FillMatchDetails(match);
		}
	}

	private void FillMatchDetails(Match match)
	{
		foreach (var player in match.Players)
		{
			player.Match = match;
			FillMatchPlayerDetails(player);
		}
	}

	private void FillMatchPlayerDetails(MatchPlayer matchPlayer)
	{
		var session = matchPlayer.Match.Session;

		// Find player
		matchPlayer.Player = FindPlayerInSession(matchPlayer.PlayerId);

		// Find deck
		var ownerId = matchPlayer.PlayerId;
		var deckId = matchPlayer.DeckId;
		if (matchPlayer.DeckId.Contains('/'))
		{
			var split = matchPlayer.DeckId.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			ownerId = split[0];
			deckId = split[1];
		}

		matchPlayer.Deck = Decks.FindAllById(ownerId, d => d.OwnerId)
				.FindById(deckId, d => d.Id)
			?? throw new InvalidOperationException($"Deck '{matchPlayer.DeckId}' not found");

		matchPlayer.Elimination?.Killer = FindPlayerInSession(matchPlayer.Elimination.KillerId);

		return;

		Player FindPlayerInSession(string playerId)
		{
			var sessionPlayer = session.Seats.FindById(playerId, p => p.PlayerId);

			if (sessionPlayer?.Player is null)
				throw new InvalidOperationException($"Player '{playerId}' is not in the session '{session.Id}'");

			return sessionPlayer.Player;
		}
	}

	private void FillPlayerDetails(Player player)
	{
		var ownedDecks = Decks.FindAllById(player.Id, d => d.OwnerId).ToList();
		player.Decks = ownedDecks.Count(d => !d.IsRetired);
		player.Matches = ownedDecks.Sum(d => d.Matches);
		player.Wins = ownedDecks.Sum(d => d.Wins);

		foreach (var deck in ownedDecks)
		{
			deck.Owner = player;
		}
	}
}