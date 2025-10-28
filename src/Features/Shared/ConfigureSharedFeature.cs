using MagicMatchTracker.Features.Shared.Services;

namespace MagicMatchTracker.Features.Shared;

public static class ConfigureSharedFeature
{
	public static IServiceCollection AddSharedFeature(this IServiceCollection services)
	{
		services.AddScoped<DeckEditState>();

		return services;
	}
}