namespace Stack
{
    public class ValidationError
    {
        public ValidationError(string message, string field)
        {
            Assure.NotNull(message, nameof(message));
            Assure.NotNull(field, nameof(field));
            Message = message;
            Field = field;
        }

        public string Message { get; set; }
        public string Field { get; set; }

        public static ValidationError Empty
        {
            get
            {
                return new ValidationError(null, null);
            }
        }
    }
}
