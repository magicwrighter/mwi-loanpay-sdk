using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mwi.LoanPay.Tests.Unit.Helpers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public string Response { get; }
        public HttpStatusCode Status { get; }

        public MockHttpMessageHandler(string response, HttpStatusCode status)
        {
            Response = response;
            Status = status;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new HttpResponseMessage
            {
                StatusCode = Status,
                Content = new StringContent(Response)
            };
        }
    }
}
