namespace MapleAPI.Web;

public class ArgBuilder : Dictionary<string, string>
{
    public ArgBuilder()
    {
        Clear();
    }

    public ArgBuilder AddArg(string key, string val)
    {
        if (!TryAdd(key, val)) this[key] = val;
        return this;
    }

    public ArgBuilder RemoveArg(string key)
    {
        if (ContainsKey(key)) Remove(key);
        return this;
    }

    public ArgBuilder ClearAndAdd(string key, string val)
    {
        Clear();
        return AddArg(key, val);
    }

    public override string ToString()
    {
        string context = "{";
        foreach (var pair in this) context += $"{pair.Key}: {pair.Value},";
        return context[..^1] + "}";
    }
    
}