namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The card payment method's metadata, provided by the TokenApi
    /// </summary>
    public class CardPaymentMethodMetadata
    {
        /// <summary>
        /// The network of the card, returned from the TokenAPI
        /// </summary>
        public string Network { get; set; }
        /// <summary>
        /// Whether or not the card is a debit card, returned from the TokenAPI
        /// </summary>
        public bool IsDebit { get; set; }
    }
}