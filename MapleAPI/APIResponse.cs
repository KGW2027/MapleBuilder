using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace MapleAPI;

public class APIResponse
{
    public bool IsError { get; private set; }
    public APIResponseType ResponseType { get; private set; }
    public JsonObject? JsonData { get; private set; }
    
    public static APIResponse Parse(string result)
    {
        APIResponse response = new APIResponse
        {
            IsError = false,
            ResponseType = APIResponseType.OK
        };
        
        if (JsonNode.Parse(result) is not JsonObject)
        {
            response.IsError = true;
            response.ResponseType = APIResponseType.LOCAL_NOT_RESPONSE_JSON;
            return response;
        }

        response.JsonData = JsonNode.Parse(result)!.AsObject();
        
        // If Error
        if (response.JsonData.ContainsKey("error"))
        {
            response.IsError = true;
            string errorCode = response.JsonData["error"]!["name"]!.ToString()[7..];
            response.ResponseType = int.TryParse(errorCode, out int code)
                ? (APIResponseType) code
                : APIResponseType.LOCAL_NOT_RESPONSE_JSON;
            return response;
        }

        return response;
    }

    public bool TryGetValue(string key, out string? value)
    {
        value = null;
        if (JsonData == null) return false;
        if (JsonData.TryGetPropertyValue(key, out var node))
        {
            if (node is JsonValue) value = node.ToString();
            else value = node!.ToJsonString();
            return true;
        }

        return false;
    }

    public void Extract(Dictionary<string, string> dict, string path = "", bool bOverride = false)
    {
        if (JsonData == null) return;

        JsonNode target = JsonData;
        if (path != "")
        {
            foreach (string p in path.Split("\\"))
            {
                if (target is JsonObject jo && jo.ContainsKey(p))
                {
                    target = jo;
                }
                else if (target is JsonArray ja && int.TryParse(p, out int index) && ja.Count > index)
                {
                    target = ja[index];
                }
                else
                {
                    return;
                }
            }
        }

        if (target is JsonObject joDict)
            foreach (var pair in joDict)
            {
                if (pair.Value == null) continue;
                dict.Add(pair.Key, Regex.Unescape(pair.Value.ToJsonString()).Trim('\"'));
            }
        else if (target is JsonArray jaArray)
            for (int idx = 0; idx < jaArray.Count; idx++)
            {
                if (jaArray[idx] == null) continue;
                dict.Add($"{idx}", Regex.Unescape(jaArray[idx].ToJsonString().Trim('\"')));
            }
    }
}