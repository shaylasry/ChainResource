namespace ChainResource.Interfaces
{
    public interface IStorage<T>
    {
        Task<T?> GetValue();
        void Save(T value);
        bool CanWrite { get; }
        DateTime? LastUpdated { get; }
        TimeSpan Expiration { get; }
    }
}
