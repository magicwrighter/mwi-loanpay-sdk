namespace Mwi.LoanPay.Models.Identity
{
    /// <summary>
    /// Request model to get an access token
    /// </summary>
    public class IdentityRequest
    {
        /// <summary>
        /// The consortium id assigned to your institution
        /// </summary>
        public int ConsortiumId { get; }
        /// <summary>
        /// The "user" identifier assigned to your institution
        /// </summary>
        public string Identifier { get; }
        /// <summary>
        /// The password tied to your user identifier
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Create an instance of an IdentityRequest
        /// </summary>
        /// <param name="consortiumId">The consortium id assigned to your institution</param>
        /// <param name="identifier">The "user" identifier assigned to your institution</param>
        /// <param name="password">The password tied to your user identifier</param>
        public IdentityRequest(int consortiumId, string identifier, string password)
        {
            ConsortiumId = consortiumId;
            Identifier = identifier;
            Password = password;
        }

        /// <summary>
        /// Assemble the composite username expected by MWI's Identity Server
        /// </summary>
        /// <returns></returns>
        public string GetCompositeUsername()
        {
            return $@"mpxa\{ConsortiumId}\{Identifier}";
        }
    }
}