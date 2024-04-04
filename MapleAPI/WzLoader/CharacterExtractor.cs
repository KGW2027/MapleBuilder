using System.Text.Json.Nodes;
using WzComparerR2.WzLib;

namespace MapleAPI.WzLoader;

public class CharacterExtractor : WzExtractor
{
    private JsonObject jo = new();
    
    protected override string GetWzPath()
    {
        return $@"{dataPath}\Character\Character.wz";
    }
    
    private void RecursiveReadCharacterData(string path, Wz_Node parent, JsonObject jsonObject)
    {
        foreach (Wz_Node node in parent.Nodes)
        {
            switch (node.Value)
            {
                case string strValue:
                    if (node.Text.EndsWith(".atlas") || node.Text.Equals("memo")) break;
                    jsonObject.Add(node.Text, ClearString(strValue));
                    break;
                case Int32 number:
                    jsonObject.Add(node.Text, number);
                    break;
                case Wz_Vector vec:
                    JsonArray data = new JsonArray
                    {
                        vec.X,
                        vec.Y
                    };
                    jsonObject.Add(node.Text, data);
                    break;
                case Wz_Png png:
                {
                    if (png.WzFile.Node.Text.Contains("_Canvas"))
                    {
                        string imageName = node.Text;
                        if (!imageName.Equals("iconRaw")) break;

                        string fileName = node.Text;
                        if (int.TryParse(parent.ParentNode.Text.Substring(0, parent.ParentNode.Text.IndexOf('.')), out var id)
                            && sEx!.TryGetValue($"eqp_{id}", out var dict)
                            && dict!.ContainsKey("name"))
                        {
                            fileName = dict["name"];
                        }
                        
                        string folder = path.Split(".wz")[0] + ".wz";
                        jsonObject.Add(node.Text, ExportPng(png, folder, fileName).Replace("\\", "/"));
                    }
                    break;
                }
                case Wz_Uol:
                    break;
                default:
                    JsonObject childObject = new JsonObject();
                    RecursiveReadCharacterData($"{path}\\{node.Text}", node, childObject);
                    if (childObject.Count == 0) break;
                    jsonObject.Add(node.Text, childObject);
                    break;
            }
        }
    }

    private protected override void ProcessImage(string path, Wz_Image image)
    {
        TryAddData(path, image.Node);
    }

    protected override void TryAddData(string path, Wz_Node node)
    {
        if (sEx == null) return;
        Dictionary<string, string>? value;
        int pointIdx = node.Text.IndexOf('.');
        if (!int.TryParse(node.Text.Substring(0, pointIdx), out var id)) return;
        if (!sEx.TryGetValue($"eqp_{id}", out value)) return;

        JsonObject innerObject = new JsonObject();
        foreach(var pair in value!) innerObject.Add(pair.Key, pair.Value);
        RecursiveReadCharacterData(path, node, innerObject);
        innerObject.Add("itemId", id);
        
        string key = ((IDictionary<string, JsonNode?>) innerObject).TryGetValue("name", out var value1) ? value1!.ToString() : node.Text;
        if (!jo.ContainsKey(key)) jo.Add(key, innerObject);
        else
        {
            int idx = 1;
            while (jo.ContainsKey($"{key}_{idx:00}")) idx++;
            jo.Add($"{key}_{idx:00}", innerObject);
        }
    }

    protected override void PostExtract()
    {
        string outputPath = "./CharacterExtractorResult.json";
        File.WriteAllText(outputPath, System.Text.RegularExpressions.Regex.Unescape(jo.ToString()));
    }
}