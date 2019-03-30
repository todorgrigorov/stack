using Microsoft.EntityFrameworkCore;

namespace Stack.Data.Persistence.EntityFramework
{
    public interface IEntityFrameworkContextBuilder
    {
        void Build(ModelBuilder builder);
    }
}
