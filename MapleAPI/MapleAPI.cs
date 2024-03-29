namespace MapleAPI;

public class MapleAPI
{
    // Singletons
    private static MapleAPI? instance;
    public static MapleAPI Instance
    {
        get
        {
            return instance ??= new MapleAPI();
        }
    }

    // Private
    private string? apiKey;
    
    private MapleAPI()
    {
        
    }

    /// <summary>
    ///  MapleAPI에서 사용할 API Key를 설정합니다.
    /// </summary>
    /// <param name="key">NEXON Open API에서 발급받은 키</param>
    /// <returns></returns>
    public MapleAPI SetApiKey(string key)
    {
        apiKey = key;
        return this;
    }
    

}