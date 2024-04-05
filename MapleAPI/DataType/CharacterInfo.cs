using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class CharacterInfo
{
    #region PlayerData
    public MapleClass Class { get; private set; }
    public string ClassString { get; private set; }
    public uint Level { get; private set; }
    public string UserName { get; private set; }
    public string WorldName { get; private set; }
    public string GuildName { get; private set; }
    public byte[] PlayerImage { get; private set; }
    #endregion
    
    #region SpecData
    public List<MapleItem> Items { get; private set; }
    #endregion
    
    private CharacterInfo()
    {
        Items = new List<MapleItem>();
        Level = 0;
        ClassString = "";
        GuildName = "";
        UserName = "";
        WorldName = "";
        PlayerImage = Array.Empty<byte>();
    }

    private static MapleClass GetMapleClass(string className)
    {
        return className.Replace(" ", "") switch
        {
            "히어로" => MapleClass.HERO,
            "팔라딘" => MapleClass.PALADIN,
            "다크나이트" => MapleClass.DARK_KNIGHT,
            "소울마스터" => MapleClass.SOUL_MASTER,
            "미하일" => MapleClass.MIKHAIL,
            "블래스터" => MapleClass.BLASTER,
            "데몬슬레이어" => MapleClass.DEMON_SLAYER,
            "데몬어벤져" => MapleClass.DEMON_AVENGER,
            "아란" => MapleClass.ARAN,
            "카이저" => MapleClass.KAISER,
            "아델" => MapleClass.ADEL,
            "제로" => MapleClass.ZERO,
            "아크메이지(불,독)" => MapleClass.ARCMAGE_FP,
            "아크메이지(썬,콜)" => MapleClass.ARCMAGE_TC,
            "비숍" => MapleClass.BISHOP,
            "플레임위자드" => MapleClass.FLAME_WIZARD,
            "배틀메이지" => MapleClass.BATTLE_MAGE,
            "에반" => MapleClass.EVAN,
            "루미너스" => MapleClass.LUMINUS,
            "일리움" => MapleClass.ILLIUM,
            "라라" => MapleClass.LALA,
            "키네시스" => MapleClass.KINESIS,
            "보우마스터" => MapleClass.BOW_MASTER,
            "신궁" => MapleClass.SINGOONG,
            "패스파인더" => MapleClass.PATH_FINDER,
            "윈드브레이커" => MapleClass.WIND_BREAKER,
            "와일드헌터" => MapleClass.WILD_HUNTER,
            "메르세데스" => MapleClass.MERCEDES,
            "카인" => MapleClass.KAIN,
            "나이트로드" => MapleClass.NIGHTLOAD,
            "섀도어" => MapleClass.SHADOWER,
            "듀얼블레이드" => MapleClass.DUALBLADE,
            "나이트워커" => MapleClass.NIGHTWALKER,
            "제논" => MapleClass.XENON,
            "팬텀" => MapleClass.PHANTOM,
            "카데나" => MapleClass.KADENA,
            "칼리" => MapleClass.KALI,
            "호영" => MapleClass.HOYOUNG,
            "바이퍼" => MapleClass.VAIPER,
            "캡틴" => MapleClass.CAPTAIN,
            "캐논마스터" => MapleClass.CANONSHOOTER,
            "스트라이커" => MapleClass.STRIKER,
            "메카닉" => MapleClass.MECHANIC,
            "은월" => MapleClass.EUNWALL,
            "엔젤릭버스터" => MapleClass.ANGELICBUSTER,
            "아크" => MapleClass.ARC,
            _ => MapleClass.NONE
        };
    }
    
    public static CharacterInfo? FromOcid(string ocid)
    {
        ArgBuilder args = new ArgBuilder().AddArg("ocid", ocid);
        
        CharacterInfo cInfo = new CharacterInfo();
        APIResponse baseInfo = APIRequest.Request(APIRequestType.BASIC, args);
        if (baseInfo.IsError) return null;
        
        baseInfo.TryGetValue("character_class", out var nameOfClass);
        cInfo.ClassString = nameOfClass!;
        cInfo.Class = GetMapleClass(nameOfClass!);
        cInfo.LoadPlayerImage(baseInfo.JsonData!["character_image"]!.ToString());
        cInfo.Level = uint.TryParse(baseInfo.JsonData!["character_level"]!.ToString(), out uint levelValue)
            ? levelValue
            : 0;
        if (baseInfo.TryGetValue("character_guild_name", out var nameOfGuild)) cInfo.GuildName = nameOfGuild!;
        if (baseInfo.TryGetValue("character_name", out var nameOfPlayer)) cInfo.UserName = nameOfPlayer!;
        if (baseInfo.TryGetValue("world_name", out var nameOfWorld)) cInfo.WorldName = nameOfWorld!;

        // 장착 장비 로드
        APIResponse equipInfo = APIRequest.Request(APIRequestType.ITEM, args);
        if (equipInfo.IsError) return null;
        
        cInfo.Items = new List<MapleItem>();
        HashSet<string> itemHashes = new HashSet<string>();
        for (int idx = 1; idx <= 3; idx++)
        {
            if (!equipInfo.TryGetValue($"item_equipment_preset_{idx}", out var preset)) continue;
            JsonArray itemList = JsonNode.Parse(preset!)!.AsArray();
            foreach (var id in itemList)
            {
                MapleItem item = MapleItem.Parse(id!.AsObject());
                if (!itemHashes.Add(item.Hash)) continue;
                cInfo.Items.Add(item);
            }
        }
        cInfo.Items.Add(MapleItem.Parse(equipInfo.JsonData!["title"]!.AsObject()));
        
        return cInfo;
    }


    private async void LoadPlayerImage(string url)
    {
        try
        {
            using HttpClient client = new HttpClient();
            PlayerImage = await client.GetByteArrayAsync(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}