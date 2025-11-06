using CustomerInsight.GoogleReview.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Options;

namespace CustomerInsight.GoogleReview.Services
{
    public sealed class GoogleTokenService
    {
        private readonly string _clientId, _clientSecret;
        public GoogleTokenService(IConfiguration cfg)
        {
            _clientId = cfg["GoogleOAuth:ClientId"]!;
            _clientSecret = cfg["GoogleOAuth:ClientSecret"]!;
        }

        public async Task<string> GetAccessTokenAsync(string refreshToken, string[] scopes, CancellationToken ct)
        {
            GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets { ClientId = _clientId, ClientSecret = _clientSecret },
                Scopes = scopes
            });

            TokenResponse token = new TokenResponse { RefreshToken = refreshToken };
            UserCredential cred = new UserCredential(flow, userId: "tenant", token);

            var ok = await cred.RefreshTokenAsync(ct).ConfigureAwait(false);
            if (ok == false || string.IsNullOrEmpty(cred.Token.AccessToken))
                throw new InvalidOperationException("Access-Token konnte nicht erneuert werden.");

            return cred.Token.AccessToken!;
        }
    }
}