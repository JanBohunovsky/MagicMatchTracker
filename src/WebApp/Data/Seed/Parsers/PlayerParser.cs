using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace MagicMatchTracker.Data.Seed.Parsers;

public sealed partial class PlayerParser
{
	public async Task<Player> ParseAsync(SqliteDataReader reader, CancellationToken cancellationToken = default)
	{
		var rawName = await reader.GetFieldValueAsync<string>("Name", cancellationToken);
		var match = NameRegex().Match(rawName);
		if (!match.Success)
			throw new FormatException($"Failed to parse player name: {rawName}");

		var name = match.Groups["name"].Value;
		var alias = match.Groups["alias"].Value.TrimToNull();

		return new Player
		{
			Name = name,
			Alias = alias,
		};
	}

	[GeneratedRegex(@"^(?<name>.+?)(?: \((?<alias>.+?)\))?$")]
	private partial Regex NameRegex();
}