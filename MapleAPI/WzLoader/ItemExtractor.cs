using System.Text.Json.Nodes;
using WzComparerR2.WzLib;

namespace MapleAPI.WzLoader;

public class ItemExtractor : WzExtractor
{
    private JsonObject jo = new();

    protected override string GetWzPath()
    {
        return $@"{dataPath}\Item\Item.wz";
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
                case Double number:
                    jsonObject.Add(node.Text, number);
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
                        if (!imageName.Equals("icon") && !imageName.Equals("iconRaw")) break;
                        if (jsonObject.ContainsKey("icon")) break;
                        imageName = "icon";

                        string fileName = node.Text;
                        string prefix = path.Contains("Consume") ? "consume" : path.Contains("Etc") ? "etc" : path.Contains("Install") ? "ins" : "";
                        if (int.TryParse(parent.ParentNode.Text, out var id)
                            && prefix != "" && sEx!.TryGetValue($"{prefix}_{id}", out var dict)
                            && dict!.ContainsKey("name"))
                        {
                            fileName = dict["name"];
                        }
                        
                        string folder = path.Split(".wz")[0] + ".wz";
                        jsonObject.Add(imageName, ExportPng(png, folder, fileName).Replace("\\", "/"));
                    }
                    else if((node.Text.Equals("icon") || node.Text.Equals("iconRaw")) && !jsonObject.ContainsKey("icon"))
                    {
                        foreach (Wz_Node childPng in node.Nodes)
                        {
                            if(childPng.Text.Equals("_outlink") && childPng.Value is string link)
                                jsonObject.Add("icon", link);
                        }
                    }

                    break;
                }
                case Wz_Uol:
                    break;
                default:
                    JsonObject childObject = new JsonObject();
                    RecursiveReadCharacterData($"{path}\\{node.Text}", node, childObject);
                    if (childObject.Count == 0) break;
                    if(!jsonObject.ContainsKey(node.Text)) jsonObject.Add(node.Text, childObject);
                    break;
            }
        }
    }

    protected override void TryAddData(string path, Wz_Node node)
    {
        
        if (sEx == null) return;
        Dictionary<string, string>? value;
        if (!int.TryParse(node.Text, out var id)) return;
        string prefix = path.Contains("Consume") ? "consume" : path.Contains("Etc") ? "etc" : path.Contains("Install") ? "ins" : "";
        if (!sEx.TryGetValue($"{prefix}_{id}", out value))
        {
            value = new Dictionary<string, string>
            {
                {"temp_name", id.ToString()}
            };
        }

        JsonObject innerObject = new JsonObject();
        foreach(var pair in value!) innerObject.Add(pair.Key, pair.Value);
        RecursiveReadCharacterData(path, node, innerObject);
        innerObject.Add("itemId", id);
        
        string key = ((IDictionary<string, JsonNode?>) innerObject).TryGetValue("name", out var value1) ? value1!.ToString() : node.Text;
        if (!jo.ContainsKey(key)) jo.Add(key, innerObject);
        else
        {
            MergeJsonObject(jo[key]!.AsObject(), innerObject);
            // int idx = 1;
            // while (jo.ContainsKey($"{key}_{idx:00}")) idx++;
            // jo.Add($"{key}_{idx:00}", innerObject);
        }
    }

    protected override void PostExtract()
    {
        Dictionary<int, string> pngPathes = new Dictionary<int, string>();
        Dictionary<string, int> remapTargets = new Dictionary<string, int>();
        foreach (var pair in jo)
        {
            if (!pair.Value!.AsObject().TryGetPropertyValue("info", out var infoNode)
                || !(infoNode is JsonObject infoObject &&
                     infoObject.TryGetPropertyValue("icon", out var iconNode))) continue;

            if (int.TryParse(pair.Value!.AsObject()["itemId"]!.ToString(), out var itemId))
            {
                if (iconNode!.ToString().StartsWith("./ImageOut"))
                    pngPathes.Add(itemId, iconNode.ToString());
                else
                {
                    string remapId = iconNode.ToString().Split(".img")[0];
                    remapId = remapId[(remapId.LastIndexOf("/", StringComparison.CurrentCulture)+1)..];
                    if (int.TryParse(remapId, out var remapId2))
                    {
                        if (pngPathes.TryGetValue(remapId2, out var newPath))
                        {
                            jo[pair.Key]!.AsObject()["info"]!.AsObject()["icon"] = newPath;
                        } else remapTargets.Add(pair.Key, remapId2);
                    }
                }
            }
        }
        
        string outputPath = "./ItemExtractorResult.json";
        File.WriteAllText(outputPath, System.Text.RegularExpressions.Regex.Unescape(jo.ToString()));
    }
}