namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// Response model from the payment status request 
    /// </summary>
    public class PaymentStatusResponse
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
        /// The model for a successful current payment status. Will be null if there were errors
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }
    }
}