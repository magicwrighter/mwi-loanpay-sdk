using System.Net.Http;
using Mwi.LoanPay.Models.Identity;
using Mwi.LoanPay.Models.Token;

namespace Mwi.LoanPay.Tests.Integration.Helpers
{
    public static class TestHelpers
    {
        // These can be empty to test for Environment.Sandbox
        public const int ConsortiumId = 0;
        public const int BankNumber = 0;
        public const int CompanyNumber = 0;

        private const string IdentityClientSecret = "";
        private const string IdentityUserIdentifier = "";
        private const string IdentityPassword = "";

        public static Client GetTestClient(HttpClient client)
        {
            return new Client(client, new EnvironmentManager(Environment.Sandbox), IdentityClientSecret);
        }

        public static IdentityRequest GetValidIdentityRequest()
        {
            return new IdentityRequest(ConsortiumId, IdentityUserIdentifier, IdentityPassword);
        }

        public static string GetAccessToken(Client client)
        {
            var request = GetValidIdentityRequest();
            var identityResponse = client.IdentityApi.GetAccessTokenAsync(request)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return identityResponse?.Token?.AccessToken;
        }

        public static TokenResponse GetCardToken(string accessToken, Client client)
        {
            var tokenResponse = client.TokenApi.GetPaymentInformationTokenAsync(accessToken, BankNumber, CompanyNumber, new TokenRequest
                {
                    Id = "tracking_id",
                    Type = TokenizationType.Card,
                    Value = "5200828282828210"
                })
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return tokenResponse;
        }

        public static TokenResponse GetAccountToken(string accessToken, Client client)
        {
            var tokenResponse = client.TokenApi.GetPaymentInformationTokenAsync(accessToken, BankNumber, CompanyNumber, new TokenRequest
                {
                    Id = "account_id_1",
                    Type = TokenizationType.AccountNumber,
                    Value = "00000123456789"
                })
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return tokenResponse;
        }
        
        public static TokenResponse GetRoutingToken(string accessToken, Client client)
        {
            var tokenResponse = client.TokenApi.GetPaymentInformationTokenAsync(accessToken, BankNumber, CompanyNumber, new TokenRequest
                {
                    Id = "routing_id_1",
                    Type = TokenizationType.RoutingNumber,
                    Value = "123123123"
                })
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return tokenResponse;
        }
    }
}