using System.Runtime.Serialization;

namespace Mwi.LoanPay.Models.Token
{
    /// <summary>
    /// The type of data being tokenized
    /// </summary>
    public enum TokenizationType
    {
        Unknown,
        [EnumMember(Value = "cc")]
        Card,
        [EnumMember(Value = "accountnumber")]
        AccountNumber,
        [EnumMember(Value = "routingnumber")]
        RoutingNumber
    }
}