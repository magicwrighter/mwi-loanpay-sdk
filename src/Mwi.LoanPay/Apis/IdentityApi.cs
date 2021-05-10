using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Mwi.LoanPay.Models.Identity;

namespace Mwi.LoanPay.Apis
{
    /// <summary>
    /// Used for getting an access token, for authentication with the other API operations
    /// </summary>
    public interface IIdentityApi
    {
        /// <summary>
        /// Request to get an access token
        /// </summary>
        /// <param name="request">Request model to get an access token</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<IdentityResponse> GetAccessTokenAsync(IdentityRequest request);
    }

    /// <inheritdoc cref="IIdentityApi"/>
    public class IdentityApi : IIdentityApi
    {
        private readonly IEnvironmentManager _environmentManager;
        private readonly HttpClient _httpClient;
        private readonly string _identityClientSecret;
        private readonly string _identityClientId;

        private string Scopes => _environmentManager.Env == Environment.Production
            ? "tkn mpx.payment"
            : "tkn mpx.sandbox";

        /// <summary>
        /// Creates an instance of the IdentityApi
        /// </summary>
        /// <param name="httpClient">Sets the HttpClient to be used by the IdentityApi</param>
        /// <param name="environmentManager">Sets the environment to be used by the IdentityApi</param>
        /// <param name="identityClientSecret">Sets the Identity Client Secret to be used by the IdentityApi</param>
        /// <param name="identityClientId">Sets the Identity Client Id to be used by the IdentityApi</param>
        public IdentityApi(HttpClient httpClient, IEnvironmentManager environmentManager,
            string identityClientId, string identityClientSecret)
        {
            _environmentManager = environmentManager;
            _httpClient = httpClient;
            _identityClientSecret = identityClientSecret;
            _identityClientId = identityClientId;
        }

        public async Task<IdentityResponse> GetAccessTokenAsync(IdentityRequest request)
        {
            var identityTokenRequest = new PasswordTokenRequest
            {
                RequestUri = new Uri(_environmentManager.IdentityUrl, "connect/token"),
                ClientId = _identityClientId,
                ClientSecret = _identityClientSecret,
                Scope = Scopes,

                UserName = request.GetCompositeUsername(),
                Password = request.Password
            };

            var response = await _httpClient.RequestPasswordTokenAsync(identityTokenRequest)
                .ConfigureAwait(false);

            if (response.IsError)
            {
                return new IdentityResponse
                {
                    Error = $"{response.Error} {response.ErrorDescription}"
                };
            }

            return new IdentityResponse
            {
                Token = new IdentityToken(response)
            };
        }
    }
}