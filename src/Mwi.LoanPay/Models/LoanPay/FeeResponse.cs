namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// Response model from the request to get an fee amount
    /// </summary>
    public class FeeResponse
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
        /// The fee amount to include on the payment request
        /// Will be zero if there were errors
        /// </summary>
        public decimal FeeAmount { get; set; }
    }
}