namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// Response model from the request to submit a payment
    /// </summary>
    public class PaymentConfirmationResponse
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
        /// The model for a successful payment submission. Will be null if there were errors
        /// </summary>
        public PaymentConfirmation Confirmation { get; set; }
    }
}