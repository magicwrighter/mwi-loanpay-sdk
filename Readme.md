# MWI LoanPay SDK

A C# implementation for interacting with the LoanPay API.

## Making A Payment

In order to make a successful payment, there are three steps.

- Authorization
  - Authenticate and get an access token
- Tokenization
  - Use your access token to secure the payment information
- Calculating the fee
  - Use the API to get the correct fee. Payments without the correct fee will be rejected.
- Submit the payment
  - Combine all of the information and send off the payment

# Setting up your client

First you need an instance of your client.

```cs
var httpClient = new HttpClient();
var environmentManager = new EnvironmentManager(Environment.Sandbox);

var client = new Client(httpClient, environmentManager, "IdentityClientSecretGoesHere");
```

# Getting an Access Token

Once you have an instance of your client, you can authenticate using the credentials provided to you.

```cs
var accessTokenResponse = await client.IdentityApi.GetAccessTokenAsync(new IdentityRequest(143, "SandboxUser", "SandboxPassword"))
    .ConfigureAwait(false);
```

# Tokenizing Payment Information

Use your access token to secure payment information. For ACH information, there will need to be two calls: one for account number, and one for routing number.

```cs
var tokenResponse = await client.TokenApi.GetPaymentInformationTokenAsync(accessTokenResponse.Token.AccessToken, 5000, 3939, new TokenRequest
    {
        Id = "tracking_id",
        Type = TokenizationType.Card,
        Value = "5200828282828210" // A fake debit card
    })
    .ConfigureAwait(false);
```

# Requesting a Fee

Send a request to get what the fee will be for a payment. Payments without the correct fee amounts will be rejected.

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

# Submitting the Payment

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
