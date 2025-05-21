using ChainResource.Interfaces;
using System.Text.Json;

namespace ChainResource.Storages
{
    public class FileSystemStorage<T> : IStorage<T>
    {
        private readonly string _filePath;
        private readonly TimeSpan _expirationTime;

        public FileSystemStorage(string fileName, TimeSpan? expirationTime = null)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            _expirationTime = expirationTime ?? TimeSpan.FromHours(4);
        }

        public bool CanWrite => true;

        public async Task<T?> TryGet()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return default;
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var data = JsonSerializer.Deserialize<T>(json);
                return data;
            }
            catch
            {
                return default;
            }
        }

        public async Task Save(T value)
        {
            var json = JsonSerializer.Serialize(value);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public Task<bool> IsExpired()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Task.FromResult(true);
                }

                var lastWriteTime = File.GetLastWriteTimeUtc(_filePath);
                var expired = DateTime.UtcNow - lastWriteTime > _expirationTime;

                return Task.FromResult(expired);
            }
            catch
            {
                return Task.FromResult(true);
            }
        }
    }
}
