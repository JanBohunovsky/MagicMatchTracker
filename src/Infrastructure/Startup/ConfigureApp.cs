using MagicMatchTracker.Features.Shared.Layout;

namespace MagicMatchTracker.Infrastructure.Startup;

public static class ConfigureApp
{
	public static async Task ConfigureAsync(this WebApplication app)
	{
		app.ConfigureRequestPipeline();
		app.ConfigureMiddlewares();
		app.MapRoutes();
		await app.InitializeDatabaseAsync();
	}

	private static void ConfigureRequestPipeline(this WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/error", createScopeForErrors: true);
		}
	}

	private static void ConfigureMiddlewares(this WebApplication app)
	{
		app.UseAntiforgery();
	}

	private static void MapRoutes(this WebApplication app)
	{
		app.MapStaticAssets();
		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();
	}

	private static async Task InitializeDatabaseAsync(this WebApplication app)
	{
		// TODO: Uncomment after setting up EF Core.

		// using var scope = app.Services.CreateScope();
		// var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		// await context.Database.MigrateAsync();
	}

}