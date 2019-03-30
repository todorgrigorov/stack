namespace Stack
{
    public class FilterModifier
    {
        public PageOptions Page { get; set; }
        public SortOptions[] Sort { get; set; }
        public JoinOptions[] Joins { get; set; }
    }
}
