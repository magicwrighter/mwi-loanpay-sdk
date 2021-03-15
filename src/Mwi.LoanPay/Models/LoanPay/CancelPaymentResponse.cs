namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// Response model from the cancel payment request. If there is no error request was successful
    /// </summary>
    public class CancelPaymentResponse
    {
        /// <summary>
        /// Any error response will be written here. Will be null if there is none
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Will be true if the request returned an error in an expected format. If there is no error request was successful
        /// </summary>
        public bool IsError => !string.IsNullOrWhiteSpace(Error);
    }
}