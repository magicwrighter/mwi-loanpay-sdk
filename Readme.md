# MWI LoanPay SDK

A C# implementation for interacting with the LoanPay API.

## Making A Payment

Four steps are needed to successfully make a payment:

1. Authorization

   - Authenticate and get an access token.

2. Tokenization

   - Use your access token to secure the payment information.

3. Fee Calculation

   - Use the API to request the correct fee. Payments without the correct fee will be rejected.

4. Payment Submission

   - Combine all of the information and send off the payment.

## Client Setup

To begin, you need an instance of your client:

```cs
var httpClient = new HttpClient();
var environmentManager = new EnvironmentManager(Mwi.LoanPay.Environment.Sandbox);

var client = new Client(httpClient, environmentManager, "IdentityClientSecretGoesHere");
```

## 1. Access Token Retrieval

Once you have an instance of your client, you can authenticate using the credentials provided to you.

```cs
var accessTokenResponse = await client.IdentityApi.GetAccessTokenAsync(new IdentityRequest(143, "SandboxUser", "SandboxPassword"))
    .ConfigureAwait(false);
```

## 2. Payment Information Tokenization

Use your access token to secure payment information. ACH information requires two calls, one for account number and one for routing number.

```cs
var tokenResponse = await client.TokenApi.GetPaymentInformationTokenAsync(accessTokenResponse.Token.AccessToken, 5000, 3939, new TokenRequest
    {
        Id = "tracking_id",
        Type = TokenizationType.Card,
        Value = "5200828282828210" // A fake debit card
    })
    .ConfigureAwait(false);
```

## 3. Fee Request

Send a request to get the fee amount for a payment. Payments without the correct fee amounts will be rejected.

```cs
var feeResponse = await client.LoanPayApi.CalculateFeeAsync(accessTokenResponse.Token.AccessToken, new CardFeeRequest
{
    BankNumber = 5000,
    CompanyNumber = 3939,
    Amount = 12.34m,
    CardNetwork = "VISN",
    IsDebit = true
}).ConfigureAwait(false);
```

## 4. Payment Submission

Submit the payment and get a confirmation number.

### Card

```cs
var cardPayment = new CardPaymentRequest
{
    BankNumber = 5000,
    CompanyNumber = 3939,
    AccountNumber = "123-123",
    Amount = 12.34m,
    ConvenienceFee = feeResponse.FeeAmount,
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

var confirmationResponse = await client.LoanPayApi.SubmitPaymentAsync(accessTokenResponse.Token.AccessToken, cardPayment, CancellationToken.None)
    .ConfigureAwait(false);
```

### ACH

```cs
var achPayment = new AchPaymentRequest
{
    BankNumber = 5000,
    CompanyNumber = 3939,
    AccountNumber = "123-123",
    Amount = 12.34m,
    ConvenienceFee = feeResponse.FeeAmount,
    PaymentMethod = new AchPaymentMethod
    {
        AccountNumber = accountTokenResponse.PaymentInformationToken.Token,
        RoutingNumber = routingTokenResponse.PaymentInformationToken.Token,
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

var confirmationResponse = await client.LoanPayApi.SubmitPaymentAsync(accessTokenResponse.Token.AccessToken, achPayment, CancellationToken.None)
    .ConfigureAwait(false);
```
