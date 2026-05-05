using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace CampStore.UI
{
    public static class ApiServis
    {
        private static readonly string BaseUrl = "https://localhost:7146/api/";
        private static HttpClient _client;

        static ApiServis()
        {
            // .NET Framework 4.7.2 için SSL doğrulamasını atla
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                 | SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls;

            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        // ── GET ───────────────────────────────────────────────────────
        public static T Get<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = _client.GetAsync(endpoint).Result;
                string icerik = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(icerik);

                dynamic hata = JsonConvert.DeserializeObject<dynamic>(icerik);
                throw new Exception(hata?.mesaj?.ToString() ?? icerik);
            }
            catch (AggregateException aex)
            {
                throw new Exception(aex.InnerException?.Message ?? aex.Message);
            }
        }

        // ── POST ──────────────────────────────────────────────────────
        public static T Post<T>(string endpoint, object data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                var icerik = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(endpoint, icerik).Result;
                string sonuc = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(sonuc);

                dynamic hata = JsonConvert.DeserializeObject<dynamic>(sonuc);
                throw new Exception(hata?.mesaj?.ToString() ?? sonuc);
            }
            catch (AggregateException aex)
            {
                throw new Exception(aex.InnerException?.Message ?? aex.Message);
            }
        }

        // ── PUT ───────────────────────────────────────────────────────
        public static T Put<T>(string endpoint, object data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                var icerik = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PutAsync(endpoint, icerik).Result;
                string sonuc = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(sonuc);

                dynamic hata = JsonConvert.DeserializeObject<dynamic>(sonuc);
                throw new Exception(hata?.mesaj?.ToString() ?? sonuc);
            }
            catch (AggregateException aex)
            {
                throw new Exception(aex.InnerException?.Message ?? aex.Message);
            }
        }

        // ── DELETE ────────────────────────────────────────────────────
        public static T Delete<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync(endpoint).Result;
                string sonuc = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(sonuc);

                dynamic hata = JsonConvert.DeserializeObject<dynamic>(sonuc);
                throw new Exception(hata?.mesaj?.ToString() ?? sonuc);
            }
            catch (AggregateException aex)
            {
                throw new Exception(aex.InnerException?.Message ?? aex.Message);
            }
        }
    }
}