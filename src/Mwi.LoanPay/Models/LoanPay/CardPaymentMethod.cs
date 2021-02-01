namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The card to be charged for the payment amount
    /// </summary>
    public class CardPaymentMethod
    {
        /// <summary>
        /// The secured value for a card, returned from the TokenApi
        /// </summary>
        public string CardToken { get; set; }

        /// <summary>
        /// The CVV value, usually printed on the back of the card
        /// </summary>
        public string Cvv { get; set; }

        /// <summary>
        /// The name appearing on the front of the card
        /// </summary>
        public string CardholderName { get; set; }

        /// <summary>
        /// The expiration date on the card, in MMYY
        /// </summary>
        public string CardExpiration { get; set; }

        /// <summary>
        /// The metadata for a card, returned from the TokenApi
        /// </summary>
        public CardPaymentMethodMetadata CardMeta { get; set; }
    }
}