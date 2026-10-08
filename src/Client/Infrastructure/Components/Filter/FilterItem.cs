namespace MagicMatchTracker.Client.Infrastructure.Components.Filter;

public record FilterItem<T>(string Id, string Label, Func<T, bool> Predicate, bool IsDefault = false);