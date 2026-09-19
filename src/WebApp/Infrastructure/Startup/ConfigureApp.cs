using MagicMatchTracker.Data.Seed;
using MagicMatchTracker.Infrastructure.Authentication;
using MagicMatchTracker.Infrastructure.Layout;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace MagicMatchTracker.Infrastructure.Startup;

public static class ConfigureApp
{
	extension(WebApplication app)
	{
		public async Task ConfigureAsync()
		{
			app.ConfigureRequestPipeline();
			app.ConfigureMiddlewares();
			app.MapRoutes();
			await app.InitializeDatabaseAsync();
		}

		private void ConfigureRequestPipeline()
		{
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/error", createScopeForErrors: true);
			}

			app.UseStatusCodePagesWithReExecute("/error/{0}");
		}

		private void ConfigureMiddlewares()
		{
			app.UseAntiforgery();

			var cachePath = Path.Combine(app.Environment.WebRootPath, "img", "cache");
			if (!Directory.Exists(cachePath))
			{
				Directory.CreateDirectory(cachePath);
			}

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(cachePath),
				RequestPath = "/img/cache",
			});
		}

		private void MapRoutes()
		{
			app.MapStaticAssets();
			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			app.MapAuthenticationEndpoints();
		}

		private async Task InitializeDatabaseAsync()
		{
			using var scope = app.Services.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<Database>();
			var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

			await context.Database.MigrateAsync();
			await seeder.SeedAsync(context);
		}
	}
}