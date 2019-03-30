using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stack.Data.Persistence.EntityFramework
{
    public static class ModelBuilderExtensions
    {
        public static EntityTypeBuilder<T> For<T>(this ModelBuilder builder)
            where T : Entity
        {
            EntityTypeBuilder<T> result = builder.Entity<T>();
            result.HasKey(nameof(Entity.Id));
            result.Ignore(nameof(Entity.IsNew));
            result.WithColumn(nameof(Entity.Created));
            result.WithColumn(nameof(Entity.Updated));
            return result.ToTable(typeof(T).Name.ToCapitalCase());
        }
        public static EntityTypeBuilder<T> WithColumn<T>(this EntityTypeBuilder<T> builder, string name)
            where T : Entity
        {
            builder
                .Property(name)
                .HasColumnName(name.ToCapitalCase());
            return builder;
        }
        public static EntityTypeBuilder<T> WithOneToMany<T, TRelation>(
            this EntityTypeBuilder<T> builder,
            Expression<Func<T, TRelation>> relation,
            Expression<Func<TRelation, IEnumerable<T>>> collection = null,
            string foreignKey = null)
                where T : Entity
                where TRelation : Entity
        {
            string key = $"{nameof(TRelation)}Id";
            if (!string.IsNullOrEmpty(foreignKey))
            {
                key = foreignKey;
            }

            builder
                .HasOne(relation)
                .WithMany(collection)
                .HasForeignKey(key.ToCapitalCase());
            return builder;
        }
    }
}
