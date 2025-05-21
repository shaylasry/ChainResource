namespace ChainResource.Interfaces
{
    public interface IStorage<T>
    {
        Task<T?> TryGet();
        Task Save(T value);
        bool CanWrite { get; }
        Task<bool> IsExpired();
    }
}
