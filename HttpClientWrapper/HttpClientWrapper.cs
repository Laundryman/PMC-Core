using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace CoreSystem2024.HttpClientWrapper
{

    /// <summary>
    /// A generic wrapper class to REST API calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SecureHttpClient<T> : IDisposable
    {
        private HttpClient httpClient;
        private bool disposed = false;
        protected readonly string _baseAddress;
        private readonly string _addressSuffix;
        public SecureHttpClient(string baseAddress, string addressSuffix)
        {
            _baseAddress = baseAddress;
            _addressSuffix = addressSuffix;
            httpClient = CreateHttpClient(_baseAddress);
        }
        protected virtual HttpClient CreateHttpClient(string serviceBaseAddress)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serviceBaseAddress);
            return httpClient;
        }
        /// <summary>
        /// For getting the resources from a web api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <param name="accessToken"></param>
        /// <returns>A Task with result object of type T</returns>
        public async Task<T> Get(string accessToken)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            T result = default(T);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = httpClient.GetAsync(_addressSuffix).Result;

            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
            {
                if (x.IsFaulted)
                    throw x.Exception;
                result = JsonConvert.DeserializeObject<T>(x.Result);
            });

            return result;
        }


        /// <summary>
        /// For getting the resources from a web api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <param name="httpContent"></param>
        /// <returns>A Task with result object of type T</returns>
        public static async Task<T> Get(string url, string accessToken, IEnumerable<KeyValuePair<string, string>> httpContent)
        {

            T result = default(T);
            using (var httpClient = new HttpClient())
            {

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                if (httpContent != null)
                {
                    HttpContent content = new FormUrlEncodedContent(httpContent);
                    request.Content = content;
                }

                HttpResponseMessage response = await httpClient.SendAsync(request);


                //var response = httpClient.GetAsync(new Uri(url)).Result;

                response.EnsureSuccessStatusCode();
                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;
                    result = JsonConvert.DeserializeObject<T>(x.Result);
                });
            }

            return result;
        }

        /// <summary>
        /// For creating a new item over a web api using POST
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="accessToken"></param>
        /// <param name="postObject">The object to be created</param>
        /// <returns>A Task with created item</returns>
        public static async Task<T> PostRequest(string apiUrl, string accessToken, T postObject)
        {
            T result = default(T);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(apiUrl, postObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;
                    result = JsonConvert.DeserializeObject<T>(x.Result);
                });
            }

            return result;
        }

        /// <summary>
        /// For updating an existing item over a web api using PUT
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="putObject">The object to be edited</param>
        public static async Task PutRequest(string apiUrl, T putObject)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(apiUrl, putObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (httpClient != null)
                {
                    httpClient.Dispose();
                }
                disposed = true;
            }
        }

    }
}