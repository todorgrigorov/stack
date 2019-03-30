using System.Text;

namespace Stack
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string text)
        {
            Assure.NotEmpty(text, nameof(text));
            char first = text[0];
            if (char.IsUpper(first))
            {
                text = text.Remove(0, 1);
                text = text.Insert(0, char.ToLower(first).ToString());
            }
            return text;
        }
        public static string ToCapitalCase(this string text)
        {
            Assure.NotEmpty(text, nameof(text));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char current = text[i];
                if (char.IsUpper(current) && i != 0)
                {
                    builder.Insert(i, "_");
                }
                builder.Append(char.ToUpper(current));
            }
            return builder.ToString();
        }
    }
}
