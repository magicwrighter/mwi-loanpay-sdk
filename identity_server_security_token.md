# Using MWI Identity Server for Auth / Access Token

This document outlines how to perform authentication for using MWI&#39;s Identity Server (IdSrv).

Generating an Access Token

The IdSrv is an _OpenID Connect_ server. In order to generate an access token that can be used for MWI API&#39;s you will use the _resource owner password credential_workflow.

You will need the following information that is provided by MWI:

- **client\_id** : an ID for your api integration
- **client\_secret** : a shared secret used to verify your client\_id.
- **access\_code** : the access code for the LBS operator
- **username** : the username for the LBS operator
- **security\_key** : the security key for the LBS operator
- **password** : the password for the LBS operator
- **api\_id** : the ID for the API you are requesting access to

Before you send your request you will need to create a  **client\_auth\_code** , which is the base64 encoded value of \&lt;client\_id\&gt;:\&lt;client\_secret\&gt;.

From your server:

- **POST**
  - **URL**
    - [https://identity.magicwrighter.com/connect/token](https://identity.magicwrighter.com/connect/token)
  - **Headers**
    - **authorization** : Basic \&lt;client\_auth\_code\&gt;
    - **content-type** : application/x-www-form-urlencoded
    - **cache-control** : no-cache
  - **Body (application/x-www-form-urlencoded)**
    - **grant\_type** : password
    - **username** : lbso\\&lt;access\_code\&gt;\\&lt;username\&gt;\\&lt;security\_key\&gt;
    - **password** : \&lt;password\&gt;
    - **scope** : \&lt;the api\_id you are requesting access to\&gt;

**Sample**

**Request**

curl --request POST \

--url https://identity.magicwrighter.com/connect/token \

-H &#39;authorization: Basic Y2xpZW50OnNlY3JldA==&#39; \

-H &#39;cache-control: no-cache&#39; \

-H &#39;content-type: application/x-www-form-urlencoded&#39; \

-d &#39;grant\_type=password&amp;username=lbso\11112222\testuser\testseckey \

&amp;password=testpassword&amp;scope=test\_api&#39;

**Response**

{

&quot;access\_token&quot;:&quot;eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ&quot;,

&quot;expires\_in&quot;:3600,

&quot;token\_type&quot;:&quot;Bearer&quot;

}
