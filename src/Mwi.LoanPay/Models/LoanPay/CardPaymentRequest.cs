using System;

namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The request model to submit a card payment to the LoanPayApi
    /// </summary>
    public class CardPaymentRequest
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
        /// The amount the payment is charged for
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The fee to be charged, returned from the fee calculation request
        /// </summary>
        public decimal ConvenienceFee { get; set; }
        /// <summary>
        /// The credit card payment method
        /// </summary>
        public CardPaymentMethod PaymentMethod { get; set; }
        /// <summary>
        /// The address of the cardholder
        /// </summary>
        public ContactInfo Contact { get; set; }
        /// <summary>
        /// The date the payment should process on.
        /// </summary>
        public DateTime ProcessDate { get; set; }
        /// <summary>
        /// A timestamp when the payment process was initiated.
        /// This is used, in combination with other factors, to prevent duplicate payments.
        /// </summary>
        public DateTime SessionTimeStamp { get; set; }
    }
}