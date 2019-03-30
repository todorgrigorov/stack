namespace Stack.Data.Queries
{
    public class ColumnResult
    {
        public ColumnResult(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Constraint { get; set; }

        public bool HasConstraint()
        {
            return !string.IsNullOrEmpty(Constraint);
        }

        public override string ToString()
        {
            string nullable = Required ? "NOT NULL" : "NULL";
            return $"{Name} {Type} {nullable}";
        }
    }
}
