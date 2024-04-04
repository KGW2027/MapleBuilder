using System.IO;

namespace MapleBuilder.Control;

public class ResourceManager
{
    private static ResourceManager? instance;
    public static ResourceManager Instance
    {
        get
        {
            return instance ??= new ResourceManager();
        }
    }
    
    private ResourceManager()
    {
        // RepFont = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Pretendard");
    }

    public string WzPath { get; private set; }

    public bool SetWzPath(string path)
    {
        string testFilePath = $@"{path}\Base\Base.wz";
        if (!File.Exists(testFilePath)) return false;
        WzPath = path;
        return true;
    }

    public bool SetApiKey(string apiKey)
    {
        return MapleAPI.MapleAPI.Instance.SetApiKey(apiKey);
    }


}