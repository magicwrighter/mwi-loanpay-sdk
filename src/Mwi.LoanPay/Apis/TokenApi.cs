using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Mwi.LoanPay.Models.Token;

namespace Mwi.LoanPay.Apis
{
    /// <summary>
    /// Used for securing payment information
    /// </summary>
    public interface ITokenApi
    {
        /// <summary>
        /// Request to secure payment information using MWI's Tokenization API. 
        /// </summary>
        /// <param name="accessToken">An access token from the IdentityApi</param>
        /// <param name="fiNumber">The institutions "FI" or "Bank" number</param>
        /// <param name="bcNumber">The institutions "BC" or "Company" number</param>
        /// <param name="request">The model containing the payment information to be secured</param>
        /// /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Response with either an error or a success result, but never both</returns>
        Task<TokenResponse> GetPaymentInformationTokenAsync(string accessToken, int fiNumber, int bcNumber, TokenRequest request, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc cref="ITokenApi"/>
    public class TokenApi : ITokenApi
    {
        private HttpClient HttpClient { get; }
        private IEnvironmentManager EnvironmentManager { get; }
        private JsonSerializerOptions SerializerSettings { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumMemberConverter(),
            }
        };

        /// <summary>
        /// Creates an instance of the TokenApi
        /// </summary>
        /// <param name="httpClient">Sets the HttpClient to be used by the TokenApi</param>
        /// <param name="environmentManager">Sets the environment to be used by the TokenApi</param>
        public TokenApi(HttpClient httpClient, IEnvironmentManager environmentManager)
        {
            HttpClient = httpClient;
            EnvironmentManager = environmentManager;
        }

        public async Task<TokenResponse> GetPaymentInformationTokenAsync(string accessToken, int fiNumber, int bcNumber, TokenRequest request, CancellationToken cancellationToken = default)
        {
            var stringBody = JsonSerializer.Serialize(request, SerializerSettings);
            var stringContent = new StringContent(stringBody, Encoding.UTF8, "application/json");

            var queryParams = "";
            if (request.Type == TokenizationType.Card)
            {
                queryParams = "?expand=metadata";
            }

            var builder = new UriBuilder(EnvironmentManager.TokenUrl);
            builder.Path += $"fis/{fiNumber}/bcs/{bcNumber}/tokens";
            builder.Query = queryParams;

            var url = builder.Uri;

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpRequest.Content = stringContent;

            var response = await HttpClient.SendAsync(httpRequest, cancellationToken)
                .ConfigureAwait(false);

            // Check for http status code failures
            if (!response.IsSuccessStatusCode)
            {
                // Dump the whole content of the response on the error message.
                var responseString = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return new TokenResponse
                {
                    Error = $"{response.StatusCode}\n{responseString}"
                };
            }

            // Read request content
            JsonDocument jsonDocument;
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                // Map to a json document for easy traversal. 
                try
                {
                    jsonDocument = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new TokenResponse
                    {
                        Error = $"Could not read the response body. {ex}"
                    };
                }
            }

            // Get the error field of the top level model
            if (!jsonDocument.RootElement.TryGetProperty("error", out var errorElement))
            {
                // Handle parse failure 
                return new TokenResponse
                {
                    Error = $"Could not read error.\n{jsonDocument.RootElement.GetRawText()}"
                };
            }

            // Check if there was an error. If so, attach it to the response model.
            if (errorElement.GetBoolean())
            {
                if (!jsonDocument.RootElement.TryGetProperty("message", out var errorMessage))
                {
                    // Handle parse failure 
                    return new TokenResponse
                    {
                        Error = $"Could not read the error response message.\n{jsonDocument.RootElement.GetRawText()}"
                    };
                }

                return new TokenResponse
                {
                    Error = errorMessage.GetString()
                };
            }

            // Get the metadata model, but only for card tokens
            if (request.Type == TokenizationType.Card)
            {
                // If metadata exists, try and parse it
                if (jsonDocument.RootElement.TryGetProperty("metadata", out var metadata))
                {
                    // Handle a case where metadata exists on the token object 
                    if (metadata.ValueKind == JsonValueKind.Object)
                    {
                        // Get the error field of the metadata model
                        if (!metadata.TryGetProperty("error", out var metadataError))
                        {
                            // Handle parse failure 
                            return new TokenResponse
                            {
                                Error = $"Could not read metadata error.\n{jsonDocument.RootElement.GetRawText()}"
                            };
                        }

                        // Check if the metadata model contained an error. If so, attach it to the response model
                        if (metadataError.ValueKind == JsonValueKind.True)
                        {
                            if (!metadata.TryGetProperty("message", out var errorMessage) || errorMessage.ValueKind != JsonValueKind.String)
                            {
                                // Handle parse failure
                                return new TokenResponse
                                {
                                    Error = $"Could not read metadata message.\n{jsonDocument.RootElement.GetRawText()}"
                                };
                            }

                            // Return metadata model inner error. The token request may have succeeded,
                            // but we are opting to avoid partial successes
                            return new TokenResponse
                            {
                                Error = errorMessage.GetString()
                            };
                        }
                    }
                }
            }

            var responseText = jsonDocument.RootElement.GetRawText();
            return new TokenResponse
            {
                PaymentInformationToken = JsonSerializer.Deserialize<PaymentInformationToken>(responseText, SerializerSettings)
            };
        }
    }
}