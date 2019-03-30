namespace Stack.Mapping
{
    public interface IMapper
    {
        T Clone<T>(T value)
            where T : class;

        TDestination Map<TSource, TDestination>(TSource source, TDestination target = null)
            where TSource: class
            where TDestination : class, new();
    }
}