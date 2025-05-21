using ChainResource.Interfaces;

namespace ChainResource.Storages
{
    public class MemoryStorage<T> : IStorage<T>
    {
        private T? _value;
        private DateTime? _lastUpdated;
        private readonly TimeSpan _expirationTime;

        public MemoryStorage(TimeSpan? expirationTime = null)
        {
            _expirationTime = expirationTime ?? TimeSpan.FromHours(1);
        }

        public bool CanWrite => true;

        public Task<T?> TryGet()
        {
            return Task.FromResult(_value);
        }

        public Task Save(T value)
        {
            _value = value;
            _lastUpdated = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task<bool> IsExpired()
        {
            if (_lastUpdated == null || _value == null)
                return Task.FromResult(true);

            return Task.FromResult(DateTime.UtcNow - _lastUpdated.Value > _expirationTime);
        }
    }
}
