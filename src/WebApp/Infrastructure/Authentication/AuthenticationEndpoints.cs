using MagicMatchTracker.Infrastructure.Authentication.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MagicMatchTracker.Infrastructure.Authentication;

public static class AuthenticationEndpoints
{
	public const string GroupPath = "/account";
	public const string SignInPath = "/signin";
	public const string SignOutPath = "/signout";

	public static void MapAuthenticationEndpoints(this WebApplication app)
	{
		var group = app.MapGroup(GroupPath);

		group.MapGet(SignInPath, SignIn)
			.AllowAnonymous();

		group.MapPost(SignOutPath, SignOut);
	}

	private static Task<IResult> SignIn(HttpContext context, string? returnUrl)
	{
		IResult result = TypedResults.Challenge(GetAuthenticationProperties(context, returnUrl),
			[DiscordAuthenticationDefaults.AuthenticationScheme]);

		return Task.FromResult(result);
	}

	private static Task<IResult> SignOut(HttpContext context, [FromForm] string? returnUrl)
	{
		IResult result = TypedResults.SignOut(GetAuthenticationProperties(context, returnUrl),
			[CookieAuthenticationDefaults.AuthenticationScheme]);

		return Task.FromResult(result);
	}

	private static AuthenticationProperties GetAuthenticationProperties(HttpContext context, string? returnUrl)
	{
		var pathBase = $"{context.Request.PathBase}/";

		if (returnUrl.IsEmpty())
		{
			returnUrl = pathBase;
		}
		else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
		{
			returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
		}
		else if (!returnUrl.StartsWith('/'))
		{
			returnUrl = $"{pathBase}{returnUrl}";
		}

		return new AuthenticationProperties
		{
			RedirectUri = returnUrl,
		};
	}
}