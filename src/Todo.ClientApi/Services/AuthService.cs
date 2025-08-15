using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Abstractions;
using System.Diagnostics;

namespace Todo.ClientApi.Services
{
    public class AuthService(IOptions<InternalApiSettings> settings, IAuthorizationHeaderProvider authorizationHeaderProvider) : IAuthService
    {
        private readonly string? _tenantId = settings.Value.TenantId;
        private readonly string? _clientId = settings.Value.ClientId;
        private readonly string? _clientSecret = settings.Value.Secret;
        private readonly string _url = settings.Value.TokenUrl;
        private readonly string? _scope = settings.Value.Scope;
        private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider = authorizationHeaderProvider;

        public async Task<string> GetAccessTokenAsync()
        {
            string token;

            if (Debugger.IsAttached)
            {
                if (_tenantId == null || _clientId == null || _clientSecret == null)
                    throw new Exception($"TenantId, ClientId, or ClientSecret is null in app configuration." +
                        $" TenantId: {_tenantId}, ClientId: {_clientId}, ClientSecret length: {_clientSecret?.Length}");
                else
                    token = await GetAccessTokenWithClientSecretAsync(_url, _tenantId, _clientId, _clientSecret);
            }
            else
            {
                // Acquire the access token.
                var scope = _scope == null || _scope == string.Empty ? ".default" : _scope;
                token = await _authorizationHeaderProvider.CreateAuthorizationHeaderForAppAsync(scope);

            }

            return token;
        }

        private static async Task<string> GetAccessTokenWithClientSecretAsync(string resourceUrl, string tenantId,
                                                                              string clientId, string clientSecret)
        {
            return await GetTokenAsync(new ClientSecretCredential(tenantId, clientId, clientSecret), resourceUrl);
        }

        private static async Task<string> GetAccessTokenWithManagedIdentityAsync(string resourceUrl)
        {
            return await GetTokenAsync(new ManagedIdentityCredential(), resourceUrl);
        }

        private static async Task<string> GetTokenAsync(TokenCredential credential, string resourceUrl)
        {
            return (await credential.GetTokenAsync(
                new TokenRequestContext(scopes: [resourceUrl]) { }, new CancellationToken())).Token;
        }
    }
}