using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

// Based on https://github.com/dotnet/blazor-samples/blob/main/10.0/BlazorWebAppOidcBffServer/BlazorWebAppOidcServer/TokenHandler.cs
public sealed class DiscordTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (httpContextAccessor.HttpContext is null)
			throw new InvalidOperationException("HttpContext is not available.");

		var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync(TokenKeys.AccessToken);

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		return await base.SendAsync(request, cancellationToken);
	}
}