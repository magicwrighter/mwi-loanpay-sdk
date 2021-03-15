using System.Net.Http;
using Mwi.LoanPay.Models.Token;
using Mwi.LoanPay.Tests.Integration.Helpers;
using Xunit;
using Xunit.Abstractions;
namespace Mwi.LoanPay.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class TokenApiTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Client _loanPayClient;
        private readonly string _accessToken;
        public TokenApiTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var httpClient = new HttpClient();
            _loanPayClient = TestHelpers.GetTestClient(httpClient);
            _accessToken = TestHelpers.GetAccessToken(_loanPayClient);
        }
        [Fact]
        public void GetCardTokenAsync_GetsTokenSuccessfully()
        {
            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210" // A fake card
            };
            var response = _loanPayClient.TokenApi.GetPaymentInformationTokenAsync(_accessToken, TestHelpers.BankNumber, TestHelpers.CompanyNumber, request)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            if (response.IsError)
            {
                _testOutputHelper.WriteLine(response.Error);
            }
            // Request was successful
            Assert.False(response.IsError);
            var actual = response.PaymentInformationToken;
            Assert.Equal(request.Id, actual.Id);
            Assert.Equal(request.Type, actual.Type);
            Assert.NotEmpty(actual.Token);
            Assert.Equal("MCRD", actual.Metadata.Network);
            Assert.True(actual.Metadata.IsCreditCard);
            Assert.False(actual.Metadata.IsCommericalCard);
            Assert.False(actual.Metadata.IsDebitCard);
        }
    }
}