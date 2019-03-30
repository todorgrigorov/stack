namespace Stack.Persistence
{
    public interface IDialect
    {
        string AutoIncrement { get; }
        bool UsePrimaryKeyConstraints { get; }
        string Integer { get; }
        string Decimal { get; }
        string Boolean { get; }
        string Text { get; }
        string Date { get; }
        string Enum { get; }
    }
}
