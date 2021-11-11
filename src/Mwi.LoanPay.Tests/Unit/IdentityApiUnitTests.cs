using System.Net;
using System.Net.Http;
using Mwi.LoanPay.Models.Identity;
using Mwi.LoanPay.Tests.Integration.Helpers;
using Mwi.LoanPay.Tests.Unit.Helpers;
using Xunit;

namespace Mwi.LoanPay.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class IdentityApiUnitTests
    {
        public Client GetTestClient(string responseBody, HttpStatusCode status)
        {
            var mockMessageHandler = new MockHttpMessageHandler(responseBody, status);
            var httpClient = new HttpClient(mockMessageHandler);
            return TestHelpers.GetTestClient(httpClient);
        }

        [Fact]
        public async void GetAccessTokenAsync_ReturnsSuccessTokenResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"
                {
                    ""access_token"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ"",
                    ""expires_in"": 3600,
                    ""token_type"": ""Bearer""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new IdentityRequest(0, "", "");
            var actual = await sut.IdentityApi.GetAccessTokenAsync(request);

            Assert.False(actual.IsError);
            Assert.NotNull(actual.Token);

            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ", actual.Token.AccessToken);
            Assert.Equal(3600, actual.Token.ExpiresIn);
            Assert.Equal("Bearer", actual.Token.TokenType);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "Not Found")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void GetCardTokenAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            var sut = GetTestClient("", statusCode);

            var request = new IdentityRequest(0, "", "");
            var actual = await sut.IdentityApi.GetAccessTokenAsync(request);

            Assert.True(actual.IsError);
            Assert.Null(actual.Token);

            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"
                {
                    ""access_token"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ"",
                    ""expires_in"": 3600,
                    ""token_type"": ""Bearer""
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new IdentityRequest(0, "", "");
            var actual = await sut.IdentityApi.GetAccessTokenAsync(request);

            Assert.True(actual.IsError);
            Assert.Null(actual.Token);

            // The identity model library handles response parsing, we are just checking that we bubble the error back up
            Assert.StartsWith("Expected depth to be zero at the end of the JSON payload", actual.Error);
        }
    }
}