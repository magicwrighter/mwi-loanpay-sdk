# Using MWI Identity Server for Auth / Access Token

This document outlines how to perform authentication for using MWI's Identity Server.

## Generating an Access Token

MWI's Identity Server is an _OpenID Connect_ server. In order to generate an access token that can be used for MWI API's you will use the _resource owner password credential_ workflow.

You will need the following information that is provided by MWI:

- **client_id** : An Id for your api integration.
- **client_secret** : A shared secret used to verify your client_id.
- **consortium_id** : The consortium id assigned to you by MWI.
- **username** : The user id provided by MWI.
- **password** : The password provided by MWI.

Before you send your request you will need to create a **client_auth_code** , which is the base64 encoded value of `<client_id>:<client_secret>`

From your server:

- **POST**
  - **URL**
    - [`https://identity.magicwrighter.com/connect/token`](https://identity.magicwrighter.com/connect/token)
  - **Headers**
    - **authorization** : `Basic <client_auth_code>`
    - **content-type** : `application/x-www-form-urlencoded`
    - **cache-control** : `no-cache`
  - **Body (application/x-www-form-urlencoded)**
    - **grant_type** : `password`
    - **username** : `mpxa\<consortium_id>\<user_identifier>`
    - **password** : `<password>`
    - **scope** : `mpx.payment tkn`

## Sample

### Request

```
curl --request POST \
 --url 'https://identity.magicwrighter.com/connect/token' \
 --header 'Authorization: Basic Y2xpZW50OnNlY3JldA==' \
 --header 'Content-Type: application/x-www-form-urlencoded' \
 --header 'cache-control: no-cache' \
 --data 'grant_type=password' \
 --data 'username=mpxa\143\SandboxUser' \
 --data 'password=SandboxPassword'
```

### Response

```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ",
  "expires_in": 3600,
  "token_type": "Bearer"
}
```
