namespace ChainResource.Interfaces
{
    public interface IResourceChain<T>
    {
        Task<T?> GetValue();
    }
}
