namespace Mwi.LoanPay.Models.Token
{
    /// <summary>
    /// The body of the tokenization response
    /// </summary>
    public class PaymentInformationToken
    {
        /// <summary>
        /// An id that matches the one provided on the request
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The tokenized payment information
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// The type of payment information that was tokenized
        /// </summary>
        public TokenizationType Type { get; set; }
        /// <summary>
        /// A masked value for the payment information, usually the last four of a card
        /// </summary>
        public string AbbreviatedValue { get; set; }
        /// <summary>
        /// Card metadata information returned from MWI's Tokenization API
        /// </summary>
        public CardMetadata Metadata { get; set; }
    }
}