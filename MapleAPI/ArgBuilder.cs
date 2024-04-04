namespace MapleAPI;

public class ArgBuilder : Dictionary<string, string>
{
    public ArgBuilder()
    {
        Clear();
    }

    public ArgBuilder AddArg(string key, string val)
    {
        Add(key, val);
        return this;
    }

    public ArgBuilder ClearAndAdd(string key, string val)
    {
        Clear();
        return AddArg(key, val);
    }
    
}