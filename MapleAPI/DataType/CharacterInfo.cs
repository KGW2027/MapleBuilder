using System.Net;
using System.Text.Json.Nodes;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleAPI.Info;
using MapleAPI.Web;

namespace MapleAPI.DataType;

public class CharacterInfo
{
    public static async Task<CharacterInfo?> FromOcid(string ocid)
    {
        CharacterInfo cInfo = new CharacterInfo();
        long finishedParseInfos = 0;
        
        // 기본 정보
        cInfo.basicInfo = new MapleBasicInfo(ocid, cInfo);
        cInfo.basicInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 0;
        await cInfo.basicInfo.RequestInfo();

        // 장착 장비
        cInfo.equipInfo = new MapleEquipInfo(ocid, cInfo);
        cInfo.equipInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 1;
        await cInfo.equipInfo.RequestInfo();

        // 심볼 데이터 로드
        cInfo.symbolInfo = new MapleSymbolInfo(ocid, cInfo);
        cInfo.symbolInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 2;
        await cInfo.symbolInfo.RequestInfo();
        
        // 펫장비
        cInfo.petInfo = new MaplePetInfo(ocid, cInfo);
        cInfo.petInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 3;
        await cInfo.petInfo.RequestInfo();
        
        // 캐시 장비
        cInfo.cashEquipInfo = new MapleCashEquipInfo(ocid, cInfo);
        cInfo.cashEquipInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 4;
        await cInfo.cashEquipInfo.RequestInfo();
        
        // 스킬 데이터
        cInfo.skillInfo = new MapleSkillInfo(ocid, cInfo);
        cInfo.skillInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 5;
        await cInfo.skillInfo.RequestInfo();

        // 어빌리티
        cInfo.abilityInfo = new MapleAbilityInfo(ocid, cInfo);
        cInfo.abilityInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 6;
        await cInfo.abilityInfo.RequestInfo();
        
        // 하이퍼 스텟
        cInfo.hyperStatInfo = new MapleHyperStatInfo(ocid, cInfo);
        cInfo.hyperStatInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 7;
        await cInfo.hyperStatInfo.RequestInfo();
        
        // 성향
        cInfo.propensityInfo = new MaplePropensityInfo(ocid, cInfo);
        cInfo.propensityInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 8;
        await cInfo.propensityInfo.RequestInfo();
        
        // 헥사 스텟
        cInfo.hexaStatInfo = new MapleHexaStatInfo(ocid, cInfo);
        cInfo.hexaStatInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 9;
        await cInfo.hexaStatInfo.RequestInfo();
        
        // 유니온 공격대 로드
        cInfo.unionRaiderInfo = new MapleRaiderInfo(ocid, cInfo);
        cInfo.unionRaiderInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 10;
        await cInfo.unionRaiderInfo.RequestInfo();
        
        // 유니온 아티팩트 로드
        cInfo.unionArtifactInfo = new MapleArtifactInfo(ocid, cInfo);
        cInfo.unionArtifactInfo.OnFinishParseInfo += () => finishedParseInfos += 1 << 11;
        await cInfo.unionArtifactInfo.RequestInfo();

        // long complete = (1 << 12) - 1;
        // while (finishedParseInfos < complete)
        // {
        //     #if DEBUG
        //     string binaryStr = Convert.ToString(finishedParseInfos, 2);
        //     binaryStr = binaryStr.PadLeft(11, '0');
        //     Console.WriteLine($"[Delay 100ms] Current : {binaryStr}");
        //     #endif
        //     await Task.Delay(100);
        // }
        return cInfo;
    }

    private CharacterInfo() { }

    private MapleBasicInfo? basicInfo;
    private MapleEquipInfo? equipInfo;
    private MapleCashEquipInfo? cashEquipInfo;
    private MapleSymbolInfo? symbolInfo;
    private MaplePetInfo? petInfo;
    private MapleSkillInfo? skillInfo;
    private MapleAbilityInfo? abilityInfo;
    private MapleHyperStatInfo? hyperStatInfo;
    private MaplePropensityInfo? propensityInfo;
    private MapleHexaStatInfo? hexaStatInfo;
    private MapleRaiderInfo? unionRaiderInfo;
    private MapleArtifactInfo? unionArtifactInfo;
    
    
    #region PlayerData

    public MapleClass.ClassType Class => basicInfo!.PlayerClass!.Value;
    public int Level => basicInfo!.PlayerLevel;
    public string PlayerName => basicInfo!.PlayerName!;
    public string WorldName => basicInfo!.WorldName!;
    public string GuildName => basicInfo!.GuildName!;
    public byte[] PlayerImage => basicInfo!.PlayerThumbnail!;
    public Dictionary<string, Dictionary<string, int>> SkillData => skillInfo!.Skills;
    #endregion
    
    #region SpecData

    public List<MapleItemBase> Items => equipInfo!.Items;
    public List<MaplePetItem> PetEquips => petInfo!.PetItems;
    public List<MapleCashItem> CashEquips => cashEquipInfo!.CashEquips;
    public Dictionary<MapleSymbol.SymbolType, int> SymbolLevels => symbolInfo!.SymbolLevels;
    public Dictionary<MapleStatus.StatusType, int> AbilityValues => abilityInfo!.AbilityValues;
    public Dictionary<MapleStatus.StatusType, int> HyperStatLevels => hyperStatInfo!.HyperStatLevels;
    public Dictionary<MaplePropensity.PropensityType, int> PropensityLevels => propensityInfo!.PropensityLevels;
    public MapleHexaStatus.HexaStatus HexaStatLevels => hexaStatInfo!.HexaStatLevels;
    public List<MapleUnion.UnionBlock> UnionInfo => unionRaiderInfo!.UnionInfo;
    public List<MapleStatus.StatusType> UnionInner => unionRaiderInfo!.UnionInner;
    public List<MapleArtifact.ArtifactPanel> ArtifactPanels => unionArtifactInfo!.ArtifactPanels;

    #endregion
}