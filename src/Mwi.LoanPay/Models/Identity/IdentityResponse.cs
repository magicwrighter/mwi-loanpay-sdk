namespace Mwi.LoanPay.Models.Identity
{
    /// <summary>
    /// Response model from the request to get an identity token
    /// </summary>
    public class IdentityResponse
    {
        /// <summary>
        /// Any error response will be written here. Will be null if there is none
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Will be true of the request returned an error in an expected format
        /// </summary>
        public bool IsError => !string.IsNullOrWhiteSpace(Error);
        /// <summary>
        /// The model for a successful access token request. Will be null if there were errors
        /// </summary>
        public IdentityToken Token { get; set; }
    }
}