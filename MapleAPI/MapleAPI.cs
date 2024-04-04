namespace MapleAPI;

public class MapleAPI
{
    class InvalidApiKeyException : Exception
    {
        
    }
    
    // Singletons
    private static MapleAPI? instance;
    public static MapleAPI Instance
    {
        get
        {
            return instance ??= new MapleAPI();
        }
    }
    public static string APIKey => Instance.apiKey ?? "none";

    private static Dictionary<string, string> TestArgs = new()
    {
        {"character_name", "구스구프"}
    };
    private string? apiKey;
    
    private MapleAPI()
    {
        
    }

    /// <summary>
    ///  MapleAPI에서 사용할 API Key를 설정합니다.
    /// </summary>
    /// <param name="key">NEXON Open API에서 발급받은 키</param>
    /// <returns></returns>
    public bool SetApiKey(string key)
    {
        apiKey = key;
        APIResponse test = APIRequest.Request(APIRequestType.OCID, TestArgs);
        if (test.IsError) apiKey = null;
        return !test.IsError;
    }

    public APIResponse Request(APIRequestType type, ArgBuilder args)
    {
        if (apiKey == null) throw new InvalidApiKeyException();
        return APIRequest.Request(type, args);
    }
    
    public async Task<APIResponse> RequestAsync(APIRequestType type, ArgBuilder args)
    {
        if (apiKey == null) throw new InvalidApiKeyException();
        return await APIRequest.RequestAsync(type, args);
    }
    

}