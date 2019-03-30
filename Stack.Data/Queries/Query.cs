using Stack.Queries;

namespace Stack.Data.Queries
{
    public class Query : IQuery
    {
        public Query(string sql, object parameters = null)
        {
            Assure.NotEmpty(sql, nameof(sql));

            Sql = sql;
            Parameters = parameters;
        }

        public string Sql { get; set; }
        public object Parameters { get; set; }
        public bool IsTerminated
        {
            get
            {
                return Sql.EndsWith(";");
            }
        }
        public bool IsParametrized
        {
            get
            {
                return Parameters != null;
            }
        }

        public static Query operator +(Query first, Query second)
        {
            object parameters = null;
            if (first.IsParametrized)
            {
                parameters = first.Parameters;
            }
            else if (second.IsParametrized)
            {
                parameters = second.Parameters;
            }
            return new Query(first.ToString() + second.ToString(), parameters);
        }

        public override string ToString()
        {
            return Terminate();
        }

        #region Private members
        private string Terminate()
        {
            if (!IsTerminated)
            {
                Sql += ";";
            }
            return Sql;
        } 
        #endregion
    }
}
