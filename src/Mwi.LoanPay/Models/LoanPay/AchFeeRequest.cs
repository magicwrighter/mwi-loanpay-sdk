namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The model used to request an ACH fee amount
    /// </summary>
    public class AchFeeRequest
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
    }
}