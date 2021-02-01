using System.Net.Http;
using Mwi.LoanPay.Apis;

namespace Mwi.LoanPay
{
    /// <summary>
    /// Multi-purpose client that provides all of the resources required to submit a payment
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Provides access to the IdentityApi, used to get an access token required to authenticate against the other Apis
        /// </summary>
        IIdentityApi IdentityApi { get; }

        /// <summary>
        /// Provides access to the LoanPayApi, used for interacting with payments
        /// </summary>
        ILoanPayApi LoanPayApi { get; }

        /// <summary>
        /// Provides access to the TokenApi, used to secure payment information
        /// </summary>
        ITokenApi TokenApi { get; }
    }

    /// <inheritdoc cref="IClient"/>
    public class Client : IClient
    {
        public IIdentityApi IdentityApi { get; }
        public ILoanPayApi LoanPayApi { get; }
        public ITokenApi TokenApi { get; }

        /// <summary>
        /// Creates an instance of the LoanPayClient
        /// </summary>
        /// <param name="httpClient">Sets the HttpClient to be used by the LoanPayClient</param>
        /// <param name="environmentManager">Sets the environment to be used by the LoanPayClient</param>
        /// <param name="identityClientSecret">Sets the Identity Client Secret to be used by the LoanPayClient</param>
        public Client(HttpClient httpClient, IEnvironmentManager environmentManager, string identityClientSecret)
        {
            IdentityApi = new IdentityApi(httpClient, environmentManager, identityClientSecret);
            LoanPayApi = new LoanPayApi(httpClient, environmentManager);
            TokenApi = new TokenApi(httpClient, environmentManager);
        }

        /// <summary>
        /// Creates an instance of the LoanPayClient using custom api implementations
        /// </summary>
        /// <param name="identityApi">IdentityApi to be used by the LoanPayClient</param>
        /// <param name="loanPayApi">LoanPayApi to be used by the LoanPayClient</param>
        /// <param name="tokenApi">TokenApi to be used by the LoanPayClient</param>
        public Client(IIdentityApi identityApi, ILoanPayApi loanPayApi, ITokenApi tokenApi)
        {
            IdentityApi = identityApi;
            LoanPayApi = loanPayApi;
            TokenApi = tokenApi;
        }
    }
}
