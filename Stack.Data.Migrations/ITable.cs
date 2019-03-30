namespace Stack.Data.Migrations
{
    public interface ITable
    {
        string Name { get; }

        void Create();
        void Drop();
        void CreateColumn(string column);
        void DropColumn(string column);
    }
}
