// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KcHttpClient.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Keycloak http client
// </summary>

using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Teta.Packages.Auth.Configuration;
using Teta.Packages.Auth.Contracts;
using Teta.Packages.Auth.Interfaces;

namespace Teta.Packages.Auth.Implementation
{

    /// <summary>
    /// KC access client implementation
    /// </summary>
    public class KcAccessClient : IKcAccessClient
    {
        private readonly HttpClient _client;
        private readonly KcOptions _options;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="client">Http client for KC access</param>
        /// <param name="options">KC access options</param>

        public KcAccessClient(HttpClient client, IOptions<KcOptions> options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Auth response</returns>
        public async Task<KcAuthResponse> GetAccessTokenAsync(string username, string password)
        {
            var uri = new Uri(_client.BaseAddress, $"realms/{_options.Realm}/protocol/openid-connect/token");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("client_id", _options.ClientId),
                        new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
                    })
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await ResponseOrThrow(request, (jnode) =>
            {
                var result = new KcAuthResponse();

                result.AccessToken = jnode["access_token"].GetValue<string>();
                result.RefreshToken = jnode["refresh_token"].GetValue<string>();

                return result;
            });
        }

        /// <inheritdoc/>
        public async Task<KcAuthResponse> RefreshAccessToken(string refreshToken)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress, $"realms/{_options.Realm}/protocol/openid-connect/token"),
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("client_id", _options.ClientId),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
                }) 
            };

            return await ResponseOrThrow(request, (jnode) =>
            {
                var result = new KcAuthResponse();

                result.AccessToken = jnode["access_token"].GetValue<string>();
                result.RefreshToken = jnode["refresh_token"].GetValue<string>();

                return result;
            });
        }

        private async Task<T> ResponseOrThrow<T>(
            HttpRequestMessage request, 
            Func<JsonNode, T> jsonToResponse)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException(response.ReasonPhrase);
                    default: throw new Exception($"User autorization failed with code: {response.StatusCode} reason: {response.ReasonPhrase}");
                }
            }

            var jsonResp = JsonObject.Parse(responseContent);
            return jsonToResponse.Invoke(jsonResp);
        }
    }
}

