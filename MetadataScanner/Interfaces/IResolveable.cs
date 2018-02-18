namespace MetadataScanner.Interfaces
{
    public interface IResolveable<TSource>
    {
        void ResolveLocal(TSource sourceType);

        void ResolveExternal(TSource sourceType);
    }
}
