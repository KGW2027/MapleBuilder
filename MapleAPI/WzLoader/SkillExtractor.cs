using System.Text.Json.Nodes;
using WzComparerR2.WzLib;

namespace MapleAPI.WzLoader;

public class SkillExtractor : WzExtractor
{
    private JsonObject jo = new();

    private string thisSkillName = "";
    
    protected override string GetWzPath()
    {
        return $"{dataPath}/Skill/Skill.wz";
    }

    protected override bool IsTargetImage(string name)
    {
        return int.TryParse(name, out _);
    }

    private void RecursiveReadSkillData(string path, Wz_Node parent, JsonObject jsonObject)
    {
        foreach (Wz_Node node in parent.Nodes)
        {
            // if (node.Value != null) typeList.Add(node.Value.GetType().ToString());
            switch (node.Value)
            {
                case string strValue:
                    if (node.Text.EndsWith(".atlas") || node.Text.Equals("memo")) break;
                    jsonObject.Add(node.Text, ClearString(strValue));
                    break;
                case Single number:
                    jsonObject.Add(node.Text, number);
                    break;
                case Int64 number:
                    jsonObject.Add(node.Text, number);
                    break;
                case Int32 number:
                    jsonObject.Add(node.Text, number);
                    break;
                case Int16 number:
                    jsonObject.Add(node.Text, number);
                    break;
                case Double number:
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
                        if (!imageName.Equals("icon"))
                            break;

                        string fileName = thisSkillName;
                        string folder = path.Split(".wz")[0] + ".wz";
                        jsonObject.Add(node.Text, ExportPng(png, folder, fileName).Replace("\\", "/"));
                    }
                    else if(node.Text.Equals("iconRaw"))
                    {
                        foreach (Wz_Node childPng in node.Nodes)
                        {
                            if(childPng.Text.Equals("_outlink") && childPng.Value is string link)
                                jsonObject.Add(node.Text, link);
                        }
                    }
                    break;
                }
                case Wz_Sound:
                case Wz_Uol:
                    break;
                default:
                    JsonObject childObject = new JsonObject();
                    RecursiveReadSkillData($"{path}\\{node.Text}", node, childObject);
                    if (childObject.Count == 0) break;
                    jsonObject.Add(node.Text, childObject);
                    break;
            }
        }
    }

    protected override void TryAddData(string path, Wz_Node node)
    {
        if (sEx == null) return;
        Dictionary<string, string>? value;
        if (!sEx.TryGetValue($"skill_{node.Text}", out value)) return;


        JsonObject innerObject = new JsonObject();
        foreach(KeyValuePair<string, string> pair in value!) innerObject.Add(pair.Key, pair.Value);
        string key = ((IDictionary<string, JsonNode?>) innerObject).TryGetValue("name", out var value1) ? value1!.ToString() : node.Text;
        thisSkillName = key;
        RecursiveReadSkillData(path, node, innerObject);
        innerObject.Add("skillId", node.Text);

        if (!innerObject.ContainsKey("common")) return;
        
        if (!jo.ContainsKey(key)) jo.Add(key, innerObject);
        else
        {
            int idx = 1;
            while (jo.ContainsKey($"{key}_{idx:00}")) idx++;
            jo.Add($"{key}_{idx:00}", innerObject);
        }
        // if (jo.ContainsKey(key)) MergeJsonObject(jo[key]!.AsObject(), innerObject);
        // else jo.Add(key, innerObject);
    }

    protected override void PostExtract()
    {
        string outputPath = "./SkillExtractorResult.json";
        File.WriteAllText(outputPath, System.Text.RegularExpressions.Regex.Unescape(jo.ToString()));
    }
}