using System;

namespace Stack
{
    public class PageOptions
    {
        public PageOptions()
        {
        }
        public PageOptions(int size, int index)
        {
            Size = size;
            Index = index;
        }

        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        public bool IsValid()
        {
            try
            {
                Validate();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void Validate()
        {
            Assure.IsGreater(size, 1, nameof(size));
            Assure.IsGreater(index, -1, nameof(index));
        }

        #region Private members
        private int size;
        private int index;
        #endregion
    }
}
