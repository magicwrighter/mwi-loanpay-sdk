using IdentityModel.Client;

namespace Mwi.LoanPay.Models.Identity
{
    /// <summary>
    /// The model returned from the identitymodel client.
    /// Mapped to have a less ambiguous name
    /// </summary>
    public class IdentityToken
    {
        /// <summary>
        /// The access token to include on future requests
        /// </summary>
        public string AccessToken { get; }
        /// <summary>
        /// The approved scopes
        /// </summary>
        public string Scope { get; }
        /// <summary>
        /// The type of token
        /// In this case, it is usually "Bearer"
        /// </summary>
        public string TokenType { get; }
        /// <summary>
        /// Use this to extend your token lifetime
        /// </summary>
        public string RefreshToken { get; }
        /// <summary>
        /// The description of an error, if one exists
        /// </summary>
        public string ErrorDescription { get; }
        /// <summary>
        /// How long, in second, until your token expires
        /// </summary>
        public int ExpiresIn { get; }

        public IdentityToken(TokenResponse response)
        {
            AccessToken = response.AccessToken;
            Scope = response.Scope;
            TokenType = response.TokenType;
            RefreshToken = response.RefreshToken;
            ErrorDescription = response.ErrorDescription;
            ExpiresIn = response.ExpiresIn;
        }
    }
}