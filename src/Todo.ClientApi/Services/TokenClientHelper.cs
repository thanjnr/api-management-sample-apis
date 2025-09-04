using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Abstractions;

namespace Todo.ClientApi.Services
{
    public class TokenClientHelper(
        IOptions<InternalApiSettings> settings,
        IAuthorizationHeaderProvider authorizationHeaderProvider,
        ILogger<ITokenClientHelper> logger) : ITokenClientHelper
    {
        private readonly InternalApiSettings _internalApiSettings = settings.Value;
        private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider = authorizationHeaderProvider;
        private readonly ILogger<ITokenClientHelper> _logger = logger;

        public async Task<string> GetAccessTokenAsync()
        {
            ValidateSettings();

            string token = await _authorizationHeaderProvider.CreateAuthorizationHeaderForAppAsync(_internalApiSettings.Scope);

            _logger.LogInformation($"Token: {token}");

            return token;
        }

        public async Task<string> GetManagedIdentityAccessTokenAsync()
        {
            ValidateSettings();

            string token = await GetAccessTokenWithManagedIdentityAsync(_internalApiSettings.Scope);

            _logger.LogInformation($"Token: {token}");

            return $"Bearer {token}";
        }

        private void ValidateSettings()
        {
            if (_internalApiSettings.ClientId == null || _internalApiSettings.Scope == null)
                throw new Exception($"ClientId, or Scope is null in app configuration." +
                    $" ClientId: {_internalApiSettings.ClientId}, Scope: {_internalApiSettings.Scope}");
        }

        private static async Task<string> GetAccessTokenWithClientSecretAsync(string resourceUrl, string tenantId,
                                                                              string clientId, string clientSecret)
        {
            return await GetTokenAsync(new ClientSecretCredential(tenantId, clientId, clientSecret), resourceUrl);
        }

        private static async Task<string> GetAccessTokenWithManagedIdentityAsync(string scope)
        {
            // Get the managed identity credential
            var managedIdentityCredential = new DefaultAzureCredential();

            // Create a Client Assertion containing the Managed Identity access token
            var tokenRequestContext = new TokenRequestContext(new[] { scope });
            var accessToken = await managedIdentityCredential.GetTokenAsync(tokenRequestContext).ConfigureAwait(false);
            return accessToken.Token;
        }

        private static async Task<string> GetTokenAsync(TokenCredential credential, string resourceUrl)
        {
            return (await credential.GetTokenAsync(
                new TokenRequestContext(scopes: [resourceUrl]) { }, new CancellationToken())).Token;
        }
    }
}