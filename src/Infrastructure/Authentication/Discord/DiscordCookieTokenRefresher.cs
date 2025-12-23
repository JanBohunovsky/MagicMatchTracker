using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

// Based on https://github.com/dotnet/blazor-samples/blob/main/10.0/BlazorWebAppOidcBffServer/BlazorWebAppOidcServer/CookieOidcRefresher.cs
public sealed class DiscordCookieTokenRefresher(ILogger<DiscordCookieTokenRefresher> logger)
{
	public async Task ValidateOrRefreshTokenAsync(CookieValidatePrincipalContext context)
	{
		var tokenExpirationText = context.Properties.GetTokenValue(TokenKeys.ExpiresAt);
		if (!DateTimeOffset.TryParse(tokenExpirationText, out var tokenExpiration))
		{
			logger.LogWarning("Could not parse token expiration date from cookie: {TokenExpirationText}", tokenExpirationText);
			return;
		}

		var cancellationToken = context.HttpContext.RequestAborted;
		var authHandlerProvider = context.HttpContext
			.RequestServices
			.GetRequiredService<DiscordAuthenticationHandlerProvider>();

		var authHandler = await authHandlerProvider.GetHandlerAsync(context.HttpContext);
		var options = authHandler.Options;

		var now = options.TimeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;
		if (now + TimeSpan.FromMinutes(5) < tokenExpiration)
			return;

		var refreshToken = context.Properties.GetTokenValue(TokenKeys.RefreshToken)
			?? throw new InvalidOperationException("No refresh token found in cookie");

		var refreshJson = await RefreshTokenAsync(options, refreshToken, cancellationToken);
		if (refreshJson is null)
		{
			context.RejectPrincipal();
			return;
		}

		using var tokenResponse = OAuthTokenResponse.Success(JsonDocument.Parse(refreshJson));

		if (tokenResponse.AccessToken.IsEmpty())
		{
			logger.LogWarning("Failed to retrieve access token from response: {RefreshResponse}", refreshJson);

			context.RejectPrincipal();
			return;
		}

		var tokens = GetTokens(tokenResponse, options);
		context.Properties.StoreTokens(tokens);

		var authTicket = await authHandler.CreateTicketAsync(context.Properties, tokenResponse);

		logger.LogInformation("Refreshed token, new expiration date: {TokenExpiration}",
			authTicket.Properties.GetTokenValue(TokenKeys.ExpiresAt));

		context.ShouldRenew = true;
		context.ReplacePrincipal(authTicket.Principal);
	}

	private async Task<string?> RefreshTokenAsync(DiscordAuthenticationOptions options, string refreshToken, CancellationToken cancellationToken)
	{
		using var response = await options.Backchannel.PostAsync(options.TokenEndpoint, new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				[TokenKeys.GrantType] = TokenKeys.RefreshToken,
				[TokenKeys.RefreshToken] = refreshToken,
				[TokenKeys.ClientId] = options.ClientId,
				[TokenKeys.ClientSecret] = options.ClientSecret,
			}), cancellationToken);

		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadAsStringAsync(cancellationToken);
		}

		var body = await response.Content.ReadAsStringAsync(cancellationToken);
		logger.LogWarning("Failed to refresh token, the remote server returned a {Status} response with the following payload: {Headers}{Body}",
			response.StatusCode, response.Headers.ToString(), body);

		return null;

	}

	private IEnumerable<AuthenticationToken> GetTokens(OAuthTokenResponse response, DiscordAuthenticationOptions options)
	{
		if (response.AccessToken.IsEmpty())
			throw new InvalidOperationException("Failed to retrieve access token.");

		yield return new AuthenticationToken
		{
			Name = TokenKeys.AccessToken,
			Value = response.AccessToken,
		};

		if (response.RefreshToken.IsNotEmpty())
		{
			yield return new AuthenticationToken
			{
				Name = TokenKeys.RefreshToken,
				Value = response.RefreshToken,
			};
		}

		if (response.TokenType.IsNotEmpty())
		{
			yield return new AuthenticationToken
			{
				Name = TokenKeys.TokenType,
				Value = response.TokenType,
			};
		}

		if (response.ExpiresIn.IsEmpty() || !int.TryParse(response.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out var expiresInSeconds))
			yield break;

		var now = options.TimeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;
		var expiresAt = now.AddSeconds(expiresInSeconds);

		yield return new AuthenticationToken
		{
			Name = TokenKeys.ExpiresAt,
			Value = expiresAt.ToString("o", CultureInfo.InvariantCulture),
		};
	}
}