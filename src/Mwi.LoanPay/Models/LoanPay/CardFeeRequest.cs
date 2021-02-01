namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The model to request a card fee amount
    /// </summary>
    public class CardFeeRequest
    {
        /// <summary>
        /// The institutions "FI" or "Bank" number
        /// </summary>
        public int BankNumber { get; set; }
        /// <summary>
        /// The institutions "BC" or "Company" number
        /// </summary>
        public int CompanyNumber { get; set; }
        /// <summary>
        /// The amount the payment is charged for
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Card network comes from metadata property after sending a request to tokenize card payment information.
        /// </summary>
        public string CardNetwork { get; set; }
        /// <summary>
        /// IsDebit comes from metadata property after sending a request to tokenize card payment information.
        /// </summary>
        public bool IsDebit { get; set; }

    }
}