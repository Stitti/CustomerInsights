using CustomerInsight.GoogleAnalyticsWorker.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Options;

namespace CustomerInsight.GoogleAnalyticsWorker.Services
{
    public sealed class GoogleTokenService
    {
        private readonly GoogleOAuthSection _cfg;
        public GoogleTokenService(IOptions<GoogleOAuthSection> cfg) => _cfg = cfg.Value;

        public async Task<string> GetAccessTokenAsync(string refreshToken, string[] scopes, CancellationToken ct)
        {
            GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets { ClientId = _cfg.ClientId, ClientSecret = _cfg.ClientSecret },
                Scopes = scopes
            });

            TokenResponse token = new TokenResponse { RefreshToken = refreshToken };
            UserCredential cred = new UserCredential(flow, userId: "ignored", token);
            bool refreshed = await cred.RefreshTokenAsync(ct).ConfigureAwait(false);
            if (refreshed == false || string.IsNullOrEmpty(cred.Token.AccessToken))
                throw new InvalidOperationException("Konnte Access-Token nicht erneuern (Refresh-Token evtl. ungültig).");

            return cred.Token.AccessToken!;
        }
    }
}