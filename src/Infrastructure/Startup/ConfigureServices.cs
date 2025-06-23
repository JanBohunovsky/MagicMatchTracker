namespace MagicMatchTracker.Infrastructure.Startup;

public static class ConfigureServices
{
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.AddBlazor();
	}

	private static void AddBlazor(this WebApplicationBuilder builder)
	{
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();
	}
}