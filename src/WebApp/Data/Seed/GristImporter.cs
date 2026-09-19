using System.Data;
using MagicMatchTracker.Data.Seed.Parsers;
using Microsoft.Data.Sqlite;

namespace MagicMatchTracker.Data.Seed;

public sealed class GristImporter(IConfiguration configuration)
{
	private SqliteConnection _connection = null!;
	private GristData _data = null!;

	public async Task<IImportedData?> ImportAsync(CancellationToken cancellationToken = default)
	{
		var connectionString = configuration.GetConnectionString("Grist");
		if (connectionString.IsEmpty())
			return null;

		await using (_connection = new SqliteConnection(connectionString))
		{
			await _connection.OpenAsync(cancellationToken);

			_data = new GristData();

			await ImportPlayersAsync(cancellationToken);
			await ImportDecksAsync(cancellationToken);
			await ImportMatchesAsync(cancellationToken);
			await ImportMatchParticipationsAsync(cancellationToken);

			FixMatchParticipationWinners();

			await _connection.CloseAsync();
			return _data;
		}
	}

	private async Task ImportPlayersAsync(CancellationToken cancellationToken)
	{
		var playerParser = new PlayerParser();

		await using var reader = await LoadTableAsync("Players", cancellationToken);
		while (await reader.ReadAsync(cancellationToken))
		{
			var id = await reader.GetFieldValueAsync<int>("id", cancellationToken);
			var player = await playerParser.ParseAsync(reader, cancellationToken);

			_data.Players.Add(id, player);
		}
	}

	private async Task ImportDecksAsync(CancellationToken cancellationToken)
	{
		var deckParser = new DeckParser(_data);

		await using var reader = await LoadTableAsync("Decks", cancellationToken);
		while (await reader.ReadAsync(cancellationToken))
		{
			var id = await reader.GetFieldValueAsync<int>("id", cancellationToken);
			var deck = await deckParser.ParseAsync(reader, cancellationToken);

			deck.Owner.Decks.Add(deck);
			_data.Decks.Add(id, deck);
		}
	}

	private async Task ImportMatchesAsync(CancellationToken cancellationToken)
	{
		var matchParser = new MatchParser();

		await using var reader = await LoadTableAsync("Matches", cancellationToken);
		while (await reader.ReadAsync(cancellationToken))
		{
			var id = await reader.GetFieldValueAsync<int>("id", cancellationToken);
			var match = await matchParser.ParseAsync(reader, cancellationToken);

			_data.Matches.Add(id, match);
		}
	}

	private async Task ImportMatchParticipationsAsync(CancellationToken cancellationToken)
	{
		var matchParticipationParser = new MatchParticipationParser(_data);

		await using var reader = await LoadTableAsync("Match_Participants", cancellationToken);
		while (await reader.ReadAsync(cancellationToken))
		{
			var id = await reader.GetFieldValueAsync<int>("id", cancellationToken);
			var matchParticipation = await matchParticipationParser.ParseAsync(reader, cancellationToken);

			matchParticipation.Match.Participations.Add(matchParticipation);
			_data.MatchParticipations.Add(id, matchParticipation);
		}
	}

	private void FixMatchParticipationWinners()
	{
		foreach (var participation in _data.MatchParticipations.Values.Where(mp => mp.EndState?.IsWinner is true))
		{
			participation.EndState!.Turn = participation.Match.GetTotalTurns();
			if (participation.Match.IsLive)
				participation.EndState.Time = participation.Match.TimeEnded;
		}
	}

	private async Task<SqliteDataReader> LoadTableAsync(string tableName, CancellationToken cancellationToken)
	{
		var command = _connection.CreateCommand();
		command.CommandText = $"SELECT * FROM `{tableName}`";

		return await command.ExecuteReaderAsync(cancellationToken);
	}

	private class GristData : IReadOnlyGristData, IImportedData
	{
		public Dictionary<int, Player> Players { get; } = new();
		public Dictionary<int, Deck> Decks { get; } = new();
		public Dictionary<int, Match> Matches { get; } = new();
		public Dictionary<int, MatchParticipation> MatchParticipations { get; } = new();

		IReadOnlyDictionary<int, Player> IReadOnlyGristData.Players => Players;
		IReadOnlyDictionary<int, Deck> IReadOnlyGristData.Decks => Decks;
		IReadOnlyDictionary<int, Match> IReadOnlyGristData.Matches => Matches;
		IReadOnlyDictionary<int, MatchParticipation> IReadOnlyGristData.MatchParticipations => MatchParticipations;

		IReadOnlyCollection<Player> IImportedData.Players => Players.Values;
		IReadOnlyCollection<Deck> IImportedData.Decks => Decks.Values;
		IReadOnlyCollection<Match> IImportedData.Matches => Matches.Values;
		IReadOnlyCollection<MatchParticipation> IImportedData.MatchParticipations => MatchParticipations.Values;
	}
}