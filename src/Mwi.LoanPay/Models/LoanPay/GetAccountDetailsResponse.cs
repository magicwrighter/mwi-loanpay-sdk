namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// Response model from the get LoanPay Account request 
    /// </summary>
    public class GetAccountDetailsResponse
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
        /// The model for a successful Get LoanPay Account request. Will be null if there were errors
        /// </summary>
        public AccountDetails AccountDetails { get; set; }
    }
}