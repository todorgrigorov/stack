using System.ComponentModel.DataAnnotations;

namespace Stack.Logging
{
    public class LogMessage : Entity
    {
        public const int MaxLogLength = 2000;

        [Required]
        [StringLength(MaxLogLength)]
        public string Message { get; set; }
        public LogType Type { get; set; }
    }
}
