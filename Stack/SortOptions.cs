namespace Stack
{
    public class SortOptions
    {
        public SortOptions()
        {
        }
        public SortOptions(string field, SortOrder order = SortOrder.Ascending)
        {
            Order = order;
            Field = field;
        }

        public SortOrder Order { get; set; }
        public string Field
        {
            get
            {
                return field;
            }
            set
            {
                field = value;
            }
        }

        public void Validate()
        {
            Assure.NotEmpty(field, nameof(field));
        }

        #region Private members
        private string field;
        #endregion
    }
}
