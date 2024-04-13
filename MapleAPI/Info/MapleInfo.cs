using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Web;

namespace MapleAPI.Info;

public abstract class MapleInfo
{
    protected MapleInfo(string ocid, CharacterInfo parent)
    {
        args = new ArgBuilder().AddArg("ocid", ocid);
        this.parent = parent;
    }

    private readonly ArgBuilder args;
    private readonly CharacterInfo parent;
    private JsonObject? jsonData;
    private protected abstract APIRequestType GetRequestType();
    private protected virtual bool GetAdditionalArgs(ArgBuilder args)
    {
        return false;
    }
    private protected abstract void ParseInfo();

    public delegate void FinishParseInfoDelegate();
    public FinishParseInfoDelegate? OnFinishParseInfo;

    private JsonNode? GetPath(string key)
    {
        string[] path = key.Split("/");
        JsonNode node = jsonData!;
        for (int idx = 0; idx < path.Length - 1; idx++)
        {
            if (node is JsonObject objNode)
            {
                node = objNode[path[idx]]!;
            }
            else if (node is JsonArray arrNode)
            {
                int arrIdx = int.Parse(path[idx]);
                node = arrNode[arrIdx]!;
            }
            else
            {
                // throw new InvalidCastException($"A value was found in {path[idx]} of the entire Json path {key}. But, expecting a JsonObject or JsonArray.");
                return null;
            }
        }
        if (node is JsonObject lastObject && lastObject.TryGetPropertyValue(path[^1], out JsonNode? val)) return val;
        if (node is JsonArray lastArray && int.TryParse(path[^1], out int index) && lastArray.Count > index)
            return lastArray[index];
        return null;
    }

    private protected bool TryGet<T>(string path, out T? data)
    {
        JsonNode? pathValue = GetPath(path);
        if (pathValue == null)
        {
            data = default;
            return false;
        }

        if (typeof(T) == typeof(int) && int.TryParse(pathValue.ToString(), out int integerData))
        {
            data = (T)(object)integerData;
            return true;
        }

        if (typeof(T) == typeof(string))
        {
            data = (T) (object) pathValue.ToString();
            return true;
        }

        if ( (typeof(T) == typeof(JsonArray) || typeof(T) == typeof(JsonObject)) && pathValue is T)
        {
            data = (T) (object) pathValue;
            return true;
        } 

        data = default;
        return false;
    }
    
    private protected T? Get<T>(string path)
    {
        JsonNode? pathValue = GetPath(path);
        if (pathValue == null) return default;
        if (typeof(T) == typeof(int)) return (T) (object) int.Parse(pathValue.ToString());
        if (typeof(T) == typeof(string)) return (T) (object) pathValue.ToString();
        if ((typeof(T) == typeof(JsonArray) || typeof(T) == typeof(JsonObject)) && pathValue is T) return (T) (object) pathValue;
        return default;
    }
    
    public async Task RequestInfo()
    {
        GetAdditionalArgs(args);
        do
        {
            var response = await APIRequest.RequestAsync(GetRequestType(), args);
            if (response.IsError)
                throw new APIRequestFailedException(response, args);

            try
            {
                jsonData = response.JsonData!;
                ParseInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred while parsing an API response. Details : {ex.Message}");
            }
        } while (GetAdditionalArgs(args));
    }

}