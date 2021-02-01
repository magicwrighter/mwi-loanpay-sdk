namespace Mwi.LoanPay.Models.Token
{
    /// <summary>
    /// Card metadata information returned from MWI's Tokenization API
    /// </summary>
    public class CardMetadata
    {
        /// <summary>
        ///  If the provided card was a debit card
        /// </summary>
        public bool IsDebitCard { get; set; }
        /// <summary>
        /// If the provided card is a credit card
        /// </summary>
        public bool IsCreditCard { get; set; }
        /// <summary>
        /// If the provided card is a commercial card
        /// </summary>
        public bool IsCommericalCard { get; set; }
        /// <summary>
        /// What card network the provided card belongs to
        /// </summary>
        public string Network { get; set; }
    }
}