using System.Net;
using System.Text.Json.Nodes;
using MapleAPI.DataType.Item;
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
                MapleCommonItem item = new MapleCommonItem(id!.AsObject());
                if (!itemHashes.Add(item.Hash)) continue;
                cInfo.Items.Add(item);
            }
        }

        cInfo.Items.Add(new MapleTitleItem(equipInfo.JsonData!["title"]!.AsObject()));

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
                MapleStatus.StatusType abType = MapleAbility.TryParse(abValue);
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
                MapleStatus.StatusType statType = MapleHyperStat.GetStatType(statObject["stat_type"]!.ToString());
                if (statType == MapleStatus.StatusType.OTHER) continue;
                if (!int.TryParse(statObject["stat_level"]!.ToString(), out int statLevel)) continue;
                if (!cInfo.HyperStatLevels.TryAdd(statType, statLevel)) cInfo.HyperStatLevels[statType] = statLevel;
            }
        }
        
        // 유니온 공격대 로드
        APIResponse unionRaiderInfo = await APIRequest.RequestAsync(APIRequestType.UNION_RADIER, args);
        if (unionRaiderInfo.IsError) throw new WebException("API Request Failed !" + unionRaiderInfo.ResponseType);

        cInfo.UnionInfo.Clear();
        if (unionRaiderInfo.JsonData!["union_block"] is JsonArray unionBlocks)
        {
            foreach (var unionNode in unionBlocks)
            {
                if (unionNode is not JsonObject unionBlock) continue;
                
                MapleClass.ClassType blockClass = MapleClass.GetMapleClass(unionBlock["block_class"]!.ToString());
                MapleUnion.RaiderRank blockRank =
                    MapleUnion.GetRaiderRank(int.Parse(unionBlock["block_level"]!.ToString()), blockClass);
                sbyte[][] claims = new sbyte[(int) blockRank][];
                JsonArray unionClaims = unionBlock["block_position"]!.AsArray();
                for (int idx = 0; idx < unionClaims.Count; idx++)
                {
                    JsonObject claimVector = unionClaims[idx]!.AsObject();
                    claims[idx] = new[] {sbyte.Parse(claimVector["x"]!.ToString()), sbyte.Parse(claimVector["y"]!.ToString())};
                }
                
                cInfo.UnionInfo.Add(new MapleUnion.UnionBlock {blockPositions = claims, classType = blockClass, raiderRank = blockRank});
            }
        }

        cInfo.UnionInner.Clear();
        if (unionRaiderInfo.JsonData!["union_inner_stat"] is JsonArray unionInners)
        {
            foreach (var unionInnerNode in unionInners)
            {
                if (unionInnerNode is not JsonObject unionInner) continue;
                cInfo.UnionInner.Add(MapleUnion.GetStatusTypeByUnionField(unionInner["stat_field_effect"]!.ToString()));
            }
        }
        
        // 유니온 아티팩트 로드
        APIResponse unionArtifactInfo = await APIRequest.RequestAsync(APIRequestType.UNION_ARTIFACT, args);
        if (unionArtifactInfo.IsError) throw new WebException("API Request Failed !" + unionArtifactInfo.ResponseType);
        
        cInfo.ArtifactPanels.Clear();
        if (unionArtifactInfo.JsonData!["union_artifact_crystal"] is JsonArray crystals)
        {
            foreach (var artifactCrystal in crystals)
            {
                if (artifactCrystal is not JsonObject crystal) continue;
                
                int level = int.Parse(crystal["level"]!.ToString());
                MapleArtifact.ArtifactPanel panel = new MapleArtifact.ArtifactPanel(level);
                for (int idx = 1; idx <= 3; idx++)
                    panel.StatusTypes[idx-1] = MapleArtifact.GetArtifactType(crystal[$"crystal_option_name_{idx}"]!.ToString());
                cInfo.ArtifactPanels.Add(panel);
            }
        }
        
        // 펫장비 로드
        APIResponse petInfo = await APIRequest.RequestAsync(APIRequestType.PET, args);
        if (petInfo.IsError) throw new WebException("API Request Failed !" + petInfo.ResponseType);
        
        cInfo.PetInfo.Clear();
        for (int index = 1; index <= 3; index++)
        {
            if (petInfo.JsonData![$"pet_{index}_equipment"] is not JsonObject petEquip) continue;
            JsonNode? petTypeNode = petInfo.JsonData![$"pet_{index}_pet_type"];
            MaplePetType.PetType petType = petTypeNode == null
                ? MaplePetType.PetType.OTHER
                : MaplePetType.GetPetType(petTypeNode.ToString());
            
            cInfo.PetInfo.Add(new MaplePetItem(petEquip, petType));
        }
        
        // 스킬 정보 로드
        string[] skillGrade = {"0", "1", "1.5", "2", "2.5", "3", "4", "hyperpassive", "hyperactive", "5", "6"};
        cInfo.SkillData.Clear();
        foreach (string grade in skillGrade)
        {
            args.AddArg("character_skill_grade", grade);
            APIResponse skillInfo = await APIRequest.RequestAsync(APIRequestType.SKILL, args);
            if (skillInfo.IsError) throw new WebException("API Request Failed !" + skillInfo.ResponseType);
            args.Remove("character_skill_grade");

            if (skillInfo.JsonData!["character_skill"] is not JsonArray skillList) continue;

            Dictionary<string, int> inSkillData = new Dictionary<string, int>();
            foreach (var skillNode in skillList)
            {
                if (skillNode is not JsonObject skillData || skillData["skill_name"] == null) continue;
                string skillName = skillData["skill_name"]!.ToString();
                int skillLevel = int.Parse(skillData["skill_level"]!.ToString());
                inSkillData.TryAdd(skillName, skillLevel);
            }

            cInfo.SkillData.TryAdd(grade, inSkillData);
        }
        
        // 캐시 착용 정보 로드
        APIResponse cashInfo = await APIRequest.RequestAsync(APIRequestType.CASH_ITEM, args);
        if (cashInfo.IsError) throw new WebException("API Request Failed !" + cashInfo.ResponseType);
        
        cInfo.CashItems.Clear();
        if (cashInfo.JsonData!["cash_item_equipment_base"] is JsonArray cashItems)
        {
            foreach (var cashItemNode in cashItems)
            {
                if (cashItemNode is not JsonObject cashItemData) continue;
                MapleCashItem cashItem = new MapleCashItem(cashItemData);
                if(!cashItem.IsEmpty) cInfo.CashItems.Add(cashItem);
            }
        }
        
        // 성향 정보 로드
        APIResponse propensityInfo = await APIRequest.RequestAsync(APIRequestType.PROPENSITY, args);
        if(propensityInfo.IsError) throw new WebException("API Request Failed !" + propensityInfo.ResponseType);
        
        cInfo.PropensityLevels.Clear();
        foreach (MaplePropensity.PropensityType propensityType in
                 System.Enum.GetValues<MaplePropensity.PropensityType>())
        {
            string jsonKey = $"{propensityType.ToString().ToLower()}_level";
            if (!propensityInfo.JsonData!.TryGetPropertyValue(jsonKey, out var propNode) || propNode == null) continue;
            cInfo.PropensityLevels[propensityType] = int.Parse(propNode.ToString());
        }
        
        // 헥사 스텟 로드
        APIResponse hexaStatInfo = await APIRequest.RequestAsync(APIRequestType.HEXA_STAT, args);
        if(hexaStatInfo.IsError) throw new WebException("API Request Failed !" + hexaStatInfo.ResponseType);

        if (hexaStatInfo.JsonData!["character_hexa_stat_core"] is JsonArray {Count: > 0} array)
        {
            if (array[0] is JsonObject obj)
            {
                string[] keys = {"main_stat_name", "sub_stat_name_1", "sub_stat_name_2"};
                string[] values = {"main_stat_level", "sub_stat_level_1", "sub_stat_level_2"};
                KeyValuePair<MapleStatus.StatusType, int>[] newPairs = new KeyValuePair<MapleStatus.StatusType, int>[3];
                for (int idx = 0; idx <= 2; idx++)
                {
                    MapleStatus.StatusType statusType =
                        MapleHexaStatus.GetStatusTypeFromHexaStatus(obj[keys[idx]]!.ToString());
                    if (statusType == MapleStatus.StatusType.OTHER ||
                        !int.TryParse(obj[values[idx]]!.ToString(), out int statLevel)) continue;
                    newPairs[idx] = KeyValuePair.Create(statusType, statLevel);
                }

                cInfo.HexaStatLevels = new MapleHexaStatus.HexaStatus
                {
                    MainStat = newPairs[0],
                    SubStat1 = newPairs[1],
                    SubStat2 = newPairs[2]
                };
            }
        }
        
        
        return cInfo;
    }

    private CharacterInfo()
    {
        Items = new List<MapleItemBase>();
        Level = 0;
        ClassString = "";
        GuildName = "";
        UserName = "";
        WorldName = "";
        PlayerImage = Array.Empty<byte>();
        SkillData = new Dictionary<string, Dictionary<string, int>>();
        
        SymbolLevels = new Dictionary<MapleSymbol.SymbolType, int>();
        AbilityValues = new Dictionary<MapleStatus.StatusType, int>();
        HyperStatLevels = new Dictionary<MapleStatus.StatusType, int>();
        HexaStatLevels = new MapleHexaStatus.HexaStatus();
        PropensityLevels = new Dictionary<MaplePropensity.PropensityType, int>();
        UnionInfo = new List<MapleUnion.UnionBlock>();
        UnionInner = new List<MapleStatus.StatusType>();
        ArtifactPanels = new List<MapleArtifact.ArtifactPanel>();
        PetInfo = new List<MaplePetItem>();
        CashItems = new List<MapleCashItem>();
    }
    
    #region PlayerData
    public MapleClass.ClassType Class { get; private set; }
    public string ClassString { get; private set; }
    public uint Level { get; private set; }
    public string UserName { get; private set; }
    public string WorldName { get; private set; }
    public string GuildName { get; private set; }
    public byte[] PlayerImage { get; private set; }
    public Dictionary<string, Dictionary<string, int>> SkillData { get; private set; }
    #endregion
    
    #region SpecData
    public List<MapleItemBase> Items { get; private set; }
    public List<MaplePetItem> PetInfo { get; private set; }
    public List<MapleCashItem> CashItems { get; private set; }
    public Dictionary<MapleSymbol.SymbolType, int> SymbolLevels { get; private set; }
    public Dictionary<MapleStatus.StatusType, int> AbilityValues { get; private set; }
    public Dictionary<MapleStatus.StatusType, int> HyperStatLevels { get; private set; }
    public MapleHexaStatus.HexaStatus HexaStatLevels { get; private set; }
    public List<MapleUnion.UnionBlock> UnionInfo { get; private set; }
    public List<MapleStatus.StatusType> UnionInner { get; private set; }
    public List<MapleArtifact.ArtifactPanel> ArtifactPanels { get; private set; }
    public Dictionary<MaplePropensity.PropensityType, int> PropensityLevels { get; private set; }
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