using ChainResource.Interfaces;
using System.Text.Json;

namespace ChainResource.Storages
{
    public class WebServiceStorage<T> : IStorage<T>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string API_URL = "https://openexchangerates.org/api/latest.json";

        public WebServiceStorage(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = new HttpClient();
        }

        public bool CanWrite => false;

        public async Task<T?> TryGet()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{API_URL}?app_id={_apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonContent);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public Task Save(T value)
        {
            throw new NotSupportedException("WebServiceStorage is read-only");
        }

        public Task<bool> IsExpired()
        {
            return Task.FromResult(false);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
