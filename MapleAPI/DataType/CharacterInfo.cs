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


    private static readonly MaplePotentialOptionType[] STR_DEX =
        {MaplePotentialOptionType.STR, MaplePotentialOptionType.DEX, MaplePotentialOptionType.OTHER};
    private static readonly MaplePotentialOptionType[] DEX_STR =
        {MaplePotentialOptionType.DEX, MaplePotentialOptionType.STR, MaplePotentialOptionType.OTHER};
    private static readonly MaplePotentialOptionType[] INT_LUK =
        {MaplePotentialOptionType.INT, MaplePotentialOptionType.LUK, MaplePotentialOptionType.OTHER};
    private static readonly MaplePotentialOptionType[] LUK_DEX =
        {MaplePotentialOptionType.LUK, MaplePotentialOptionType.DEX, MaplePotentialOptionType.OTHER};
    private static readonly MaplePotentialOptionType[] LUK_DEX_STR =
        {MaplePotentialOptionType.LUK, MaplePotentialOptionType.DEX, MaplePotentialOptionType.STR};
    private static readonly MaplePotentialOptionType[] HP =
        {MaplePotentialOptionType.MAX_HP, MaplePotentialOptionType.OTHER, MaplePotentialOptionType.OTHER};
    private static readonly MaplePotentialOptionType[] NONE =
        {MaplePotentialOptionType.OTHER, MaplePotentialOptionType.OTHER, MaplePotentialOptionType.OTHER};
    public static MaplePotentialOptionType[] GetClassStatType(MapleClass classType)
    {
        switch (classType)
        {
            case MapleClass.HERO: // 전사
            case MapleClass.PALADIN:
            case MapleClass.DARK_KNIGHT:
            case MapleClass.SOUL_MASTER:
            case MapleClass.MIKHAIL:
            case MapleClass.BLASTER:
            case MapleClass.DEMON_SLAYER:
            case MapleClass.ARAN:
            case MapleClass.KAISER:
            case MapleClass.ADEL:
            case MapleClass.ZERO:
            case MapleClass.VAIPER: // 힘해적
            case MapleClass.CANONSHOOTER:
            case MapleClass.STRIKER:
            case MapleClass.EUNWALL:
            case MapleClass.ARC:
                return STR_DEX;
            case MapleClass.DEMON_AVENGER: // 데벤저
                return HP;
            case MapleClass.ARCMAGE_FP: // 법사
            case MapleClass.ARCMAGE_TC:
            case MapleClass.BISHOP:
            case MapleClass.FLAME_WIZARD:
            case MapleClass.BATTLE_MAGE:
            case MapleClass.EVAN:
            case MapleClass.LUMINUS:
            case MapleClass.ILLIUM:
            case MapleClass.LALA:
            case MapleClass.KINESIS:
                return INT_LUK;
            case MapleClass.BOW_MASTER: // 궁수
            case MapleClass.SINGOONG:
            case MapleClass.PATH_FINDER:
            case MapleClass.WIND_BREAKER:
            case MapleClass.WILD_HUNTER:
            case MapleClass.MERCEDES:
            case MapleClass.KAIN:
            case MapleClass.CAPTAIN: // 덱해적
            case MapleClass.ANGELICBUSTER:
            case MapleClass.MECHANIC:
                return DEX_STR;
            case MapleClass.NIGHTLOAD: // 도적
            case MapleClass.NIGHTWALKER:
            case MapleClass.PHANTOM:
            case MapleClass.KALI:
            case MapleClass.HOYOUNG:
                return LUK_DEX;
            case MapleClass.SHADOWER: // 2부스텟 도적 + 제논
            case MapleClass.KADENA:
            case MapleClass.DUALBLADE:
            case MapleClass.XENON:
                return LUK_DEX_STR;
            default:
                return NONE;
        }
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