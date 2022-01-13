using System.Net;
using System.Net.Http;
using Mwi.LoanPay.Models.Token;
using Mwi.LoanPay.Tests.Integration.Helpers;
using Mwi.LoanPay.Tests.Unit.Helpers;
using Xunit;

namespace Mwi.LoanPay.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class TokenApiUnitTests
    {
        public Client GetTestClient(string responseBody, HttpStatusCode status)
        {
            var mockMessageHandler = new MockHttpMessageHandler(responseBody, status);
            var httpClient = new HttpClient(mockMessageHandler);
            return TestHelpers.GetTestClient(httpClient);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsSuccessTokenResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": ""9524628308510227"",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": {
                    ""error"": false,
                    ""message"": """",
                    ""isDebitCard"": false,
                    ""isCreditCard"": true,
                    ""isCommericalCard"": true,
                    ""network"": ""MCRD""
                  },
                  ""error"": false,
                  ""message"": """",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.False(actual.IsError);
            Assert.NotNull(actual.PaymentInformationToken);
            Assert.Equal("9524628308510227", actual.PaymentInformationToken.Token);
            Assert.Equal("76c24b49-0c60-4609-94e6-1167ebbcfe3b", actual.PaymentInformationToken.Id);
            Assert.Equal(TokenizationType.Card, actual.PaymentInformationToken.Type);
            Assert.Equal("8210", actual.PaymentInformationToken.AbbreviatedValue);
            Assert.Equal("MCRD", actual.PaymentInformationToken.Metadata.Network);
            Assert.True(actual.PaymentInformationToken.Metadata.IsCreditCard);
            Assert.False(actual.PaymentInformationToken.Metadata.IsDebitCard);
            Assert.True(actual.PaymentInformationToken.Metadata.IsCommericalCard);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        [InlineData(HttpStatusCode.BadRequest, "BadRequest")]
        public async void GetCardTokenAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": """",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": null,
                  ""error"": true,
                  ""message"": ""Tokenization failed"",
                  ""abbreviatedValue"": ""8210""
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsError_WithErrorTokenRequest()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": """",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": null,
                  ""error"": true,
                  ""message"": ""Tokenization failed"",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Tokenization failed", actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsError_WithMissingErrorField()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": """",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": null,
                  ""message"": ""Tokenization failed"",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Could not read error.", actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsError_WithoutErrorMessage()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": """",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": null,
                  ""error"": true,
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Could not read the error response message.", actual.Error);
        }


        [Fact]
        public async void GetCardTokenAsync_ReturnsSuccess_WithoutMetadata()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": ""9524628308510227"",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": null,
                  ""error"": false,
                  ""message"": """",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.False(actual.IsError);
            Assert.NotNull(actual.PaymentInformationToken);
            Assert.Equal("9524628308510227", actual.PaymentInformationToken.Token);
            Assert.Equal("76c24b49-0c60-4609-94e6-1167ebbcfe3b", actual.PaymentInformationToken.Id);
            Assert.Equal(TokenizationType.Card, actual.PaymentInformationToken.Type);
            Assert.Equal("8210", actual.PaymentInformationToken.AbbreviatedValue);
            Assert.Null(actual.PaymentInformationToken.Metadata);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsMetadataError_WithMissingMetadataErrorField()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": """",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": {
                    ""message"": ""Failed to retrieve metadata for card number ************4444""
                  },
                  ""error"": false,
                  ""message"": """",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Could not read metadata error.", actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsMetadataError_WithMissingMetadataMessageField()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": """",
                  ""type"": ""cc"",
                  ""frequency"": ""once"",
                  ""metadata"": {
                    ""error"": true,
                    ""message"": true
                  },
                  ""error"": false,
                  ""message"": """",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Could not read metadata message.", actual.Error);
        }

        [Fact]
        public async void GetCardTokenAsync_ReturnsMetadataInnerError_WithPartialResponse()
        {
            const string responseBody = @"
                {
                  ""id"": ""76c24b49-0c60-4609-94e6-1167ebbcfe3b"",
                  ""token"": ""9580884096653676"",
                  ""type"": ""cc"",
                  ""metadata"": {
                    ""error"": true,
                    ""message"": ""Failed to retrieve metadata for card number ************4444""
                  },
                  ""error"": false,
                  ""message"": """",
                  ""abbreviatedValue"": ""8210""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new TokenRequest
            {
                Id = "tracking_id",
                Type = TokenizationType.Card,
                Value = "5200828282828210"
            };

            var actual = await sut.TokenApi.GetPaymentInformationTokenAsync("", 0, 0, request);

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentInformationToken);

            Assert.StartsWith("Failed to retrieve metadata for card number", actual.Error);
        }
    }
}