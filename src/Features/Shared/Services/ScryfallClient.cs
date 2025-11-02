namespace MagicMatchTracker.Features.Shared.Services;

public sealed class ScryfallClient(HttpClient httpClient)
{
	public async Task<string?> GetCardArtUriAsync(string cardName, CancellationToken cancellationToken = default)
	{
		var requestUri = $"cards/named?format=image&version=art_crop&fuzzy={Uri.EscapeDataString(cardName)}";
		var response = await httpClient.GetAsync(requestUri, cancellationToken);

		if (!response.IsSuccessStatusCode)
			return null;

		// Using `format=image` returns a 302 redirect to the actual image.
		return response.RequestMessage?.RequestUri?.ToString();
	}
}