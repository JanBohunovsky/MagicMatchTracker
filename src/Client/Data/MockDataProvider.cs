
using System.Net.Http.Json;

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

		var httpClient = httpClientFactory.CreateClient(string.Empty);
		Players = await httpClient.GetFromJsonAsync<List<Player>>("mock_data/players.json") ?? [];
		logger.LogInformation("Loaded {Count} players", Players.Count);

		Decks = await httpClient.GetFromJsonAsync<List<Deck>>("mock_data/decks.json") ?? [];
		logger.LogInformation("Loaded {Count} decks", Decks.Count);

		Sessions = await httpClient.GetFromJsonAsync<List<Session>>("mock_data/sessions.json") ?? [];
		logger.LogInformation("Loaded {Count} sessions", Sessions.Count);

		foreach (var session in Sessions)
		{
			foreach (var sessionPlayer in session.Players)
			{
				sessionPlayer.Player = Players.FindBySlug(sessionPlayer.PlayerSlug, p => p.Slug)
					?? throw new InvalidOperationException($"Player '{sessionPlayer.PlayerSlug}' not found");
			}

			foreach (var match in session.Matches)
			{
				match.Session = session;

				foreach (var participation in match.Participations)
				{
					participation.Match = match;
					participation.Player = session.Players.FindBySlug(participation.PlayerSlug, p => p.PlayerSlug)?.Player
						?? throw new InvalidOperationException($"Player '{participation.PlayerSlug}' is not in the session '{session.Slug}'");

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
				}
			}
		}

		foreach (var player in Players)
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

		_isInitialized = true;
		logger.LogInformation("Initialized mock data provider");
	}
}