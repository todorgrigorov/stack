using System.Text;

namespace Stack.Data.Migrations.Queries
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder TrimComma(this StringBuilder builder)
        {
            Assure.NotNull(builder, nameof(builder));
            if (builder.ToString().EndsWith(","))
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return builder;
        }
    }
}
