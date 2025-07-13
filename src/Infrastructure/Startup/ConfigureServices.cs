using FluentValidation;
using MagicMatchTracker.Features.Players;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Infrastructure.Startup;

public static class ConfigureServices
{
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.AddBlazor();
		builder.AddDatabase();
		builder.AddInfrastructure();
		builder.AddFeatures();
	}

	private static void AddBlazor(this WebApplicationBuilder builder)
	{
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();
	}

	private static void AddDatabase(this WebApplicationBuilder builder)
	{
		builder.Services.AddNpgsql<Database>(builder.Configuration.GetConnectionString("Default"),
			npgsqlOptions =>
			{
				npgsqlOptions.SetPostgresVersion(17, 5);
			},
			dbContextOptions =>
			{
				dbContextOptions.UseSnakeCaseNamingConvention();
			});
	}

	private static void AddInfrastructure(this WebApplicationBuilder builder)
	{
		builder.Services.AddValidatorsFromAssemblyContaining<Program>();
		builder.Services.AddScoped<FocusService>();
	}

	private static void AddFeatures(this WebApplicationBuilder builder)
	{
		builder.Services.AddPlayersFeature();
	}
}