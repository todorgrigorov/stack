namespace Stack
{
    public class JoinOptions
    {
        public JoinOptions(string path)
        {
            Assure.NotEmpty(path, nameof(path));
            Path = path;
        }

        public string Path { get; set; }

        public void Validate()
        {
            Assure.NotEmpty(Path, nameof(Path));
        }
    }
}