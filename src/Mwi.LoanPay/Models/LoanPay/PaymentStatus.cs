using System;

namespace Mwi.LoanPay.Models.LoanPay
{
    public enum Status
    {
        Unknown,
        Errored,
        Deleted,
        Pending,
        Processed,
        Declined
    }

    /// <summary>
    /// The payment status response
    /// </summary>
    public class PaymentStatus
    {
        /// <summary>
        /// The "Billing" or "Loan" Account Number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// The institutions "FI" or "Bank" number
        /// </summary>
        public int BankNumber { get; set; }
        /// <summary>
        /// The institutions "BC" or "Company" number
        /// </summary>
        public int CompanyNumber { get; set; }
        /// <summary>
        /// The amount the payment is to be processed for
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The fee to be charged, returned from the fee calculation request
        /// </summary>
        public decimal ConvenienceFee { get; set; }
        /// <summary>
        /// The date the payment should post on.
        /// </summary>
        public DateTime PostDate { get; set; }
        /// <summary>
        /// The confirmation number of a successful payment
        /// Store this to look up or cancel a payment
        /// </summary>
        public string ConfirmationNumber { get; set; }
        /// <summary>
        /// The status of a payment.
        /// Can be Unknown, Errored, Deleted, Pending, Processed, Declined.
        /// </summary>
        public Status Status { get; set; }
    }
}