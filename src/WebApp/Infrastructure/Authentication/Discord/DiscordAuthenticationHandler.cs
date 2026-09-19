using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace MagicMatchTracker.Infrastructure.Authentication.Discord;

// Based on https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/blob/dev/src/AspNet.Security.OAuth.Discord/DiscordAuthenticationHandler.cs
public sealed class DiscordAuthenticationHandler(
	IOptionsMonitor<DiscordAuthenticationOptions> options,
	ILoggerFactory loggerFactory,
	UrlEncoder urlEncoder) : OAuthHandler<DiscordAuthenticationOptions>(options, loggerFactory, urlEncoder)
{
	public Task<AuthenticationTicket> CreateTicketAsync(AuthenticationProperties properties, OAuthTokenResponse tokens)
		=> CreateTicketAsync(new ClaimsIdentity(ClaimsIssuer), properties, tokens);

	protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

		using var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);
		if (!response.IsSuccessStatusCode)
		{
			var body = await response.Content.ReadAsStringAsync(Context.RequestAborted);
			Logger.LogError("Failed to retrieve user information, the remote server returned a {Status} response with the following payload: {Headers}{Body}",
				response.StatusCode, response.Headers.ToString(), body);

			throw new HttpRequestException("An error has occurred while retrieving the user information.");
		}

		var content = await response.Content.ReadAsStringAsync(Context.RequestAborted);
		using var payload = JsonDocument.Parse(content);

		var principal = new ClaimsPrincipal(identity);
		var context = new OAuthCreatingTicketContext(principal, properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
		context.RunClaimActions();

		if (Options.OwnerId is not null && Options.OwnerId == identity.DiscordId)
		{
			identity.AddClaim(new Claim(ClaimTypes.Role, Roles.Owner));
		}

		await Events.CreatingTicket(context);

		Logger.LogInformation("Created ticket for {DisplayName} ({UserName}, {DiscordId}) with roles [{Roles}]",
			identity.DisplayName, identity.Name, identity.DiscordId, identity.FindAll(ClaimTypes.Role).Select(c => c.Value));

		return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
	}

	protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
	{
		var challengeUrl = base.BuildChallengeUrl(properties, redirectUri);

		if (Options.Prompt is not null)
		{
			challengeUrl = QueryHelpers.AddQueryString(challengeUrl, "prompt", Options.Prompt);
		}

		return challengeUrl;
	}
}