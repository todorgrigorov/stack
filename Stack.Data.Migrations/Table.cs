using Stack.Configuration;
using Stack.Data.Migrations.Queries;
using Stack.Data.Queries;
using Stack.Persistence;
using System;

namespace Stack.Data.Migrations
{
    public class Table : ITable
    {
        internal Table(Type type)
        {
            Assure.NotNull(type, nameof(type));
            EntityUtil.AssureEntity(type);

            name = type.Name.ToCapitalCase();
            builder = new DdlQueryBuilder(type);
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public static ITable For<T>()
            where T : Entity
        {
            return new Table(typeof(T));
        }
        public static ITable ForIntermediate<TFirst, TSecond, TIntermediate>()
            where TFirst : Entity
            where TSecond : Entity
        {
            return new IntermediateTable(typeof(TFirst), typeof(TSecond), typeof(TIntermediate));
        }

        public void Create()
        {
            TableUtil.AssureNotExisting(name);
            Query query = builder.CreateTable();
            Database.Persister.Execute(query);
            TableUtil.UpdateSchema();
        }
        public void Drop()
        {
            TableUtil.AssureExisting(name);
            Query query = builder.DropTable();
            Database.Persister.Execute(query);
            TableUtil.UpdateSchema();
        }
        public void CreateColumn(string column)
        {
            TableUtil.AssureExisting(name);
            TableUtil.AssureColumnsNotExisting(name, column);
            Query query = builder.CreateColumn(column);
            Database.Persister.Execute(query);
            TableUtil.UpdateSchema();
        }
        public void DropColumn(string column)
        {
            TableUtil.AssureExisting(name);
            TableUtil.AssureColumnExisting(name, column);
            Query query = builder.DropColumn(column);
            Database.Persister.Execute(query);
            TableUtil.UpdateSchema();
        }

        #region Private members
        private string name;
        private DdlQueryBuilder builder;
        #endregion
    }

    internal class IntermediateTable : ITable
    {
        internal IntermediateTable(Type first, Type second, Type intermediate)
        {
            Assure.NotNull(first, nameof(first));
            Assure.NotNull(second, nameof(second));
            Assure.NotNull(intermediate, nameof(intermediate));
            EntityUtil.AssureEntity(first);
            EntityUtil.AssureEntity(second);

            firstName = first.Name.ToCapitalCase();
            secondName = second.Name.ToCapitalCase();
            name = intermediate.Name.ToCapitalCase();

            builder = new DdlQueryBuilder(intermediate);
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public void Create()
        {
            // perform a series of checks before creating the "many-to-many" table
            // example: if we want to create a relationship table between COUNTRY and LANGUAGE:
            //          check for the existance of COUNTRY and LANGUAGE respectively
            //          check for the non existance of COUNTRY_LANGUAGE and LANGUAGE_COUNTRY (it depends which end was used for the creation)

            TableUtil.AssureExisting(firstName);
            TableUtil.AssureExisting(secondName);
            TableUtil.AssureNotExisting(name);
            TableUtil.AssureNotExisting($"{secondName}_{firstName}");

            Query query = builder.CreateTable();
            Database.Persister.Execute(query);
            TableUtil.UpdateSchema();
        }
        public void Drop()
        {
            TableUtil.AssureExisting(name);
            Query query = builder.DropTable();
            Database.Persister.Execute(query);
            TableUtil.UpdateSchema();
        }
        public void CreateColumn(string column)
        {
            throw new NotSupportedException();
        }
        public void DropColumn(string column)
        {
            throw new NotSupportedException();
        }

        #region Private members
        private Type BuildType(Type first, Type second)
        {
            throw new NotImplementedException();
        }

        private string name;
        private string firstName;
        private string secondName;
        private DdlQueryBuilder builder;
        #endregion
    }
}
