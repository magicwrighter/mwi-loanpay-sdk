using System;
using System.Net.Http;
using System.Threading;
using Mwi.LoanPay.Models.LoanPay;
using Mwi.LoanPay.Tests.Integration.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mwi.LoanPay.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class LoanPayApiTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Client _loanPayClient;
        private readonly string _accessToken;

        public LoanPayApiTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var httpClient = new HttpClient();
            _loanPayClient = TestHelpers.GetTestClient(httpClient);
            _accessToken = TestHelpers.GetAccessToken(_loanPayClient);
        }

        [Fact]
        public void CalculateFee_GetsCardFee_WithCardRequest()
        {
            var actual = _loanPayClient.LoanPayApi.CalculateFeeAsync(_accessToken, new CardFeeRequest
            {
                BankNumber = TestHelpers.BankNumber,
                CompanyNumber = TestHelpers.CompanyNumber,
                Amount = 12.34m,
                CardNetwork = "VISN",
                IsDebit = true
            })
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (actual.IsError)
            {
                _testOutputHelper.WriteLine(actual.Error);
            }

            Assert.False(actual.IsError);
            Assert.Equal(4.65m, actual.FeeAmount);
        }

        [Fact]
        public void CalculateFee_GetsAchFee_WithCardRequest()
        {
            var actual = _loanPayClient.LoanPayApi.CalculateFeeAsync(_accessToken, new AchFeeRequest
            {
                BankNumber = TestHelpers.BankNumber,
                CompanyNumber = TestHelpers.CompanyNumber,
                Amount = 12.34m,
            })
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();


            if (actual.IsError)
            {
                _testOutputHelper.WriteLine(actual.Error);
            }

            Assert.False(actual.IsError);
            Assert.Equal(2.65m, actual.FeeAmount);
        }

        [Fact]
        public void SubmitPayment_ReturnsConfirmationNumber_WithValidCardPayment()
        {
            var tokenResponse = TestHelpers.GetCardToken(_accessToken, _loanPayClient);

            var cardPayment = new CardPaymentRequest
            {
                BankNumber = TestHelpers.BankNumber,
                CompanyNumber = TestHelpers.CompanyNumber,
                AccountNumber = "123-123",
                Amount = 12.34m,
                ConvenienceFee = 4.65m,
                PaymentMethod = new CardPaymentMethod
                {
                    CardToken = tokenResponse.PaymentInformationToken.Token,
                    CardholderName = "Test Name",
                    CardExpiration = "0222",
                    Cvv = "123",
                    CardMeta = new CardPaymentMethodMetadata
                    {
                        IsDebit = tokenResponse.PaymentInformationToken.Metadata.IsDebitCard,
                        Network = tokenResponse.PaymentInformationToken.Metadata.Network
                    }
                },
                Contact = new ContactInfo
                {
                    Name = "Test Name",
                    Phone = "(123) 456-7890",
                    Email = "test@email.com",
                    Address = "1039 3 Mile Rd NW",
                    City = "Walker",
                    State = "MI",
                    Zip = "49544"
                },
                ProcessDate = DateTime.Now,
                SessionTimeStamp = DateTime.Now
            };

            var actual = _loanPayClient.LoanPayApi.SubmitPaymentAsync(_accessToken, cardPayment, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (actual.IsError)
            {
                _testOutputHelper.WriteLine(actual.Error);
            }

            Assert.False(actual.IsError);

            // We have a confirmation number!
            Assert.NotEmpty(actual.Confirmation.ConfirmationNumber);
        }

        [Fact]
        public void SubmitPayment_ReturnsConfirmationNumber_WithValidAchPayment()
        {
            var accountTokenResponse = TestHelpers.GetAccountToken(_accessToken, _loanPayClient);
            var routingTokenResponse = TestHelpers.GetRoutingToken(_accessToken, _loanPayClient);

            var achPayment = new AchPaymentRequest
            {
                BankNumber = TestHelpers.BankNumber,
                CompanyNumber = TestHelpers.CompanyNumber,
                AccountNumber = "123-123",
                Amount = 12.34m,
                ConvenienceFee = 2.65m,
                PaymentMethod = new AchPaymentMethod
                {
                    AccountToken = accountTokenResponse.PaymentInformationToken.Token,
                    RoutingToken = routingTokenResponse.PaymentInformationToken.Token,
                    AccountType = BankAccountType.Checking
                },
                Contact = new ContactInfo
                {
                    Name = "Test Name",
                    Phone = "(123) 456-7890",
                    Email = "test@email.com",
                    Address = "1039 3 Mile Rd NW",
                    City = "Walker",
                    State = "MI",
                    Zip = "49544"
                },
                ProcessDate = DateTime.Now.AddDays(2),
                SessionTimeStamp = DateTime.Now
            };

            var actual = _loanPayClient.LoanPayApi.SubmitPaymentAsync(_accessToken, achPayment, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (actual.IsError)
            {
                _testOutputHelper.WriteLine(actual.Error);
            }

            Assert.False(actual.IsError);

            // We have a confirmation number!
            Assert.NotEmpty(actual.Confirmation.ConfirmationNumber);
        }

        [Fact]
        public void GetPaymentStatus_ReturnsPaymentStatusResponse_WhenValidCardPaymentExists()
        {
            var tokenResponse = TestHelpers.GetCardToken(_accessToken, _loanPayClient);

            var cardPayment = new CardPaymentRequest
            {
                BankNumber = TestHelpers.BankNumber,
                CompanyNumber = TestHelpers.CompanyNumber,
                AccountNumber = "123-123",
                Amount = 12.34m,
                ConvenienceFee = 4.65m,
                PaymentMethod = new CardPaymentMethod
                {
                    CardToken = tokenResponse.PaymentInformationToken.Token,
                    CardholderName = "Test Name",
                    CardExpiration = "0222",
                    Cvv = "123",
                    CardMeta = new CardPaymentMethodMetadata
                    {
                        IsDebit = tokenResponse.PaymentInformationToken.Metadata.IsDebitCard,
                        Network = tokenResponse.PaymentInformationToken.Metadata.Network
                    }
                },
                Contact = new ContactInfo
                {
                    Name = "Test Name",
                    Phone = "(123) 456-7890",
                    Email = "test@email.com",
                    Address = "1039 3 Mile Rd NW",
                    City = "Walker",
                    State = "MI",
                    Zip = "49544"
                },
                ProcessDate = DateTime.Now,
                SessionTimeStamp = DateTime.Now
            };

            var paymentResult = _loanPayClient.LoanPayApi.SubmitPaymentAsync(_accessToken, cardPayment, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var confirmationNumber = paymentResult.Confirmation.ConfirmationNumber;

            var actual = _loanPayClient.LoanPayApi.GetPaymentStatusAsync(_accessToken, confirmationNumber, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (actual.IsError)
            {
                _testOutputHelper.WriteLine(actual.Error);
            }


            Assert.False(actual.IsError);
            Assert.Equal(Status.Processed, actual.PaymentStatus.Status);
        }

        [Fact]
        public void CancelPayment_ReturnsSuccess_WhenPendingPaymentExists()
        {
            var tokenResponse = TestHelpers.GetCardToken(_accessToken, _loanPayClient);

            var cardPayment = new CardPaymentRequest
            {
                BankNumber = TestHelpers.BankNumber,
                CompanyNumber = TestHelpers.CompanyNumber,
                AccountNumber = "123-123",
                Amount = 12.34m,
                ConvenienceFee = 4.65m,
                PaymentMethod = new CardPaymentMethod
                {
                    CardToken = tokenResponse.PaymentInformationToken.Token,
                    CardholderName = "Test Name",
                    CardExpiration = "0222",
                    Cvv = "123",
                    CardMeta = new CardPaymentMethodMetadata
                    {
                        IsDebit = tokenResponse.PaymentInformationToken.Metadata.IsDebitCard,
                        Network = tokenResponse.PaymentInformationToken.Metadata.Network
                    }
                },
                Contact = new ContactInfo
                {
                    Name = "Test Name",
                    Phone = "(123) 456-7890",
                    Email = "test@email.com",
                    Address = "1039 3 Mile Rd NW",
                    City = "Walker",
                    State = "MI",
                    Zip = "49544"
                },
                ProcessDate = DateTime.Now.AddDays(2),
                SessionTimeStamp = DateTime.Now
            };

            var paymentResult = _loanPayClient.LoanPayApi.SubmitPaymentAsync(_accessToken, cardPayment, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var confirmationNumber = paymentResult.Confirmation.ConfirmationNumber;

            var actual = _loanPayClient.LoanPayApi.CancelPaymentAsync(_accessToken, confirmationNumber, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.False(actual.IsError);
        }

        [Fact]
        public void GetAccountDetails_ReturnSuccess_WhenBillingAccountExists()
        {
            var actual = _loanPayClient.LoanPayApi.GetAccountDetailsAsync(_accessToken, TestHelpers.BankNumber, TestHelpers.CompanyNumber, "123-123", CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.False(actual.IsError);
            Assert.Equal("123-123", actual.AccountDetails.AccountNumber);
        }

        [Fact]
        public void GetAccountDetailsByPrefix_ReturnSuccess_WhenBillingAccountsExists()
        {
            var actual = _loanPayClient.LoanPayApi
                .GetAccountDetailsByPrefixAsync(_accessToken, TestHelpers.BankNumber, TestHelpers.CompanyNumber, "123", CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.False(actual.IsError);
            Assert.True(actual.AccountDetailsList.Count > 1);
            Assert.Equal("123100", actual.AccountDetailsList[0].AccountNumber);
        }
    }
}
