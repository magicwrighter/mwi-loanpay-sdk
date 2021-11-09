using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Mwi.LoanPay.Models.LoanPay;
using Mwi.LoanPay.Tests.Integration.Helpers;
using Mwi.LoanPay.Tests.Unit.Helpers;
using Xunit;

namespace Mwi.LoanPay.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class LoanPayApiUnitTests
    {
        public Client GetTestClient(string responseBody, HttpStatusCode status)
        {
            var mockMessageHandler = new MockHttpMessageHandler(responseBody, status);
            var httpClient = new HttpClient(mockMessageHandler);
            return TestHelpers.GetTestClient(httpClient);
        }

        #region CalculateFeeAsync

        [Fact]
        public async void CalculateFeeAsync_ReturnsSuccessfulFeeResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"  
                {
                    ""feeAmount"": 4.65
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new AchFeeRequest
            {
                BankNumber = 5000,
                CompanyNumber = 3939,
                Amount = 12.34m
            };

            var actual = await sut.LoanPayApi.CalculateFeeAsync("", request);

            Assert.False(actual.IsError);
            Assert.Null(actual.Error);
            Assert.Equal(4.65m, actual.FeeAmount);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void CalculateFeeAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var request = new AchFeeRequest
            {
                BankNumber = 5000,
                CompanyNumber = 3939,
                Amount = 12.34m
            };

            var actual = await sut.LoanPayApi.CalculateFeeAsync("", request);

            Assert.True(actual.IsError);
            Assert.Equal(0m, actual.FeeAmount);
            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void CalculateFeeAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"
                {
                    ""feeAmount"": 4.65
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var request = new AchFeeRequest
            {
                BankNumber = 5000,
                CompanyNumber = 3939,
                Amount = 12.34m
            };

            var actual = await sut.LoanPayApi.CalculateFeeAsync("", request);

            Assert.True(actual.IsError);
            Assert.Equal(0m, actual.FeeAmount);

            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void CalculateFeeAsync_ReturnsError_WithErrorResponse()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""ach-fee-error"",
                      ""message"": ""Failed to get fee."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var request = new AchFeeRequest
            {
                BankNumber = 5000,
                CompanyNumber = 3939,
                Amount = 12.34m
            };

            var actual = await sut.LoanPayApi.CalculateFeeAsync("", request);

            Assert.True(actual.IsError);
            Assert.Equal(0m, actual.FeeAmount);

            Assert.Contains("Failed to get fee.", actual.Error);
        }

        [Fact]
        public async void CalculateFeeAsync_ReturnsError_WithMissingErrorsField()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var request = new AchFeeRequest
            {
                BankNumber = 5000,
                CompanyNumber = 3939,
                Amount = 12.34m
            };

            var actual = await sut.LoanPayApi.CalculateFeeAsync("", request);

            Assert.True(actual.IsError);
            Assert.Equal(0m, actual.FeeAmount);
            Assert.Contains("Could not read the response body.", actual.Error);
        }

        #endregion CalculateFeeAsync

        #region SubmitPaymentAsync

        [Fact]
        public async void SubmitPaymentAsync_ReturnsSuccessfulResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"  
                {
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amount"": 12.34,
                  ""convenienceFee"": 4.65,
                  ""processDate"": ""2021-10-27T00:00:00.000Z"",
                  ""name"": ""Test Payment"",
                  ""confirmationNumber"": ""1234567890""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var cardPayment = new CardPaymentRequest();
            var actual = await sut.LoanPayApi.SubmitPaymentAsync("", cardPayment);

            Assert.False(actual.IsError);
            Assert.Null(actual.Error);
            Assert.NotNull(actual.Confirmation);
            Assert.Equal("123123", actual.Confirmation.AccountNumber);
            Assert.Equal(12.34m, actual.Confirmation.Amount);
            Assert.Equal(4.65m, actual.Confirmation.ConvenienceFee);
            Assert.Equal(new DateTime(2021, 10, 27), actual.Confirmation.ProcessDate);
            Assert.Equal("Test Payment", actual.Confirmation.Name);
            Assert.Equal("1234567890", actual.Confirmation.ConfirmationNumber);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void SubmitPaymentAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var cardPayment = new CardPaymentRequest();
            var actual = await sut.LoanPayApi.SubmitPaymentAsync("", cardPayment);

            Assert.True(actual.IsError);
            Assert.Null(actual.Confirmation);
            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void SubmitPaymentAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"  
                {
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amount"": 12.34,
                  ""convenienceFee"": 4.65,
                  ""processDate"": ""2021-10-27T00:00:00.000Z"",
                  ""name"": ""Test Payment"",
                  ""confirmationNumber"": ""1234567890""
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var cardPayment = new CardPaymentRequest();
            var actual = await sut.LoanPayApi.SubmitPaymentAsync("", cardPayment);

            Assert.True(actual.IsError);
            Assert.Null(actual.Confirmation);

            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void SubmitPaymentAsync_ReturnsError_WithErrorResponse()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""card-payment-error"",
                      ""message"": ""Failed to submit card payment."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var cardPayment = new CardPaymentRequest();
            var actual = await sut.LoanPayApi.SubmitPaymentAsync("", cardPayment);

            Assert.True(actual.IsError);
            Assert.Null(actual.Confirmation);
            Assert.Contains("Failed to submit card payment.", actual.Error);
        }

        [Fact]
        public async void SubmitPaymentAsync_ReturnsError_WithMissingErrorsField()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var cardPayment = new CardPaymentRequest();
            var actual = await sut.LoanPayApi.SubmitPaymentAsync("", cardPayment);

            Assert.True(actual.IsError);
            Assert.Null(actual.Confirmation);
            Assert.Contains("Could not read the response body.", actual.Error);
        }

        #endregion SubmitPaymentAsync

        #region GetPaymentStatusAsync

        [Fact]
        public async void GetPaymentStatusAsync_ReturnsSuccessfulResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"  
                {
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amount"": 12.34,
                  ""convenienceFee"": 4.65,
                  ""postDate"": ""2021-10-27T18:12:15.810Z"",
                  ""confirmationNumber"": ""1234567890"",
                  ""status"": ""Processed""
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.GetPaymentStatusAsync("", "");

            Assert.False(actual.IsError);
            Assert.Null(actual.Error);
            Assert.NotNull(actual.PaymentStatus);
            Assert.Equal("123123", actual.PaymentStatus.AccountNumber);
            Assert.Equal(5000, actual.PaymentStatus.BankNumber);
            Assert.Equal(3939, actual.PaymentStatus.CompanyNumber);
            Assert.Equal(12.34m, actual.PaymentStatus.Amount);
            Assert.Equal(4.65m, actual.PaymentStatus.ConvenienceFee);
            Assert.Equal("1234567890", actual.PaymentStatus.ConfirmationNumber);
            Assert.Equal(Status.Processed, actual.PaymentStatus.Status);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void GetPaymentStatusAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var actual = await sut.LoanPayApi.GetPaymentStatusAsync("", "");

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentStatus);
            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void GetPaymentStatusAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"  
                {
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amount"": 12.34,
                  ""convenienceFee"": 4.65,
                  ""postDate"": ""2021-10-27T18:12:15.810Z"",
                  ""confirmationNumber"": ""1234567890"",
                  ""status"": ""Processed""
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.GetPaymentStatusAsync("", "");

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentStatus);

            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void GetPaymentStatusAsync_ReturnsError_WithErrorResponse()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""payment-status-error"",
                      ""message"": ""Failed to retrieve payment status."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.GetPaymentStatusAsync("", "");

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentStatus);
            Assert.Contains("Failed to retrieve payment status.", actual.Error);
        }

        [Fact]
        public async void GetPaymentStatusAsync_ReturnsError_WithMissingErrorsField()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.GetPaymentStatusAsync("", "");

            Assert.True(actual.IsError);
            Assert.Null(actual.PaymentStatus);
            Assert.Contains("Could not read the response body.", actual.Error);
        }

        #endregion GetPaymentStatusAsync

        #region GetAccountDetailsAsync

        [Fact]
        public async void GetAccountDetailsAsync_ReturnsSuccessfulResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"  
                {
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amountDue"": 12.34,
                  ""dueDate"": ""2021-10-27T00:00:00.000Z"",
                  ""maximumAchPaymentAmount"": 234.56,
                  ""minimumAchPaymentAmount"": 2.34,
                  ""maximumCardPaymentAmount"": 123.45,
                  ""minimumCardPaymentAmount"": 1.23
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.GetAccountDetailsAsync("", 5000, 3939, "123123");

            Assert.False(actual.IsError);
            Assert.Null(actual.Error);
            Assert.NotNull(actual.AccountDetails);
            Assert.Equal("123123", actual.AccountDetails.AccountNumber);
            Assert.Equal(12.34m, actual.AccountDetails.AmountDue);
            Assert.Equal(new DateTime(2021, 10, 27), actual.AccountDetails.DueDate);
            Assert.Equal(234.56m, actual.AccountDetails.MaximumAchPaymentAmount);
            Assert.Equal(2.34m, actual.AccountDetails.MinimumAchPaymentAmount);
            Assert.Equal(123.45m, actual.AccountDetails.MaximumCardPaymentAmount);
            Assert.Equal(1.23m, actual.AccountDetails.MinimumCardPaymentAmount);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void GetAccountDetailsAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var actual = await sut.LoanPayApi.GetAccountDetailsAsync("", 5000, 3939, "123123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetails);
            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void GetAccountDetailsAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"  
                {
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amountDue"": 12.34,
                  ""dueDate"": ""2021-10-27T00:00:00.000Z"",
                  ""maximumAchPaymentAmount"": 234.56,
                  ""minimumAchPaymentAmount"": 2.34,
                  ""maximumCardPaymentAmount"": 123.45,
                  ""minimumCardPaymentAmount"": 1.23
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.GetAccountDetailsAsync("", 5000, 3939, "123123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetails);

            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void GetAccountDetailsAsync_ReturnsError_WithErrorResponse()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""get-loanpay-account-error"",
                      ""message"": ""Failed to get LoanPay account."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.GetAccountDetailsAsync("", 5000, 3939, "123123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetails);
            Assert.Contains("Failed to get LoanPay account.", actual.Error);
        }

        [Fact]
        public async void GetAccountDetailsAsync_ReturnsError_WithMissingErrorsField()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.GetAccountDetailsAsync("", 5000, 3939, "123123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetails);
            Assert.Contains("Could not read the response body.", actual.Error);
        }

        #endregion GetAccountDetailsAsync

        #region GetAccountDetailsByPrefixAsync

        [Fact]
        public async void GetAccountDetailsByPrefixAsync_ReturnsSuccessfulResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"  
                [{
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amountDue"": 12.34,
                  ""dueDate"": ""2021-10-27T00:00:00.000Z"",
                  ""maximumAchPaymentAmount"": 234.56,
                  ""minimumAchPaymentAmount"": 2.34,
                  ""maximumCardPaymentAmount"": 123.45,
                  ""minimumCardPaymentAmount"": 1.23
                }]";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.GetAccountDetailsByPrefixAsync("", 5000, 3939, "123");

            Assert.False(actual.IsError);
            Assert.Null(actual.Error);
            Assert.NotNull(actual.AccountDetailsList);

            var first = actual.AccountDetailsList.FirstOrDefault();

            Assert.Equal("123123", first.AccountNumber);
            Assert.Equal(12.34m, first.AmountDue);
            Assert.Equal(new DateTime(2021, 10, 27), first.DueDate);
            Assert.Equal(234.56m, first.MaximumAchPaymentAmount);
            Assert.Equal(2.34m, first.MinimumAchPaymentAmount);
            Assert.Equal(123.45m, first.MaximumCardPaymentAmount);
            Assert.Equal(1.23m, first.MinimumCardPaymentAmount);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void GetAccountDetailsByPrefixAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var actual = await sut.LoanPayApi.GetAccountDetailsByPrefixAsync("", 5000, 3939, "123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetailsList);
            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void GetAccountDetailsByPrefixAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"  
                [{
                  ""accountNumber"": ""123123"",
                  ""bankNumber"": 5000,
                  ""companyNumber"": 3939,
                  ""amountDue"": 12.34,
                  ""dueDate"": ""2021-10-27T00:00:00.000Z"",
                  ""maximumAchPaymentAmount"": 234.56,
                  ""minimumAchPaymentAmount"": 2.34,
                  ""maximumCardPaymentAmount"": 123.45,
                  ""minimumCardPaymentAmount"": 1.23
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.GetAccountDetailsByPrefixAsync("", 5000, 3939, "123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetailsList);

            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void GetAccountDetailsByPrefixAsync_ReturnsError_WithErrorResponse()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""get-loanpay-accounts-error"",
                      ""message"": ""Failed to get list of LoanPay accounts."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.GetAccountDetailsByPrefixAsync("", 5000, 3939, "123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetailsList);
            Assert.Contains("Failed to get list of LoanPay accounts.", actual.Error);
        }

        [Fact]
        public async void GetAccountDetailsByPrefixAsync_ReturnsError_WithMissingErrorsField()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.GetAccountDetailsByPrefixAsync("", 5000, 3939, "123");

            Assert.True(actual.IsError);
            Assert.Null(actual.AccountDetailsList);
            Assert.Contains("Could not read the response body.", actual.Error);
        }

        #endregion GetAccountDetailsByPrefixAsync

        #region CancelPaymentAsync

        [Fact]
        public async void CancelPaymentAsync_ReturnsSuccessfulResponse_WithSuccessfulRequest()
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, HttpStatusCode.OK);

            var actual = await sut.LoanPayApi.CancelPaymentAsync("", "1234567890");

            Assert.False(actual.IsError);
            Assert.Null(actual.Error);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "NotFound")]
        [InlineData(HttpStatusCode.Unauthorized, "Unauthorized")]
        public async void CancelPaymentAsync_ReturnsError_WithFailedHttpStatusCode(HttpStatusCode statusCode, string responseString)
        {
            const string responseBody = @"";

            var sut = GetTestClient(responseBody, statusCode);

            var actual = await sut.LoanPayApi.CancelPaymentAsync("", "1234567890");

            Assert.True(actual.IsError);
            Assert.StartsWith(responseString, actual.Error);
        }

        [Fact]
        public async void CancelPaymentAsync_ReturnsError_WithMalformedJson()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""cancel-payment-error"",
                      ""message"": ""Failed to cancel payment, no pending payment found."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                ";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.CancelPaymentAsync("", "1234567890");

            Assert.True(actual.IsError);
            Assert.StartsWith("Could not read the response body", actual.Error);
        }

        [Fact]
        public async void CancelPaymentAsync_ReturnsError_WithErrorResponse()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""errors"": [
                    {
                      ""code"": ""cancel-payment-error"",
                      ""message"": ""Failed to cancel payment, no pending payment found."",
                      ""exception"": { }
                    }
                  ],
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.CancelPaymentAsync("", "1234567890");

            Assert.True(actual.IsError);
            Assert.Contains("Failed to cancel payment, no pending payment found.", actual.Error);
        }

        [Fact]
        public async void CancelPaymentAsync_ReturnsError_WithMissingErrorsField()
        {
            const string responseBody = @"
                {
                  ""status"": ""BadRequest"",
                  ""isError"": true,
                  ""isEmpty"": true
                }";

            var sut = GetTestClient(responseBody, HttpStatusCode.BadRequest);

            var actual = await sut.LoanPayApi.CancelPaymentAsync("", "1234567890");

            Assert.True(actual.IsError);
            Assert.Contains("Could not read the response body.", actual.Error);
        }

        #endregion CancelPaymentAsync
    }
}