using System.Net.Http;
using Mwi.LoanPay.Models.Identity;
using Mwi.LoanPay.Tests.Integration.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mwi.LoanPay.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class IdentityApiTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;

        public IdentityApiTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _client = new HttpClient();
        }

        [Fact]
        public void GetAccessTokenAsync_GetsTokenSuccessfully()
        {
            var client = TestHelpers.GetTestClient(_client);
            
            var response = client.IdentityApi.GetAccessTokenAsync(TestHelpers.GetValidIdentityRequest())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (response.IsError)
            {
                _testOutputHelper.WriteLine(response.Error);
            }

            // Request was successful
            Assert.False(response.IsError);

            _testOutputHelper.WriteLine(response.Token.AccessToken);

            // An access token exists.
            Assert.True(response.Token.AccessToken.Length > 30);
        }
    }
}