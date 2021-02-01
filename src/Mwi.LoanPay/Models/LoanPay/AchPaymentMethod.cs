namespace Mwi.LoanPay.Models.LoanPay
{
    public enum BankAccountType
    {
        Unknown = 0,
        Checking = 27,
        Savings = 37
    }

    /// <summary>
    /// The ACH account to be charged for the payment amount
    /// </summary>
    public class AchPaymentMethod
    {
        /// <summary>
        /// The tokenized account number value returned from the token api
        /// </summary>  
        public string AccountToken { get; set; }
        /// <summary>
        /// The tokenized routing number value returned from the token api
        /// </summary>
        public string RoutingToken { get; set; }
        /// <summary>
        /// Account type: Checking or Savings
        /// </summary>
        public BankAccountType AccountType { get; set; }
    }
}