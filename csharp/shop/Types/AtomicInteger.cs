public class AtomicInteger
{
    private int _value;

    public AtomicInteger(int initialValue = 0)
    {
        _value = initialValue;
    }

    public int Get()
    {
        return Interlocked.CompareExchange(ref _value, 0, 0);
    }

    public void Set(int newValue)
    {
        Interlocked.Exchange(ref _value, newValue);
    }

    public int IncrementAndGet()
    {
        return Interlocked.Increment(ref _value);
    }

    public int DecrementAndGet()
    {
        return Interlocked.Decrement(ref _value);
    }

    public int GetAndIncrement()
    {
        return Interlocked.Increment(ref _value) - 1;
    }

    public int GetAndDecrement()
    {
        return Interlocked.Decrement(ref _value) + 1;
    }

    public int AddAndGet(int delta)
    {
        return Interlocked.Add(ref _value, delta);
    }

    public int GetAndAdd(int delta)
    {
        return Interlocked.Add(ref _value, delta) - delta;
    }

    public bool CompareAndSet(int expectedValue, int newValue)
    {
        return Interlocked.CompareExchange(ref _value, newValue, expectedValue) == expectedValue;
    }

    public override string ToString()
    {
        return Get().ToString();
    }
}
