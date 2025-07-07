using MagicMatchTracker.Infrastructure.Layout;
using Microsoft.EntityFrameworkCore;

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

		app.UseStatusCodePagesWithReExecute("/error/{0}");
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
		using var scope = app.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<Database>();
		await context.Database.MigrateAsync();
	}

}