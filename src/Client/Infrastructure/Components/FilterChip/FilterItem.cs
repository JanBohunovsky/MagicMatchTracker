namespace MagicMatchTracker.Client.Infrastructure.Components.FilterChip;

public record FilterItem<T>(string Id, string Label, Func<T, bool> Filter);