# Using MWI Modern Tokenization API

This document outlines how to utilize the tokenization API.

The api id used to retrieve access tokens for this API is `tkn`.

## Tokenizing Payment Information

You will need the following information that is provided by MWI:

- **access_token** : the access token retrieved from MWI's Identity Server.
- **fi_number** : MWI's number for your ODFI
- **bc_number** MWI's number for your processing entity

From your end user application:

No request sent for tokenization can exceed 1024 bytes in size.

- **POST**
  - **URL**
    - [https://token.magicwrighter.com/api/v1/fis/<fi_number>/bcs/<bc_number>/tokens](https://token.magicwrighter.com/api/v1/fis/%3Cfi_number%3E/bcs/%3Cbc_number%3E/tokens)
  - **Headers**
    - **authorization** : Bearer <access_token>
    - **content-type** : application/json
    - **cache-control** : no-cache
  - **Body (application/json)**
    - **id** : (optional) Used to identify the request. This value will be returned by the server in the response. This value is chosen by the sender and should be used to identify the corresponding response for the request that was just sent. This field can be of any length, but it needs to be small enough to allow for the request to fit within the request size limit.
    - **value** : The data to be tokenized. This field must contain a valid credit card number (up to 19 digits), bank account number (up to 17 characters), or routing number (9 digits).
    - **type** : the type of the data being provided to tokenize: cc, accountnumber, or routingnumber.
    - **frequency** : the frequency which this token is expected to be used: once, monthly, quarterly, semiannually, annually. The default value is once. _If a value is provided for a frequency requiring retention longer than you are authorized for you will receive an error._
    - Transaction information can optionally be provided with a tokenization request if you intend on redeeming the token as part of your user workflow (i.e., the user is making a real time payment). When a transactionId is provided you must include the cvv and/or the expDate.
      - **transactionId** : The globally unique id of the transaction to be processed. This can be used later to retrieve the cvv/expDate when the token is redeemed for payment. This feature is currently only supported for credit card requests.
      - **cvv** : (optional) The CVV value for the credit card associated with the transaction to be processed. A valid CVV will be either three or four digits.
      - **expDate** : (optional) The expiration date for the credit card associated with the transaction to be processed. The expiration date needs to be in the format of MMYY

### Credit Card Metadata

When requesting a token for a credit card, you are also able to request metadata about the provided credit card. Requesting credit card metadata will attach some basic information about the provided credit card. Some example requests and responses detailing the credit card metadata information have been included below. In order to request credit card metadata, a request needs to be posted to the Tokenization API with the URL [https://token.magicwrighter.com/api/v1/fis/<fi_number>/bcs/<bc_number>/tokens?expand=metadata](https://token.magicwrighter.com/api/v1/fis/%3Cfi_number%3E/bcs/%3Cbc_number%3E/tokens?expand=metadata).

# Samples

## Credit Card with CVV and Expiration Date

`POST /api/v1/fis/1111/bcs/2222/tokens`

### Request

```json
{
  "id": "test",
  "value": "5555555555554444",
  "type": "cc",
  "frequency": "once",
  "transactionId": "1234",
  "cvv": "123",
  "expDate": "1022"
}
```

### Response, Success

```json
{
  "id": "test",
  "token": "9932750561705702",
  "type": "cc",
  "frequency": "once",
  "error": false,
  "message": "",
  "abbreviatedValue": "4444"
}
```

### Response, Error

```json
{
  "id": "test",
  "token": "",
  "type": "cc",
  "frequency": "once",
  "error": true,
  "message": "A description of the problem",
  "abbreviatedValue": ""
}
```

### Response, Unauthorized Retention Error

```json
{
  "id": "test",
  "token": "",
  "type": "cc",
  "frequency": "annually",
  "error": true,
  "message": "The company associated with this request is not authorized for 'annual' retention.",
  "abbreviatedValue": ""
}
```

## Credit Card with Metadata

`POST /api/v1/fis/1111/bcs/2222/token?expand=metadata`

### Request

```json
{
  "id": "test",
  "value": "5555555555554444",
  "type": "cc",
  "frequency": "once"
}
```

### Response with Metadata

```json
{
  "id": "test",
  "token": "9524628308510227",
  "type": "cc",
  "frequency": "once",
  "metadata": {
    "error": false,
    "message": "",
    "isDebitCard": false,
    "isCreditCard": true,
    "isCommericalCard": true,
    "network": "MCRD"
  },
  "error": false,
  "message": "",
  "abbreviatedValue": "4444"
}
```

### Response with Metadata where Tokenization fails

```json
{
  "id": "test",
  "token": "",
  "type": "cc",
  "frequency": "once",
  "metadata": null,
  "error": true,
  "message": "Tokenization failed",
  "abbreviatedValue": "4444"
}
```

In the event that metadata is not found, a metadata object will be included, but it will indicate that an error occurred, along with a message that describes the problem.

### Response with Metadata Error

```json
{
  "id": "test",
  "token": "9916144551689391",
  "type": "cc",
  "frequency": "once",
  "metadata": {
    "error": true,
    "message": "Failed to retrieve metadata for card number ************4444"
  },
  "error": false,
  "message": "",
  "abbreviatedValue": "4444"
}
```

## Account Number Tokenization

`POST /api/v1/fis/1111/bcs/2222/tokens`

### Request

```json
{
  "id": "test",
  "value": "99999999",
  "type": "accountnumber",
  "frequency": "once"
}
```

### Response, Success

```json
{
  "id": "test",
  "token": "tI!234$$056%705*0",
  "type": "accountnumber",
  "frequency": "once",
  "metadata": null,
  "error": false,
  "message": "",
  "abbreviatedValue": "9999"
}
```

### Response, Error

```json
{
  "id": "test",
  "token": "",
  "type": "accountnumber",
  "frequency": "once",
  "metadata": null,
  "error": true,
  "message": "A description of the problem",
  "abbreviatedValue": "9999"
}
```

## Routing Number Tokenization

`POST /api/v1/fis/1111/bcs/2222/tokens`

### Request

```json
{
  "id": "test",
  "value": "123123123",
  "type": "routingnumber",
  "frequency": "once"
}
```

### Response, Success

```json
{
  "id": "test",
  "token": "912312312",
  "type": "routingnumber",
  "frequency": "once",
  "metadata": null,
  "error": false,
  "message": "",
  "abbreviatedValue": "3123"
}
```

### Response, Error

```json
{
  "id": "test",
  "token": "",
  "type": "routingnumber",
  "frequency": "once",
  "metadata": null,
  "error": true,
  "message": "A description of the problem",
  "abbreviatedValue": "3123"
}
```

## Tokenizing Multiple Values

Multiple tokenization requests of different types can be sent at the same time up to a limit of 3.

### Request

```json
[
  {
    "id": "test",
    "value": "123123123",
    "type": "routingnumber",
    "frequency": "once"
  },
  {
    "id": "test",
    "value": "99999999",
    "type": "accountnumber",
    "frequency": "once"
  },
  {
    "id": "test",
    "value": "5555555555554444",
    "type": "cc",
    "frequency": "once"
  }
]
```

### Response, Success

```json
[
  {
    "id": "test1",
    "token": "919371503",
    "type": "routingnumber",
    "frequency": "once",
    "metadata": null,
    "error": false,
    "message": "",
    "abbreviatedValue": "3123"
  },
  {
    "id": "test2",
    "token": "Ti41MHhLX3lLUgjbA",
    "type": "accountnumber",
    "frequency": "once",
    "metadata": null,
    "error": false,
    "message": "",
    "abbreviatedValue": "9999"
  },
  {
    "id": "test3",
    "token": "9930652113142941",
    "type": "cc",
    "frequency": "once",
    "metadata": null,
    "error": false,
    "message": "",
    "abbreviatedValue": "4444"
  }
]
```

### Response, Error(s)

When tokenizing multiple values, zero or more of the requests could fail and return with an error.

```json
[
  {
    "id": "test1",
    "token": "",
    "type": "routingnumber",
    "frequency": "once",
    "metadata": null,
    "error": true,
    "message": "A message describing the error",
    "abbreviatedValue": ""
  },
  {
    "id": "test2",
    "token": "Ti41MHhLX3lLUgjbA",
    "type": "accountnumber",
    "frequency": "once",
    "metadata": null,
    "error": false,
    "message": "",
    "abbreviatedValue": "9999"
  },
  {
    "id": "test3",
    "token": "9930652113142941",
    "type": "cc",
    "frequency": "once",
    "metadata": null,
    "error": false,
    "message": "",
    "abbreviatedValue": "4444"
  }
]
```
