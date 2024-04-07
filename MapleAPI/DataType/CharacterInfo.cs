using System.Net;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class CharacterInfo
{
    public static async Task<CharacterInfo?> FromOcid(string ocid)
    {
        ArgBuilder args = new ArgBuilder().AddArg("ocid", ocid);

        CharacterInfo cInfo = new CharacterInfo();
        APIResponse baseInfo = await APIRequest.RequestAsync(APIRequestType.BASIC, args);
        if (baseInfo.IsError) throw new WebException("API Request Failed! " + baseInfo.ResponseType);

        baseInfo.TryGetValue("character_class", out var nameOfClass);
        cInfo.ClassString = nameOfClass!;
        cInfo.Class = MapleClass.GetMapleClass(nameOfClass!);
        cInfo.LoadPlayerImage(baseInfo.JsonData!["character_image"]!.ToString());
        cInfo.Level = uint.TryParse(baseInfo.JsonData!["character_level"]!.ToString(), out uint levelValue)
            ? levelValue
            : 0;
        if (baseInfo.TryGetValue("character_guild_name", out var nameOfGuild)) cInfo.GuildName = nameOfGuild!;
        if (baseInfo.TryGetValue("character_name", out var nameOfPlayer)) cInfo.UserName = nameOfPlayer!;
        if (baseInfo.TryGetValue("world_name", out var nameOfWorld)) cInfo.WorldName = nameOfWorld!;

        // 장착 장비 로드
        APIResponse equipInfo = await APIRequest.RequestAsync(APIRequestType.ITEM, args);
        if (equipInfo.IsError) throw new WebException("API Request Failed! " + equipInfo.ResponseType);

        cInfo.Items.Clear();
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

        // 심볼 데이터 로드
        APIResponse symbolInfo = await APIRequest.RequestAsync(APIRequestType.SYMBOL, args);
        if (symbolInfo.IsError) throw new WebException("API Request Failed! " + symbolInfo.ResponseType);

        cInfo.SymbolLevels.Clear();
        if (symbolInfo.JsonData!["symbol"] is JsonArray symbolData)
        {
            foreach (var value in symbolData)
            {
                if (value is not JsonObject valueObject) continue;

                MapleSymbol.SymbolType symbolType = MapleSymbol.GetSymbolType(valueObject["symbol_name"]!.ToString());
                if (symbolType == MapleSymbol.SymbolType.UNKNOWN) continue;

                int level = int.Parse(valueObject["symbol_level"]!.ToString());
                cInfo.SymbolLevels.Add(symbolType, level);
            }
        }

        // 어빌 데이터 로드
        APIResponse abilityInfo = await APIRequest.RequestAsync(APIRequestType.ABILITY, args);
        if (abilityInfo.IsError) throw new WebException("API Request Failed! " + abilityInfo.ResponseType);

        cInfo.AbilityValues.Clear();
        if (abilityInfo.JsonData!["ability_info"] is JsonArray abilityData)
        {
            foreach (var value in abilityData)
            {
                string abValue = value!["ability_value"]!.ToString();
                MapleAbility.AbilityType abType = MapleAbility.TryParse(abValue);
                string abStr = MapleAbility.GetAbilityString(abType);
                int valueIndex = abStr.IndexOf("%d", StringComparison.CurrentCulture);
                string parse = valueIndex >= 0 ? abValue.Substring(valueIndex, 2).Replace("%", "").Trim() : "";

                if (int.TryParse(parse, out int abNumValue))
                {
                    cInfo.AbilityValues.Add(abType, abNumValue);
                }
                else
                {
                    MaplePotentialGrade.GradeType abGrade =
                        MaplePotentialGrade.GetPotentialGrade(value["ability_grade"]);
                    cInfo.AbilityValues.Add(abType, MapleAbility.GetMinMax(abType, abGrade)[0]);
                }
            }
        }
        
        // 하이퍼 스탯 로드
        APIResponse hyperStatInfo = await APIRequest.RequestAsync(APIRequestType.HYPER_STAT, args);
        if (hyperStatInfo.IsError) throw new WebException("API Request Failed !" + hyperStatInfo.ResponseType);
        
        cInfo.HyperStatLevels.Clear();
        int useHyperStatPreset =
            int.TryParse(hyperStatInfo.JsonData!["use_preset_no"]!.ToString(), out int hsp) ? hsp : 1;
        if (hyperStatInfo.JsonData[$"hyper_stat_preset_{useHyperStatPreset}"] is JsonArray hyperStats)
        {
            foreach (var stat in hyperStats)
            {
                if (stat is not JsonObject statObject) continue;
                MapleHyperStat.StatType statType = MapleHyperStat.GetStatType(statObject["stat_type"]!.ToString());
                if (statType == MapleHyperStat.StatType.UNKNOWN) continue;
                if (!int.TryParse(statObject["stat_level"]!.ToString(), out int statLevel)) continue;
                if (!cInfo.HyperStatLevels.TryAdd(statType, statLevel)) cInfo.HyperStatLevels[statType] = statLevel;
            }
        }

        return cInfo;
    }

    private CharacterInfo()
    {
        Items = new List<MapleItem>();
        Level = 0;
        ClassString = "";
        GuildName = "";
        UserName = "";
        WorldName = "";
        PlayerImage = Array.Empty<byte>();
        SymbolLevels = new Dictionary<MapleSymbol.SymbolType, int>();
        AbilityValues = new Dictionary<MapleAbility.AbilityType, int>();
        HyperStatLevels = new Dictionary<MapleHyperStat.StatType, int>();
    }
    
    #region PlayerData
    public MapleClass.ClassType Class { get; private set; }
    public string ClassString { get; private set; }
    public uint Level { get; private set; }
    public string UserName { get; private set; }
    public string WorldName { get; private set; }
    public string GuildName { get; private set; }
    public byte[] PlayerImage { get; private set; }
    #endregion
    
    #region SpecData
    public List<MapleItem> Items { get; private set; }
    public Dictionary<MapleSymbol.SymbolType, int> SymbolLevels { get; private set; }
    public Dictionary<MapleAbility.AbilityType, int> AbilityValues { get; private set; }
    public Dictionary<MapleHyperStat.StatType, int> HyperStatLevels { get; private set; }
    #endregion
    

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