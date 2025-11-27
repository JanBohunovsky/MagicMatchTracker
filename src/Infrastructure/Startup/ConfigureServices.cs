using FluentValidation;
using MagicMatchTracker.Features.Matches;
using MagicMatchTracker.Features.Players;
using MagicMatchTracker.Features.Shared;
using MagicMatchTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Infrastructure.Startup;

public static class ConfigureServices
{
	extension(WebApplicationBuilder builder)
	{
		public void AddServices()
		{
			builder.AddBlazor();
			builder.AddDatabase();
			builder.AddInfrastructure();
			builder.AddFeatures();
		}

		private void AddBlazor()
		{
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();
		}

		private void AddDatabase()
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

		private void AddInfrastructure()
		{
			builder.Services.AddValidatorsFromAssemblyContaining<Program>();
			builder.Services
				.AddScoped<IMessageHub, MessageHub>()
				.AddScoped<FocusService>()
				.AddScoped<AutoCloseService>();
		}

		private void AddFeatures()
		{
			builder.Services
				.AddSharedFeature()
				.AddPlayersFeature()
				.AddMatchesFeature();
		}
	}
}