using AppStore.Models;
using Jose;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using IdentityModel.Client;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using AppStore.Configuration;
using System.Threading.Tasks;
using AppStore.Models.Reponse;
using AppStore.Models.Request;
using Microsoft.AspNetCore.WebUtilities;

namespace AppStore
{
    public class AppStoreClient
    {
        private HttpClient _client;
        public AppStoreClient()
        {
            CreateHttpClient();
        }

        private void CreateHttpClient()
        {
            _client = new HttpClient();
        }

        public string GenerateToken() {
            var header = new Dictionary<string, object>()
                {
                    { "alg", "ES256" },
                    { "kid", ConfigHelper.Kid },
                    { "typ", "JWT" }
                };

            var scope = new string[2] { "GET /v1/apps?filter[platform]=IOS", "POST /v1/subscriptionGroups" };
            var a = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var b = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
            var payload = new Dictionary<string, object>
                {
                    { "iss", ConfigHelper.Iss },
                    { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                    { "exp", DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds() },
                    { "aud", "appstoreconnect-v1" },
                    //{ "scope", scope }
                };

            CngKey key = CngKey.Import(Convert.FromBase64String(ConfigHelper.PrivateKey), CngKeyBlobFormat.Pkcs8PrivateBlob);

            string token = Jose.JWT.Encode(payload, key, JwsAlgorithm.ES256, header);
            return token;
        }
        public async Task<CreateProductReponse> CreateSubcriptionGroup(CreateSubcriptionGroupRequest request)
        {
            EnsureHttpClientCreated();

            var requestContent = JsonConvert.SerializeObject(request);

            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsync($"{ConfigHelper.BaseUrl}/v1/subscriptionGroups", httpRequestMessage.Content);

            var responseAsString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<CreateProductReponse>(responseAsString);

            return result;
        }

        public async Task<CreateSubcriptionReponse> CreateSubcriptions(CreateSubcriptionRequest request)
        {
            EnsureHttpClientCreated();

            var requestContent = JsonConvert.SerializeObject(request);

            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsync($"{ConfigHelper.BaseUrl}/v1/subscriptions", httpRequestMessage.Content);

            var responseAsString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<CreateSubcriptionReponse>(responseAsString);

            return result;
        }

        public async Task<CreateSubcriptionReponse> CreateSubcriptionPrice(CreateSubcriptionPriceRequest request)
        {
            EnsureHttpClientCreated();

            var requestContent = JsonConvert.SerializeObject(request);

            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsync($"{ConfigHelper.BaseUrl}/v1/subscriptionPrices", httpRequestMessage.Content);

            var responseAsString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<CreateSubcriptionReponse>(responseAsString);

            return result;
        }

        public async Task<CreateProductReponse> GetApps(GetAppsRequest request)
        {
            EnsureHttpClientCreated();

            var requestContent = JsonConvert.SerializeObject(request);

            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.GetAsync($"{ConfigHelper.BaseUrl}/v1/apps" + request.data);

            var responseAsString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<CreateProductReponse>(responseAsString);

            return result;
        }
        public async Task<GetPricePointsReponse> GetPricePoint(GetPricePointRequest request)
        {
            EnsureHttpClientCreated();

            var requestContent = JsonConvert.SerializeObject(request);

            var httpRequestMessage = new HttpRequestMessage
            {
                //Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
            };
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.GetAsync($"{ConfigHelper.BaseUrl}/v1/subscriptions/{request.subcription_id}/pricePoints?fields[territories]=currency&filter[territory]=USA&include=territory&limit=801");
            var responseAsString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<GetPricePointsReponse>(responseAsString);

            return result;
        }
        public void SetToken(string token)
        {
            _client.SetBearerToken(token);
        }
        private void EnsureHttpClientCreated()
        {
            if (_client == null)
            {
                CreateHttpClient();
            }
        }
        public string GetSubscriptionPeriodByNumber(int number) {
            switch (number) { 
            
                case 1:
                    return "ONE_MONTH";
                case 2:
                    return "TWO_MONTHS";
                case 3:
                    return "THREE_MONTHS";
                case 6:
                    return "SIX_MONTHS";
                case 12:
                    return "ONE_YEAR";
            }
            return "";
        }
    }
}
