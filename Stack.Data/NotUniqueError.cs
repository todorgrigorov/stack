namespace Stack.Data
{
    public class NotUniqueError
    {
        public NotUniqueError(string message, string field)
        {
            Assure.NotNull(message, nameof(message));
            Assure.NotNull(field, nameof(field));
            Message = message;
            Field = field;
        }

        public string Message { get; set; }
        public string Field { get; set; }
    }
}
