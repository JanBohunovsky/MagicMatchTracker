using System.Data;
using Microsoft.Data.Sqlite;

namespace MagicMatchTracker.Data.Seed.Extensions;

public static class SqliteDataReaderExtensions
{
	extension(SqliteDataReader reader)
	{
		public async Task<T?> GetFieldValueOrDefaultAsync<T>(string columnName, T? defaultValue = default, CancellationToken cancellationToken = default)
		{
			if (await reader.IsDBNullAsync(columnName, cancellationToken))
				return defaultValue;

			return await reader.GetFieldValueAsync<T>(columnName, cancellationToken);
		}
	}
}