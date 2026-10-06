
using System.Net.Http.Json;

namespace MagicMatchTracker.Client.Data;

public sealed class MockDataProvider(ILogger<MockDataProvider> logger, IHttpClientFactory httpClientFactory)
{
	private bool _isInitialized;

	public List<Player> Players { get; private set; } = [];
	public List<Deck> Decks { get; private set; } = [];

	public async Task InitializeAsync()
	{
		if (_isInitialized)
			return;

		var httpClient = httpClientFactory.CreateClient(string.Empty);
		Players = await httpClient.GetFromJsonAsync<List<Player>>("mock_data/players.json") ?? [];
		logger.LogInformation("Loaded {Count} players", Players.Count);

		Decks = await httpClient.GetFromJsonAsync<List<Deck>>("mock_data/decks.json") ?? [];
		logger.LogInformation("Loaded {Count} decks", Decks.Count);

		foreach (var player in Players)
		{
			var ownedDecks = Decks.FindAllBySlug(player.Slug, d => d.OwnerSlug).ToList();
			player.Decks = ownedDecks.Count(d => !d.IsArchived);
			player.Matches = ownedDecks.Sum(d => d.Matches);
			player.Wins = ownedDecks.Sum(d => d.Wins);
		}

		_isInitialized = true;
		logger.LogInformation("Initialized mock data provider");
	}
}