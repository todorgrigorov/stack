﻿namespace Stack.Web.Mvc
{
    public class InvalidIdentifierResult
    {
        public InvalidIdentifierResult(string message, int id)
        {
            Assure.NotNull(message, nameof(message));
            Assure.NotNull(id, nameof(id));
            Message = message;
            Id = id;
        }

        public string Message { get; set; }
        public int Id { get; set; }
    }
}
