public class AtomicBoolean
{
    private int _boolValue = 0; // 0 for false, 1 for true

    public AtomicBoolean(bool value = false)
    {
        if (value == false) _boolValue = 0;
        else _boolValue = 1;
    }

    // Get the boolean value atomically
    public bool Get()
    {
        return Interlocked.CompareExchange(ref _boolValue, 0, 0) == 1;
    }

    // Set the boolean value atomically
    public void Set(bool value)
    {
        Interlocked.Exchange(ref _boolValue, value ? 1 : 0);
    }

    // Compare and set the boolean value atomically
    public bool CompareAndSet(bool expected, bool newValue)
    {
        int expectedInt = expected ? 1 : 0;
        int newInt = newValue ? 1 : 0;
        return Interlocked.CompareExchange(ref _boolValue, newInt, expectedInt) == expectedInt;
    }
}
