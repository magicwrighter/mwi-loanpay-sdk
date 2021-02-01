namespace Mwi.LoanPay.Models.Token
{
    /// <summary>
    /// Response model from the request to secure payment information
    /// </summary>
    public class TokenResponse
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
        /// The model for a successful tokenization request. Will be null if there were errors
        /// </summary>
        public PaymentInformationToken PaymentInformationToken { get; set; }
    }
}