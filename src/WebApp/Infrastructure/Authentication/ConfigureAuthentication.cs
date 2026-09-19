using MagicMatchTracker.Infrastructure.Authentication.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MagicMatchTracker.Infrastructure.Authentication;

public static class ConfigureAuthentication
{
	public static IServiceCollection AddDiscordAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.LoginPath = $"{AuthenticationEndpoints.GroupPath}{AuthenticationEndpoints.SignInPath}";
				options.LogoutPath = $"{AuthenticationEndpoints.GroupPath}{AuthenticationEndpoints.SignOutPath}";
				options.AccessDeniedPath = "/forbidden";
				options.Cookie.Name = "MagicMatchTracker.Auth.Cookie";
				options.Cookie.MaxAge = TimeSpan.FromDays(30);
			})
			.AddDiscord(options =>
			{
				var section = configuration.GetRequiredSection("Auth:Discord");
				options.ClientId = section["ClientId"]
					?? throw new InvalidOperationException("ClientId for Discord authentication is not specified.");
				options.ClientSecret = section["ClientSecret"]
					?? throw new InvalidOperationException("ClientSecret for Discord authentication is not specified.");
				options.OwnerId = section["OwnerId"]
					?? throw new InvalidOperationException("OwnerId for Discord authentication is not specified.");

				// We don't want to prompt the user to authorise the app every time they sign in
				options.Prompt = DiscordAuthenticationConstants.Prompts.None;

				options.SaveTokens = true;
			});

		services.AddScoped<DiscordAuthenticationHandlerProvider>();
		services.AddSingleton<DiscordCookieTokenRefresher>();

		services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
			.Configure<DiscordCookieTokenRefresher>((options, refresher) =>
			{
				options.Events.OnValidatePrincipal += refresher.ValidateOrRefreshTokenAsync;
			});

		return services;
	}
}