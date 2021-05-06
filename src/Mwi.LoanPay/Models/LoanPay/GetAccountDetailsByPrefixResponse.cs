using System.Collections.Generic;

namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// Response model from the get LoanPay Accounts by prefix request 
    /// </summary>
    public class GetAccountDetailsByPrefixResponse
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
        /// The model list for a successful Get LoanPay Accounts by prefix request. Will be null if there were errors
        /// </summary>
        public List<AccountDetails> AccountDetailsList { get; set; }
    }
}