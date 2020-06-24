using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Assessment.Infrastructure
{
    public class ApiRestClient
    {
        private readonly string _baseUrl;
        public ApiRestClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<string> GetAsync(string endpoint)
        {
            var restClient = new HttpClient();
            var response = await restClient.GetStringAsync($"{_baseUrl}\\{endpoint}");
            return response;
        }
    }
}
