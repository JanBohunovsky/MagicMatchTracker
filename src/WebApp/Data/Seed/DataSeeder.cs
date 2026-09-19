using System.Diagnostics;
using MagicMatchTracker.Features.Shared;
using MagicMatchTracker.Features.Shared.Services;
using MagicMatchTracker.Features.Shared.Services.Scryfall;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Data.Seed;

public sealed class DataSeeder(
	ILogger<DataSeeder> logger,
	GristImporter gristImporter,
	ScryfallClient scryfallClient,
	ImageCachingService imageCachingService)
{
	public async Task SeedAsync(Database database, CancellationToken cancellationToken = default)
	{
		var hasData = await database.Players.AnyAsync(cancellationToken);
		if (hasData)
			return;

		var timestamp = Stopwatch.GetTimestamp();

		var importedData = await gristImporter.ImportAsync(cancellationToken);
		if (importedData is null)
			return;

		await DownloadDeckCoversAsync(importedData, cancellationToken);

		database.Players.AddRange(importedData.Players);
		database.Decks.AddRange(importedData.Decks);
		database.Matches.AddRange(importedData.Matches);
		database.MatchParticipations.AddRange(importedData.MatchParticipations);
		await database.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Seeding completed in {ElapsedTime}", Stopwatch.GetElapsedTime(timestamp));
	}

	private async Task DownloadDeckCoversAsync(IImportedData data, CancellationToken cancellationToken = default)
	{
		var commanders = data.Decks
			.Select(d => d.Commander)
			.Distinct();

		var images = await scryfallClient.GetBulkCardArtUrisAsync(commanders, cancellationToken)
			.Select(async (x, ct) =>
			{
				var localImageUri = await imageCachingService.CacheImageAsync(CacheConstants.DeckCoversFolder, x.ImageUri, ct);
				return x with { ImageUri = localImageUri };
			})
			.ToDictionaryAsync(x => x.CardName, x => x.ImageUri, cancellationToken: cancellationToken);

		foreach (var deck in data.Decks)
		{
			deck.ImageUri = images.GetValueOrDefault(deck.Commander);
		}
	}
}