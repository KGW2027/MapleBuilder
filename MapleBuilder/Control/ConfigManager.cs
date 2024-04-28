using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

namespace MapleBuilder.Control;

public class ConfigManager
{
    private static ConfigManager? _instance;

    public static ConfigManager Instance
    {
        get
        {
            return _instance ??= new ConfigManager();
        }
    }

    private const string CfgPath = "./config.json";
    private JsonObject config;

    public object? this[string key] => config.ContainsKey(key) ? config[key] : null;

    private ConfigManager()
    {
        if (!File.Exists(CfgPath))
        {
            CreateNewConfig();
        }

        string reader = File.ReadAllText(CfgPath);
        var node = JsonNode.Parse(reader);
        if (node is not JsonObject jo)
        {
            CreateNewConfig();
            reader = File.ReadAllText(CfgPath);
            config = JsonNode.Parse(reader)!.AsObject();
        }
        else config = jo;
    }

    private void CreateNewConfig()
    {
        if(File.Exists(CfgPath)) File.Delete(CfgPath);
        File.WriteAllText(CfgPath, "{}");
    }

    public void Bind(string key, string value)
    {
        config.TryAdd(key, "");
        config[key] = value;
        
        File.WriteAllText(CfgPath, System.Text.RegularExpressions.Regex.Unescape(config.ToString()));
    }
}