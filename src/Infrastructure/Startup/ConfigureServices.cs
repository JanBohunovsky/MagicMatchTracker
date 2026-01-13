using FluentValidation;
using MagicMatchTracker.Data.Seed;
using MagicMatchTracker.Features.Matches;
using MagicMatchTracker.Features.Players;
using MagicMatchTracker.Features.Shared;
using MagicMatchTracker.Infrastructure.Authentication;
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
			builder.Services.AddDbContext<Database>(options =>
			{
				options.UseSnakeCaseNamingConvention();
				options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
					npgsql => npgsql.SetPostgresVersion(17, 5));
			});

			builder.Services.AddTransient<DataSeeder>();
			builder.Services.AddTransient<GristImporter>();
		}

		private void AddInfrastructure()
		{
			builder.Services.AddDiscordAuthentication(builder.Configuration);
			builder.Services.AddCascadingAuthenticationState();
			builder.Services.AddValidatorsFromAssemblyContaining<Program>();
			builder.Services
				.AddSingleton<ApplicationVersionProvider>()
				.AddScoped<IMessageHub, MessageHub>()
				.AddScoped<FocusService>()
				.AddScoped<AutoCloseService>()
				.AddScoped<StickyDetectorService>();
		}

		private void AddFeatures()
		{
			builder.Services
				.AddSharedFeature()
				.AddPlayersFeature(builder.Configuration)
				.AddMatchesFeature(builder.Configuration);
		}
	}
}