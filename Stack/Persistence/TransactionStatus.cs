namespace Stack.Persistence
{
    public enum TransactionStatus
    {
        NotStarted,
        Begun,
        Committed,
        Rollbacked,
        Ended
    }
}
