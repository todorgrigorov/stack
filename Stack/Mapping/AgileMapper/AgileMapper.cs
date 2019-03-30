using AM = AgileObjects.AgileMapper;

namespace Stack.Mapping.AgileMapper
{
    public class AgileMapper : IMapper
    {
        public AgileMapper()
        {
            mapper = AM.Mapper.CreateNew();
            mapper.WhenMapping.MapNullCollectionsToNull();
        }

        public T Clone<T>(T value)
            where T : class
        {
            return mapper.Clone(value);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination target = null)
            where TSource : class
            where TDestination : class, new()
        {
            TDestination result = null;
            if (source != null)
            {
                if (target == null)
                {
                    result = mapper.Map(source).ToANew<TDestination>();
                }
                else
                {
                    result = mapper.Map(source).Over(target);
                }
            }
            return result;
        }

        #region Private members
        private AM.IMapper mapper;
        #endregion
    }
}
