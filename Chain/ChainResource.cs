using ChainResource.Interfaces;

namespace ChainResource.Chain
{
    public class ChainResource<T>
    {
        private readonly IStorage<T>[] _storages;

        public ChainResource(IStorage<T>[] storages)
        {
            _storages = storages ?? throw new ArgumentNullException(nameof(storages));
            if (storages.Length == 0)
                throw new ArgumentException("At least one storage must be provided", nameof(storages));
        }

        public async Task<T?> GetValue()
        {
            T? value = default;
            int lastValidStorageIndex = -1;

            for (int i = 0; i < _storages.Length; i++)
            {
                var storage = _storages[i];
                if (!await storage.IsExpired())
                {
                    value = await storage.TryGet();
                    if (value != null)
                    {
                        lastValidStorageIndex = i;
                        break;
                    }
                }
            }

            if (value != null && lastValidStorageIndex > 0)
            {
                await PropagateValueUp(value, lastValidStorageIndex - 1);
            }

            return value;
        }

        private async Task PropagateValueUp(T value, int startIndex)
        {
            for (int i = startIndex; i >= 0; i--)
            {
                var storage = _storages[i];
                if (storage.CanWrite)
                {
                    await storage.Save(value);
                }
            }
        }
    }
}