using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MapleAPI;
using MapleAPI.DataType;
using MapleAPI.Web;
using MapleAPI.WzLoader;
using MapleBuilder.View;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.Control;

public static class ResourceManager
{
    private static string wzPath = "";
    private static readonly MapleAPI.MapleAPI API = MapleAPI.MapleAPI.Instance;
    private static Dictionary<string, WzItem> itemIcons = new();
    private static bool itemIconLoading;
    public static bool itemIconLoaded;
    
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

    private static void LoadIcons(string path)
    {
        if(!File.Exists(path)) 
            throw new FileNotFoundException(
                $"Please Re-Unpack wzFileData! {path} is not found.");
        
        using StreamReader reader = File.OpenText(path);
        string jsonContent = reader.ReadToEnd();
        JsonObject json = JsonNode.Parse(jsonContent)!.AsObject();

        int id = 0;
        Summarize.StartProgressBar();
        foreach (var pair in json)
        {
            // MainWindow.Window!.Dispatcher.Invoke(() =>
            Summarize.UpdateProgressBar(++id, json.Count);
            
            if (pair.Value is not JsonObject childObject) continue;

            if (!childObject.TryGetPropertyValue("name", out var nameNode)
                || !childObject.TryGetPropertyValue("info", out var infoNode)
                || !(infoNode is JsonObject infoObject &&
                     infoObject.TryGetPropertyValue("icon", out var iconRawNode))) continue;

            string name = nameNode!.ToString();
            if (itemIcons.ContainsKey(name)) continue;

            string iconRawPath = iconRawNode!.ToString();
            if (!iconRawPath.StartsWith("./")) continue; // this is outlink format
            string desc = childObject.TryGetPropertyValue("desc", out var descNode) ? descNode!.ToString() : "";

            MainWindow.Window!.Dispatcher.Invoke(
                () =>
                {
                    itemIcons.Add(name, new WzItem(name, iconRawPath, desc)); 
                });
        }
        Summarize.FinishProgressBar();
    }

    public static WzItem? GetItemIcon(string itemName)
    {
        if (!itemIconLoaded && !itemIconLoading)
        {
            itemIconLoading = true;
            LoadIcons("./CharacterExtractorResult.json");
            LoadIcons("./ItemExtractorResult.json");
            Summarize.VisibleLoadButton();
            itemIconLoaded = true;
        }

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