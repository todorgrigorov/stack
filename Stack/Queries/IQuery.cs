namespace Stack.Queries
{
    public interface IQuery
    {
        string Sql { get; set; }
        object Parameters { get; set; }
        bool IsTerminated { get; }
        bool IsParametrized { get; }
    }
}
