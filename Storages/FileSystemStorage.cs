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
                    return default;

                var json = await File.ReadAllTextAsync(_filePath);
                var data = JsonSerializer.Deserialize<(T Value, DateTime SavedAt)>(json);
                return data.Value;
            }
            catch
            {
                return default;
            }
        }

        public async Task Save(T value)
        {
            var data = (Value: value, SavedAt: DateTime.UtcNow);
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public Task<bool> IsExpired()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return Task.FromResult(true);

                var lastWriteTime = File.GetLastWriteTimeUtc(_filePath);
                return Task.FromResult(DateTime.UtcNow - lastWriteTime > _expirationTime);
            }
            catch
            {
                return Task.FromResult(true);
            }
        }
    }
}
