using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MapleAPI;
using MapleAPI.DataType;
using MapleAPI.Web;
using MapleAPI.WzLoader;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.View;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.Control;

public static class ResourceManager
{
    private static string wzPath = "";
    private static readonly MapleAPI.MapleAPI API = MapleAPI.MapleAPI.Instance;
    private static Dictionary<string, WzItem> itemIcons = new();
    
    #region Wz Related
    public static bool SetWzPath(string path)
    {
        string testFilePath = $@"{path}\Base\Base.wz";
        if (!File.Exists(testFilePath)) return false;
        wzPath = path;
        
        WzLoader.Instance.SetDataPath(wzPath)
            .AddExtract("Skill") // 10,498 Files (36.6MB)
            .AddExtract(
                "Character\\[Accessory,Android,ArcaneForce,AuthenticForce,Cap,Cape,Coat,Dragon,Face,Glove,Longcoat,Mechanic,Pants,Ring,Shield,Shoes,Weapon]") // IconRaw only :: 9,688 Files (12.3MB)
            .AddExtract("Item\\[Consume,Etc,Install]") // 16,759 Files (56.3MB)
            .Extract() // Total about 106MB
            ;
        
        return true;
    }

    public static WzItem? GetItemIcon(string itemName)
    {
        // LoadIcons("./ItemExtractorResult.json");

        return itemIcons!.GetValueOrDefault(itemName, null);
    }

    #endregion

    #region API Related
    public static bool SetApiKey(string apiKey)
    {
        bool apiKeySuccess = API.SetApiKey(apiKey);
        if (apiKeySuccess)
            Summarize.EnableNicknameInput();
        return apiKeySuccess;
    }

    public static async Task<BitmapImage?> LoadPngFromWebUrl(string url)
    {
        try
        {
            using HttpClient client = new HttpClient();
            byte[] pngData = await client.GetByteArrayAsync(url);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = new MemoryStream(pngData);
            bitmapImage.EndInit();
            return bitmapImage;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // ignore
        }

        return null;
    }

    private static void AddErrorData(Dictionary<string, string> dict, APIResponseType resType)
    {
        string errDisplay = resType switch
        {
            APIResponseType.INVALID_IDENTIFIER => "유효하지 않은 데이터가 입력되었습니다.",
            APIResponseType.INVALID_APIKEY => "API Key가 유효하지 않습니다.",
            _ => "알 수 없는 오류 발생"
        };
        dict.Add("error", errDisplay);
    }

    public static Dictionary<string, string> RequestCharBasic(string nickname, out string? ocid)
    {
        Dictionary<string, string> result = new();

        ArgBuilder args = new ArgBuilder()
            .AddArg("character_name", nickname);
        APIResponse res = API.Request(APIRequestType.OCID, args);
        if (res.TryGetValue("ocid", out ocid))
        {
            args.ClearAndAdd("ocid", ocid!);
            res = API.Request(APIRequestType.BASIC, args);
            if (res.IsError) AddErrorData(result, res.ResponseType);
            else res.Extract(result);
        }
        else if(res.IsError) AddErrorData(result, res.ResponseType);
        return result;
    }

    public static void GetCharacterInfo(string ocid)
    {
        BuilderDataContainer.CharacterInfo = CharacterInfo.FromOcid(ocid).Result;
    }
    #endregion

}