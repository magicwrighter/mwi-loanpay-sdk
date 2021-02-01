using System;

namespace Mwi.LoanPay
{
    /// <summary>
    /// Sets the environment to Sandbox or Production.
    /// </summary>
    public enum Environment
    {
        /// <summary>
        /// Sandbox echos back pre-set responses, does not enforce valid credentials, and will not actually process payments.
        /// </summary>
        Sandbox = 0,
        /// <summary>
        /// Production will enforce valid credentials and requests, and will process payments.
        /// </summary>
        Production = 1
    }

    /// <summary>
    /// An interface to allow overriding of environment information.
    /// </summary>
    public interface IEnvironmentManager
    {
        /// <summary>
        /// The url used for authenticating a user.
        /// </summary>
        Uri IdentityUrl { get; }
        /// <summary>
        /// The url used to secure payment information.
        /// </summary>
        Uri TokenUrl { get; }
        /// <summary>
        /// The url used to interact with payments.
        /// </summary>
        Uri LoanPay { get; }
        /// <summary>
        /// The available environments
        /// </summary>
        Environment Env { get; }
    }

    /// <inheritdoc cref="IEnvironmentManager"/>
    public class EnvironmentManager : IEnvironmentManager
    {
        public Uri IdentityUrl => Env == Environment.Production
            ? new Uri("https://identity.magicwrighter.com/")
            : new Uri("https://loanpay.mwiapi.com/identity-sandbox/");

        public Uri TokenUrl => Env == Environment.Production
            ? new Uri("https://token.magicwrighter.com/api/v1/")
            : new Uri("https://loanpay.mwiapi.com/token-sandbox/api/v1/");

        public Uri LoanPay => Env == Environment.Production
            ? new Uri("https://loanpay.mwiapi.com/v1/")
            : new Uri("https://loanpay.mwiapi.com/loanpay-sandbox/v1/");

        public Environment Env { get; }

        /// <summary>
        /// Creates an instance of the environment manager.
        /// </summary>
        /// <param name="environment">Sets the environment to be used by the EnvironmentManager</param>
        public EnvironmentManager(Environment environment)
        {
            Env = environment;
        }
    }
}