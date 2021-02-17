# Using MWI Modern Tokenization API

This document outlines how to utilize the tokenization API.

The api id used to retrieve access tokens for this API is tkn.

Tokenizing Payment Information

You will need the following information that is provided by MWI:

- **access\_token** : the access token retrieved from MWI&#39;s Identity Server.
- **fi\_number** : MWI&#39;s number for your ODFI
- **bc\_number**  MWI&#39;s number for your processing entity

From your end user application:

No request sent for tokenization can exceed 1024 bytes in size.

- **POST**
  - **URL**
    - [https://token.magicwrighter.com/api/v1/fis/\&lt;fi\_number\&gt;/bcs/\&lt;bc\_number\&gt;/tokens](https://token.magicwrighter.com/api/v1/fis/%3Cfi_number%3E/bcs/%3Cbc_number%3E/tokens)
  - **Headers**
    - **authorization** : Bearer \&lt;access\_token\&gt;
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

**Credit Card Metadata**

When requesting a token for a credit card, you are also able to request metadata about the provided credit card. Requesting credit card metadata will attach some basic information about the provided credit card. Some example requests and responses detailing the credit card metadata information have been included below. In order to request credit card metadata, a request needs to be posted to the Tokenization API with the URL [https://token.magicwrighter.com/api/v1/fis/\&lt;fi\_number\&gt;/bcs/\&lt;bc\_number\&gt;/tokens?expand=metadata](https://token.magicwrighter.com/api/v1/fis/%3Cfi_number%3E/bcs/%3Cbc_number%3E/tokens?expand=metadata).

Samples

**Credit Card with CVV and Expiration Date**

**Request**

POST /api/v1/fis/1111/bcs/2222/tokens

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;5555555555554444&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;transactionId&quot;: &quot;1234&quot;,

&quot;cvv&quot;: &quot;123&quot;,

&quot;expDate&quot;: &quot;1022&quot;

}

**Response, Success**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;9932750561705702&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;4444&quot;,

}

**Response, Error**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;error&quot;: true,

&quot;message&quot;: &quot;A description of the problem&quot;,

&quot;abbreviatedValue&quot;: &quot;&quot;,

}

**Response, Unauthorized Retention Error**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;annually&quot;,

&quot;error&quot;: true,

&quot;message&quot;: &quot;The company associated with this request is not authorized for &#39;annual&#39; retention.&quot;,

&quot;abbreviatedValue&quot;: &quot;&quot;,

}

**Credit Card with Metadata**

**Request**

POST /api/v1/fis/1111/bcs/2222/token?expand=metadata

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;5555555555554444&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;

}

**Response with Metadata**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;9524628308510227&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: {

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;isDebitCard&quot;: false,

&quot;isCreditCard&quot;: true,

&quot;isCommericalCard&quot;: true,

&quot;network&quot;: &quot;MCRD&quot;

},

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;4444&quot;

}

**Response with Metadata where Tokenization fails**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null

&quot;error&quot;: true,

&quot;message&quot;: &quot;Tokenization failed&quot;,

&quot;abbreviatedValue&quot;: &quot;4444&quot;

}

In the event that metadata is not found, a metadata object will be included, but it will indicate that an error occurred, along with a message that describes the problem.

**Response with Metadata Error**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;9916144551689391&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: {

&quot;error&quot;: true,

&quot;message&quot;: &quot;Failed to retrieve metadata for card number \*\*\*\*\*\*\*\*\*\*\*\*4444&quot;

},

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;4444&quot;

}

**Account Number Tokenization**

**Request**

POST /api/v1/fis/1111/bcs/2222/tokens

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;99999999&quot;,

&quot;type&quot;: &quot;accountnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;

}

**Response, Success**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;tI!234$$056%705\*0&quot;,

&quot;type&quot;: &quot;accountnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;9999&quot;,

}

**Response, Error**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;&quot;,

&quot;type&quot;: &quot;accountnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: true,

&quot;message&quot;: &quot;A description of the problem&quot;,

&quot;abbreviatedValue&quot;: &quot;9999&quot;,

}

**Routing Number Tokenization**

**Request**

POST /api/v1/fis/1111/bcs/2222/tokens

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;123123123&quot;,

&quot;type&quot;: &quot;routingnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;

}

**Response, Success**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;912312312&quot;,

&quot;type&quot;: &quot;routingnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;3123&quot;,

}

**Response, Error**

{

&quot;id&quot;: &quot;test&quot;,

&quot;token&quot;: &quot;&quot;,

&quot;type&quot;: &quot;routingnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: true,

&quot;message&quot;: &quot;A description of the problem&quot;

&quot;abbreviatedValue&quot;: &quot;3123&quot;,

}

**Tokenizing multiple values**

Multiple tokenization requests of different types can be sent at the same time up to a limit of 3.

**Request**

[

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;123123123&quot;,

&quot;type&quot;: &quot;routingnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;

},

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;99999999&quot;,

&quot;type&quot;: &quot;accountnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;

},

{

&quot;id&quot;: &quot;test&quot;,

&quot;value&quot;: &quot;5555555555554444&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;

}

]

**Response, Success**

[

{

&quot;id&quot;: &quot;test1&quot;,

&quot;token&quot;: &quot;919371503&quot;,

&quot;type&quot;: &quot;routingnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;3123&quot;

},

{

&quot;id&quot;: &quot;test2&quot;,

&quot;token&quot;: &quot;Ti41MHhLX3lLUgjbA&quot;,

&quot;type&quot;: &quot;accountnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;9999&quot;

},

{

&quot;id&quot;: &quot;test3&quot;,

&quot;token&quot;: &quot;9930652113142941&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;4444&quot;

}

]

**Response, Error(s)**

When tokenizing multiple values, zero or more of the requests could fail and return with an error.

[

{

&quot;id&quot;: &quot;test1&quot;,

&quot;token&quot;: &quot;&quot;,

&quot;type&quot;: &quot;routingnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: true,

&quot;message&quot;: &quot;A message describing the error&quot;,

&quot;abbreviatedValue&quot;: &quot;&quot;

},

{

&quot;id&quot;: &quot;test2&quot;,

&quot;token&quot;: &quot;Ti41MHhLX3lLUgjbA&quot;,

&quot;type&quot;: &quot;accountnumber&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;9999&quot;

},

{

&quot;id&quot;: &quot;test3&quot;,

&quot;token&quot;: &quot;9930652113142941&quot;,

&quot;type&quot;: &quot;cc&quot;,

&quot;frequency&quot;: &quot;once&quot;,

&quot;metadata&quot;: null,

&quot;error&quot;: false,

&quot;message&quot;: &quot;&quot;,

&quot;abbreviatedValue&quot;: &quot;4444&quot;

}

]
