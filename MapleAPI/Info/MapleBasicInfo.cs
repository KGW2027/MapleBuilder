using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleAPI.Web;

namespace MapleAPI.Info;

public class MapleBasicInfo : MapleInfo
{
    public MapleBasicInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        PlayerLevel = 0;
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.BASIC;
    }

    public MapleClass.ClassType? PlayerClass;
    public int PlayerLevel;
    public byte[]? PlayerThumbnail;

    public string? GuildName;
    public string? PlayerName;
    public string? WorldName;
    
    private protected override void ParseInfo()
    {
        PlayerClass = MapleClass.GetMapleClass(Get<string>("character_class")!);
        GuildName = Get<string>("character_guild_name");
        PlayerName = Get<string>("character_name");
        WorldName = Get<string>("world_name");
        TryGet("character_level", out PlayerLevel);
        LoadPlayerImage(Get<string>("character_image")!);
        
        #if DEBUG
        Console.WriteLine($"[MapleBasicInfo.ParseInfo called] {PlayerClass} {PlayerLevel} {PlayerName}");
        #endif
    }
    
    private async void LoadPlayerImage(string url)
    {
        try
        {
            using HttpClient client = new HttpClient();
            PlayerThumbnail = await client.GetByteArrayAsync(url);
            OnFinishParseInfo!.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}