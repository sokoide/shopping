public class Globals
{
    public static string[] Flags = new string[]{
        "login","products", "checkout", "delivery",
    };

    public static int GetFlagId(string feature)
    {
        int id = 0;
        foreach (string name in Flags)
        {
            if (feature == name)
            {
                return id;
            }
            id++;
        }
        return -1;
    }
    public static AtomicBoolean[] BreakFlags = new AtomicBoolean[Flags.Length];

    public static void Init()
    {
        for (int i = 0; i < Flags.Length; i++)
        {
            BreakFlags[i] = new AtomicBoolean(false);
        }
    }
}
