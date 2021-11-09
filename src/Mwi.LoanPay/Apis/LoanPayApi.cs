using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Mwi.LoanPay.Models.LoanPay;

namespace Mwi.LoanPay.Apis
{
    /// <summary>
    /// Used for interacting with payments
    /// </summary>
    public interface ILoanPayApi
    {
        /// <summary>
        /// Get a fee for a given ACH payment
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="achFeeRequest">The request model to ask the API what a given fee should be for a payment</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<FeeResponse> CalculateFeeAsync(string accessToken, AchFeeRequest achFeeRequest, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get a fee for a given card payment
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="cardFeeRequest">The request model to ask the API what a given fee should be for a payment</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<FeeResponse> CalculateFeeAsync(string accessToken, CardFeeRequest cardFeeRequest, CancellationToken cancellationToken = default);
        /// <summary>
        /// Submit an ACH payment to be processed
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="achPayment">The request model to submit an ach payment for processing</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<PaymentConfirmationResponse> SubmitPaymentAsync(string accessToken, AchPaymentRequest achPayment, CancellationToken cancellationToken = default);
        /// <summary>
        /// Submit a card payment to be processed
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="cardPayment">The request model to submit a card payment for processing</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<PaymentConfirmationResponse> SubmitPaymentAsync(string accessToken, CardPaymentRequest cardPayment, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get a payment status by confirmation number 
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="confirmationNumber">The confirmation number from a previous payment</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<PaymentStatusResponse> GetPaymentStatusAsync(string accessToken, string confirmationNumber, CancellationToken cancellationToken = default);
        /// <summary>
        /// Cancel a pending payment by confirmation number 
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="confirmationNumber">The confirmation number from a previous payment</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<CancelPaymentResponse> CancelPaymentAsync(string accessToken, string confirmationNumber, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get LoanPay account details by account number 
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="bankNumber">The LoanPay account bank number</param>
        /// <param name="companyNumber">The LoanPay account company number</param>
        /// <param name="accountNumber">The LoanPay account number</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<GetAccountDetailsResponse> GetAccountDetailsAsync(string accessToken, int bankNumber, int companyNumber, string accountNumber, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get list of LoanPay account details by account number prefix 
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="bankNumber">The LoanPay account bank number</param>
        /// <param name="companyNumber">The LoanPay account company number</param>
        /// <param name="accountPrefix">The LoanPay account number prefix</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<GetAccountDetailsByPrefixResponse> GetAccountDetailsByPrefixAsync(string accessToken, int bankNumber, int companyNumber, string accountPrefix, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc cref="ILoanPayApi"/>
    public class LoanPayApi : ILoanPayApi
    {
        private HttpClient HttpClient { get; }
        private readonly IEnvironmentManager _environmentManager;
        private JsonSerializerOptions SerializerSettings { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumMemberConverter(),
            }
        };

        /// <summary>
        /// Create an instance of the LoanPayApi
        /// </summary>
        /// <param name="httpClient">Sets the HttpClient to be used by the LoanPayApi</param>
        /// <param name="environmentManager">Sets the environment to be used by the LoanPayApi</param>
        public LoanPayApi(HttpClient httpClient, IEnvironmentManager environmentManager)
        {
            _environmentManager = environmentManager;
            HttpClient = httpClient;
        }

        public async Task<FeeResponse> CalculateFeeAsync(string accessToken, AchFeeRequest achFeeRequest,
            CancellationToken cancellationToken = default)
        {
            var requestBody = JsonSerializer.Serialize(achFeeRequest, SerializerSettings);

            return await SendFeeRequest(accessToken, requestBody, "fees/ach", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<FeeResponse> CalculateFeeAsync(string accessToken, CardFeeRequest cardFeeRequest,
            CancellationToken cancellationToken = default)
        {
            var requestBody = JsonSerializer.Serialize(cardFeeRequest, SerializerSettings);

            return await SendFeeRequest(accessToken, requestBody, "fees/card", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PaymentConfirmationResponse> SubmitPaymentAsync(string accessToken,
            AchPaymentRequest achPayment, CancellationToken cancellationToken = default)
        {
            var requestBody = JsonSerializer.Serialize(achPayment, SerializerSettings);

            return await SubmitPaymentRequest(accessToken, requestBody, "payments/ach", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PaymentConfirmationResponse> SubmitPaymentAsync(string accessToken,
            CardPaymentRequest cardPayment, CancellationToken cancellationToken = default)
        {
            var requestBody = JsonSerializer.Serialize(cardPayment, SerializerSettings);

            return await SubmitPaymentRequest(accessToken, requestBody, "payments/card", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string accessToken, string confirmationNumber,
            CancellationToken cancellationToken = default)
        {
            return await SendPaymentStatusRequest(accessToken, $"payments/{confirmationNumber}/status",
                cancellationToken);
        }

        public async Task<CancelPaymentResponse> CancelPaymentAsync(string accessToken, string confirmationNumber,
            CancellationToken cancellationToken = default)
        {
            return await CancelPaymentRequest(accessToken, $"payments/{confirmationNumber}",
                cancellationToken);
        }

        public async Task<GetAccountDetailsResponse> GetAccountDetailsAsync(string accessToken, int bankNumber,
            int companyNumber, string accountNumber, CancellationToken cancellationToken = default)
        {
            return await GetAccountDetailsRequest(accessToken,
                $"fis/{bankNumber}/bcs/{companyNumber}/accounts/{accountNumber}", cancellationToken);
        }

        public async Task<GetAccountDetailsByPrefixResponse> GetAccountDetailsByPrefixAsync(string accessToken, int bankNumber,
            int companyNumber, string accountPrefix, CancellationToken cancellationToken = default)
        {
            return await GetAccountDetailsByPrefixRequest(accessToken,
                $"fis/{bankNumber}/bcs/{companyNumber}/accounts?starts_with={accountPrefix}", cancellationToken);
        }

        private async Task<FeeResponse> SendFeeRequest(string accessToken, string requestBody, string url,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_environmentManager.LoanPay, url)))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                // Attach body
                using (var stringContent = new StringContent(requestBody, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    response = await HttpClient
                        .SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            // Check for failed status codes. We want to parse the error model returned on bad requests, so we skip those.
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new FeeResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };   
            }

            JsonDocument jsonDocument;
            // Read request content
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                try
                {
                    // Map to a json document for easy traversal
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new FeeResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }

            }

            // Get the isError field of the top level model
            if (jsonDocument.RootElement.TryGetProperty("isError", out var isErrorElement))
            {
                // If there was an error, read them
                if (isErrorElement.GetBoolean())
                {
                    if (!jsonDocument.RootElement.TryGetProperty("errors", out var errorMessage))
                    {
                        // Handle parse failure 
                        return new FeeResponse
                        {
                            Error = $"Could not read the response body.\n{jsonDocument.RootElement.GetRawText()}"
                        };
                    }

                    return new FeeResponse
                    {
                        Error = errorMessage.GetRawText()
                    };
                }
            }

            //If there was not an "isError" field, assume the request succeeded.
            var responseText = jsonDocument.RootElement.GetRawText();
            var successResponse = JsonSerializer.Deserialize<FeeResponse>(responseText, SerializerSettings);
            return successResponse;
        }

        private async Task<PaymentConfirmationResponse> SubmitPaymentRequest(string accessToken, string requestBody,
            string url, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_environmentManager.LoanPay, url)))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var stringContent = new StringContent(requestBody, Encoding.UTF8, "application/json"))
                {
                    // Attach body
                    request.Content = stringContent;

                    response = await HttpClient
                        .SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            // Check for failed status codes. We want to parse the error model returned on bad requests, so we skip those.
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new PaymentConfirmationResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };
            }

            // Read request content
            JsonDocument jsonDocument;
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                try
                {
                    // Map to a json document for easy traversal
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new PaymentConfirmationResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }
            }

            // Get the isError field of the top level model
            if (jsonDocument.RootElement.TryGetProperty("isError", out var isErrorElement))
            {
                // If there was an error, read them
                if (isErrorElement.GetBoolean())
                {
                    if (!jsonDocument.RootElement.TryGetProperty("errors", out var errorMessage))
                    {
                        // Handle parse failure 
                        return new PaymentConfirmationResponse
                        {
                            Error = $"Could not read the response body.\n{jsonDocument.RootElement.GetRawText()}"
                        };
                    }

                    return new PaymentConfirmationResponse
                    {
                        Error = errorMessage.GetRawText()
                    };
                }
            }

            var responseText = jsonDocument.RootElement.GetRawText();
            return new PaymentConfirmationResponse
            {
                Confirmation = JsonSerializer.Deserialize<PaymentConfirmation>(responseText, SerializerSettings)
            };
        }

        private async Task<PaymentStatusResponse> SendPaymentStatusRequest(string accessToken, string url,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_environmentManager.LoanPay, url)))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                response = await HttpClient
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            }

            // Check for failed status codes. We want to parse the error model returned on bad requests, so we skip those.
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new PaymentStatusResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };
            }

            // Read request content
            JsonDocument jsonDocument;
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                try
                {
                    // Map to a json document for easy traversal
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new PaymentStatusResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }
            }

            // Get the isError field of the top level model
            if (jsonDocument.RootElement.TryGetProperty("isError", out var isErrorElement))
            {
                // If there was an error, read them
                if (isErrorElement.GetBoolean())
                {
                    if (!jsonDocument.RootElement.TryGetProperty("errors", out var errorMessage))
                    {
                        // Handle parse failure 
                        return new PaymentStatusResponse
                        {
                            Error = $"Could not read the response body.\n{jsonDocument.RootElement.GetRawText()}"
                        };
                    }

                    return new PaymentStatusResponse
                    {
                        Error = errorMessage.GetRawText()
                    };
                }
            }

            var responseText = jsonDocument.RootElement.GetRawText();
            //If there was not an "isError" field, assume the request succeeded.
            return new PaymentStatusResponse
            {
                PaymentStatus = JsonSerializer.Deserialize<PaymentStatus>(responseText, SerializerSettings)
            };
        }

        private async Task<CancelPaymentResponse> CancelPaymentRequest(string accessToken, string url,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_environmentManager.LoanPay, url)))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                response = await HttpClient
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            }

            // Check for failed status codes. We want to parse the error model returned on bad requests, so we skip those.
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new CancelPaymentResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };
            }

            // Cancelling a pending payment returns an empty body if it succeeds, so we shouldn't try and parse anything and just exit.
            if (response.IsSuccessStatusCode)
            {
                return new CancelPaymentResponse();
            }

            // Read request content
            JsonDocument jsonDocument;
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                try
                {
                    // Map to a json document for easy traversal
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new CancelPaymentResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }
            }

            // Get the isError field of the top level model
            if (jsonDocument.RootElement.TryGetProperty("isError", out var isErrorElement))
            {
                // If there was an error, read them
                if (isErrorElement.GetBoolean())
                {
                    if (!jsonDocument.RootElement.TryGetProperty("errors", out var errorMessage))
                    {
                        // Handle parse failure 
                        return new CancelPaymentResponse
                        {
                            Error = $"Could not read the response body.\n{jsonDocument.RootElement.GetRawText()}"
                        };
                    }

                    return new CancelPaymentResponse
                    {
                        Error = errorMessage.GetRawText()
                    };
                }
            }

            //If there was not an "isError" field, assume the request succeeded.
            return new CancelPaymentResponse();
        }

        private async Task<GetAccountDetailsResponse> GetAccountDetailsRequest(string accessToken, string url,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_environmentManager.LoanPay, url)))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                response = await HttpClient
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            }

            // Check for failed status codes. We want to parse the error model returned on bad requests, so we skip those.
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new GetAccountDetailsResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };
            }

            // Read request content
            JsonDocument jsonDocument;
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                try
                {
                    // Map to a json document for easy traversal
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new GetAccountDetailsResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }
            }

            // Get the isError field of the top level model
            if (jsonDocument.RootElement.TryGetProperty("isError", out var isErrorElement))
            {
                // If there was an error, read them
                if (isErrorElement.GetBoolean())
                {
                    if (!jsonDocument.RootElement.TryGetProperty("errors", out var errorMessage))
                    {
                        // Handle parse failure 
                        return new GetAccountDetailsResponse
                        {
                            Error = $"Could not read the response body.\n{jsonDocument.RootElement.GetRawText()}"
                        };
                    }

                    return new GetAccountDetailsResponse
                    {
                        Error = errorMessage.GetRawText()
                    };
                }
            }

            //If there was not an "isError" field, assume the request succeeded.
            var responseText = jsonDocument.RootElement.GetRawText();
            return new GetAccountDetailsResponse
            {
                AccountDetails = JsonSerializer.Deserialize<AccountDetails>(responseText, SerializerSettings)
            };
        }

        private async Task<GetAccountDetailsByPrefixResponse> GetAccountDetailsByPrefixRequest(string accessToken, string url,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_environmentManager.LoanPay, url)))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                response = await HttpClient
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            }

            // Check for failed status codes. We want to parse the error model returned on bad requests, so we skip those.
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new GetAccountDetailsByPrefixResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };
            }

            // Read request content
            JsonDocument jsonDocument;
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                try
                {
                    // Map to a json document for easy traversal
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new GetAccountDetailsByPrefixResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }
            }

            // Get the isError field of the top level model
            // The array check is for mountebank testing
            if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array && jsonDocument.RootElement.TryGetProperty("isError", out var isErrorElement))
            {
                // If there was an error, read them
                if (isErrorElement.GetBoolean())
                {
                    if (!jsonDocument.RootElement.TryGetProperty("errors", out var errorMessage))
                    {
                        // Handle parse failure 
                        return new GetAccountDetailsByPrefixResponse
                        {
                            Error = $"Could not read the response body.\n{jsonDocument.RootElement.GetRawText()}"
                        };
                    }

                    return new GetAccountDetailsByPrefixResponse
                    {
                        Error = errorMessage.GetRawText()
                    };
                }
            }

            //If there was not an "isError" field, assume the request succeeded.
            var responseText = jsonDocument.RootElement.GetRawText();
            return new GetAccountDetailsByPrefixResponse
            {
                AccountDetailsList = JsonSerializer.Deserialize<List<AccountDetails>>(responseText, SerializerSettings)
            };
        }
    }
}