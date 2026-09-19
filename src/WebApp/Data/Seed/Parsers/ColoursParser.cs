using System.Text;
using System.Text.Json;

namespace MagicMatchTracker.Data.Seed.Parsers;

public sealed class ColoursParser
{
	public async Task<Colours> ParseAsync(string? rawColours, CancellationToken cancellationToken = default)
	{
		if (rawColours.IsEmpty())
			return Colours.Colourless;

		var result = Colours.Colourless;

		var stringStream = new MemoryStream(Encoding.UTF8.GetBytes(rawColours));
		await foreach (var rawColour in JsonSerializer.DeserializeAsyncEnumerable<string>(stringStream, cancellationToken: cancellationToken))
		{
			if (!Enum.TryParse(rawColour, out Colours colour))
				throw new FormatException($"Failed to parse colours: {rawColours}");

			result |= colour;
		}

		return result;
	}
}