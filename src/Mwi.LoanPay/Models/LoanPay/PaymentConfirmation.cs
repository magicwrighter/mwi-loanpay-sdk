using System;

namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The payment confirmation response after a successful payment
    /// </summary>
    public class PaymentConfirmation
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
        /// The "Billing" or "Loan" Account Number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// The amount the payment is to be processed for
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The fee to be charged, returned from the fee calculation request
        /// </summary>
        public decimal ConvenienceFee { get; set; }
        /// <summary>
        /// The date the payment should process on.
        /// </summary>
        public DateTime ProcessDate { get; set; }
        /// <summary>
        /// The name of the person making a payment
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The confirmation number of a successful payment
        /// Store this, to look up or cancel a payment
        /// </summary>
        public string ConfirmationNumber { get; set; }
    }
}