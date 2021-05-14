using System;

namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The LoanPay account detail response
    /// </summary>
    public class AccountDetails
    {
        /// <summary>
        /// The "Billing" or "Loan" Account Number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// The amount due next payment
        /// </summary>
        public decimal AmountDue { get; set; }
        /// <summary>
        /// The due date of the next payment
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// The maximum allowed ach payment amount
        /// </summary>
        public decimal MaximumAchPaymentAmount { get; set; }
        /// <summary>
        /// The minimum allowed ach payment amount
        /// </summary>
        public decimal MinimumAchPaymentAmount { get; set; }
        /// <summary>
        /// The maximum allowed card payment amount
        /// </summary>
        public decimal MaximumCardPaymentAmount { get; set; }
        /// <summary>
        /// The minimum allowed card payment amount
        /// </summary>
        public decimal MinimumCardPaymentAmount { get; set; }
    }
}