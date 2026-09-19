using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MagicMatchTracker.Features.Shared.Services.Scryfall.Models;

namespace MagicMatchTracker.Features.Shared.Services.Scryfall;

public sealed partial class ScryfallClient(HttpClient httpClient, ILogger<ScryfallClient> logger)
{
	private readonly JsonSerializerOptions _options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		WriteIndented = false,
	};

	public async Task<string?> GetCardArtUriAsync(string cardName, CancellationToken cancellationToken = default)
	{
		var requestUri = $"cards/named?format=image&version=art_crop&fuzzy={Uri.EscapeDataString(cardName)}";
		var response = await httpClient.GetAsync(requestUri, cancellationToken);

		if (!response.IsSuccessStatusCode)
			return null;

		// Using `format=image` returns a 302 redirect to the actual image.
		return response.RequestMessage?.RequestUri?.ToString();
	}

	public async IAsyncEnumerable<(string CardName, string ImageUri)> GetBulkCardArtUrisAsync(
		IEnumerable<string> cardNames,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		const int maxCardNamesPerRequest = 75;
		const int delayBetweenRequestsMs = 60;

		var firstRequest = true;
		foreach (var cardNamesBatch in cardNames.Chunk(maxCardNamesPerRequest))
		{
			if (!firstRequest)
				await Task.Delay(delayBetweenRequestsMs, cancellationToken);
			firstRequest = false;

			var batchResponse = await BatchedRequest(cardNamesBatch);
			foreach (var (cardName, imageUri) in batchResponse)
				yield return (cardName, imageUri);
		}

		yield break;

		async Task<IEnumerable<(string CardName, string ImageUri)>> BatchedRequest(IEnumerable<string> cardNamesBatch)
		{
			var identifiers = cardNamesBatch.Select(n => new ScryfallCardIdentifier(n))
				.ToList();
			var requestBody = new CardCollectionRequest(identifiers);
			var response = await httpClient.PostAsJsonAsync("cards/collection", requestBody, _options, cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);
				Log.UnsuccessfulStatusCode(logger, response.StatusCode, rawResponse);

				return [];
			}

			var responseBody = await response.Content.ReadFromJsonAsync<CardCollectionResponse>(_options, cancellationToken);
			if (responseBody is null)
			{
				var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);
				Log.DeserializationError(logger, rawResponse);

				return [];
			}

			if (responseBody.NotFound?.Count > 0)
			{
				var unknownCardNames = responseBody.NotFound
					.Select(c => c.Name)
					.JoinToString(", ");

				Log.UnknownCardNames(logger, unknownCardNames);
			}

			return responseBody.Data
				.Where(c => c.HasImage)
				.Select(c => (c.Name, c.ImageUris!.ArtCrop));
		}
	}

	private record CardCollectionRequest(IReadOnlyList<ScryfallCardIdentifier> Identifiers);
	private record CardCollectionResponse(IReadOnlyList<ScryfallCard> Data, IReadOnlyList<ScryfallCardIdentifier>? NotFound);

	private static partial class Log
	{
		[LoggerMessage(LogLevel.Error, "Failed to get bulk card art URIs: StatusCode={StatusCode}, Response={Response}")]
		internal static partial void UnsuccessfulStatusCode(ILogger<ScryfallClient> logger, HttpStatusCode statusCode, string response);

		[LoggerMessage(LogLevel.Error, "Failed to deserialize bulk card art URIs response: Response={Response}")]
		internal static partial void DeserializationError(ILogger<ScryfallClient> logger, string response);

		[LoggerMessage(LogLevel.Warning, "Unknown card names: {CardNames}")]
		internal static partial void UnknownCardNames(ILogger<ScryfallClient> logger, string cardNames);
	}
}