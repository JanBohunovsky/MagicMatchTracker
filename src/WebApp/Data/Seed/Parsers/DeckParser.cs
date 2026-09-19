using System.Data;
using System.Text.RegularExpressions;
using MagicMatchTracker.Data.Seed.Extensions;
using Microsoft.Data.Sqlite;

namespace MagicMatchTracker.Data.Seed.Parsers;

public sealed partial class DeckParser(IReadOnlyGristData data)
{
	private readonly ColoursParser _coloursParser = new();

	public async Task<Deck> ParseAsync(SqliteDataReader reader, CancellationToken cancellationToken = default)
	{
		var rawCommander = await reader.GetFieldValueAsync<string>("Commander", cancellationToken);
		var rawColours = await reader.GetFieldValueOrDefaultAsync<string>("Colours", cancellationToken: cancellationToken);
		var rawUrl = await reader.GetFieldValueAsync<string>("URL", cancellationToken);
		var ownerId = await reader.GetFieldValueAsync<int>("Author", cancellationToken);
		var isArchived = await reader.GetFieldValueAsync<bool>("Archived", cancellationToken);

		var matchCommander = DeckCommanderRegex().Match(rawCommander);
		if (!matchCommander.Success)
			throw new FormatException($"Failed to parse deck commander: {rawCommander}");

		var commander = matchCommander.Groups["commander"].Value;
		var partner = matchCommander.Groups["partner"].Value.TrimToNull();

		var colours = await _coloursParser.ParseAsync(rawColours, cancellationToken);

		var matchUrl = DeckUrlRegex().Match(rawUrl);
		var url = matchUrl.Groups["url"].Value.TrimToNull();
		var name = matchUrl.Groups["name"].Value.TrimToNull();

		return new Deck
		{
			Owner = data.Players[ownerId],
			Name = name,
			Commander = commander,
			Partner = partner,
			ColourIdentity = colours,
			DeckUri = url,
			IsArchived = isArchived,
		};
	}

	[GeneratedRegex(@"^(?<commander>.+?)(?: \+ (?<partner>.+?))?$")]
	private partial Regex DeckCommanderRegex();

	[GeneratedRegex(@"^(?:\[(?<name>.+?)\] )?(?<url>.+?)$")]
	private partial Regex DeckUrlRegex();
}