namespace MagicMatchTracker.Infrastructure.Services;

// TODO: Add grouping support
public interface ISearchProvider<TItem>
{
	IReadOnlyList<SearchFilter> Filters { get; }

	Task<IReadOnlyList<TItem>> SearchAsync(string searchTerm, SearchFilter? filter, CancellationToken cancellationToken = default);
}

public record SearchFilter(string Name, bool IsDefault = false);