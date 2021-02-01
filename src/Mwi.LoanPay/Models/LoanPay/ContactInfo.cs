namespace Mwi.LoanPay.Models.LoanPay
{
    /// <summary>
    /// The contact information of the person making a payment
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// The name of the person making a payment
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The phone number of the person making a payment
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// The email address of the person making a payment
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The street address of the person making a payment
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The city of the person making a payment
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// The state of the person making a payment
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// The zip code of the person making a payment
        /// </summary>
        public string Zip { get; set; }
    }
}