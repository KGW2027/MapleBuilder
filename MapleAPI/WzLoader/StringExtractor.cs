using WzComparerR2.WzLib;

namespace MapleAPI.WzLoader;

public class StringExtractor : WzExtractor
{
    
    private Dictionary<string, Dictionary<string, string>> datas = new();
    private HashSet<string> prefixes = new();
    
    protected override string GetWzPath()
    {
        return $"{dataPath}/String/String.wz";
    }

    protected override bool IsTargetImage(string name)
    {
        return int.TryParse(name, out _);
    }

    private string MakeKey(string path)
    {
        int suffixImgIdx = path.IndexOf(".img", StringComparison.CurrentCulture);
        int sepIdx = path.Substring(0, suffixImgIdx).LastIndexOf('\\');
        int lastSepIdx = path.LastIndexOf('\\');

        string prefix = path.Substring(sepIdx + 1).Split(".")[0];
        string suffix = path.Substring(lastSepIdx + 1);
        prefixes.Add(prefix);
        
        return $"{prefix}_{suffix}".ToLower();
    }

    protected override void TryAddData(string path, Wz_Node node)
    {
        bool foundName = false;
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (Wz_Node child in node.Nodes)
        {
            switch (child.Value)
            {
                case string strValue:
                    if (child.Text.EndsWith(".atlas")) break;
                    data.Add(child.Text, ClearString(strValue));
                    foundName |= child.Text.Equals("name");
                    break;
            }
        }
        if (foundName)
            datas.Add(MakeKey(path), data);
    }

    public bool TryGetValue(string key, out Dictionary<string, string>? value)
    {
        return datas.TryGetValue(key, out value);
    }

    protected override void PostExtract()
    {
        Console.WriteLine($"Prefixes : {string.Join(", ", prefixes)}");
    }

}