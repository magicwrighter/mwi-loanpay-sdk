namespace Mwi.LoanPay.Models.Token
{
    /// <summary>
    /// The request model to tokenize 
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// An Id field where you can provide a custom value to more easily correlate your responses
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The value to be tokenized
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// How long until a token needs to be refreshed.
        /// </summary>
        public string Frequency { get; } = "annually";
        /// <summary>
        /// The type of data being tokenized
        /// </summary>
        public TokenizationType Type { get; set; }
    }
}