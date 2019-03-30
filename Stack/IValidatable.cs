namespace Stack
{
    public interface IValidatable
    {
        void Validate();
        ValidationError TryValidate();
    }
}
