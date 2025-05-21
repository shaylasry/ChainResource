using ChainResource.Interfaces;

namespace ChainResource.Chain
{
    public class ChainResource<T> : IResourceChain<T>
    {
        private readonly List<IStorage<T>> _storages;
        public ChainResource(IEnumerable<IStorage<T>> storages) {
            _storages = storages?.ToList() ?? new List<IStorage<T>>();
            
            if (_storages.Count == 0) {
                throw new ArgumentException("At least one storage must be provided", nameof(storages));
            }
            
            for (int i = 0; i < _storages.Count - 1; i++) {
                if (!_storages[i].CanWrite) {
                    throw new ArgumentException($"Only the last storage should be ReadOnly. Storage at position {i} is ReadOnly but is not the last in the chain.");
                }
            }
        }

       public async Task<T?> GetValue() {
            T? value = default;
            int foundAtIndex = -1;

            for (int i = 0; i < _storages.Count; i++) {
                var storage = _storages[i];
                    
                if (!await storage.IsExpired()) {
                    value = await storage.TryGet();
                    if (value != null) {
                        foundAtIndex = i;
                        break;
                    }
                }
            }

            if (value != null && foundAtIndex > 0) {
                for (int j = 0; j < foundAtIndex; j++) {
                    if (_storages[j].CanWrite) {
                        await _storages[j].Save(value);
                    }
                }
            }

            return value;
        }
    }
}