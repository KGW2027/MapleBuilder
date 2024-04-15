using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Spec;

namespace MapleBuilder.Control.Data;

public class PlayerData
{
    public enum StatSources
    {
        DEFAULT,                // 기본
        EQUIPMENT,              // 장비, 펫장비, 캐시장비, 세트효과, 잠재능력
        SYMBOL,                 // 심볼
        HEXA,                   // 헥사스텟
        HYPER_STAT,             // 하이퍼 스텟
        ABILITY,                // 어빌리티
        UNION,                  // 유니온 공격대, 유니온 아티팩트
        PROPENSITY,             // 성향
        POWER_EFFECTIVE_SKILL,  // 전투력에 포함되는 스킬 (정축, 여축, 펫 세트 효과)
        POWER_INEFFECTIVE_SKILL,// 전투력에 포함되지 않는 스킬
        DOPING,                 // 도핑
        OTHER,                  // 마약 등 기타
    }
    
    /// <summary>
    /// [StatSource, StatusType] - Apply MainStatus
    /// [StatSource] - Get Status by StatSource
    /// [EquipType] - Get Equipment Item from EquipType
    /// [SymbolType] - Get Symbol Level from SymbolType
    /// </summary>
    /// <param name="cInfo"></param>
    public PlayerData(CharacterInfo cInfo)
    {
        level = cInfo.Level;
        playerClass = cInfo.Class;
        AffectTypes = MapleClass.GetClassStatType(playerClass);
        statContainers = new Dictionary<StatSources, MapleStatContainer>();
        symbolLevels = new Dictionary<MapleSymbol.SymbolType, int>();
        
        PlayerImage = cInfo.PlayerImage;
        PlayerName = cInfo.PlayerName;
        PlayerGuild = cInfo.GuildName;
        
        HyperStat = new HyperStatWrapper(cInfo.HyperStatLevels, OnStatusChanged);
        Ability = new AbilityWrapper(cInfo.AbilityValues, OnStatusChanged);
        Artifact = new ArtifactWrapper(cInfo.ArtifactPanels, OnStatusChanged);
        UnionClaim = new UnionClaimWrapper(cInfo.UnionInfo, cInfo.UnionInner, OnStatusChanged);
        Raider = new UnionRaiderWrapper(cInfo.UnionInfo, OnStatusChanged);
        Equipment = new EquipWrapper(OnStatusChanged);
        PetEquip = new PetEquipWrapper(OnStatusChanged);
        
        this[StatSources.DEFAULT, MapleStatus.StatusType.STR] += cInfo.ApStats[0];
        this[StatSources.DEFAULT, MapleStatus.StatusType.DEX] += cInfo.ApStats[1];
        this[StatSources.DEFAULT, MapleStatus.StatusType.INT] += cInfo.ApStats[2];
        this[StatSources.DEFAULT, MapleStatus.StatusType.LUK] += cInfo.ApStats[3];
        this[StatSources.DEFAULT, MapleStatus.StatusType.HP] += cInfo.ApStats[4];
        this[StatSources.DEFAULT, MapleStatus.StatusType.CRITICAL_CHANCE] += 5;

        // Init Symbol Data
        foreach (var pair in cInfo.SymbolLevels) this[pair.Key] += pair.Value;

        // 임시 성향 로드 코드
        foreach (var pair in cInfo.PropensityLevels)
        {
            MapleStatus.StatusType[] propStatTypes = MaplePropensity.GetStatusByPropensity(pair.Key);
            double[] args = MaplePropensity.GetStatusValueByPropensity(pair.Key);

            for (int idx = 0; idx < propStatTypes.Length; idx++)
            {
                double delta = args[idx + 1] * Math.Floor(pair.Value / args[0]);
                this[StatSources.PROPENSITY, propStatTypes[idx]] += delta;
            }
        }
        // 임시 헥사 스텟 로드 코드
        MapleHexaStatus.HexaStatus hexaStatus = cInfo.HexaStatLevels;
        if (hexaStatus.MainStat.Key != MapleStatus.StatusType.OTHER)
        {
            this[StatSources.HEXA, hexaStatus.MainStat.Key] += MapleHexaStatus.GetStatusValue(hexaStatus.MainStat.Key,
                hexaStatus.MainStat.Value, true, playerClass);
            this[StatSources.HEXA, hexaStatus.SubStat1.Key] += MapleHexaStatus.GetStatusValue(hexaStatus.SubStat1.Key,
                hexaStatus.SubStat1.Value, false, playerClass);
            this[StatSources.HEXA, hexaStatus.SubStat2.Key] += MapleHexaStatus.GetStatusValue(hexaStatus.SubStat2.Key,
                hexaStatus.SubStat2.Value, false, playerClass);
        }
        
        isInitialized = true;
    }

    private readonly bool isInitialized;
    private readonly int level;
    private readonly MapleClass.ClassType playerClass;
    private readonly Dictionary<StatSources, MapleStatContainer> statContainers;
    private readonly Dictionary<MapleSymbol.SymbolType, int> symbolLevels;
    
    public readonly MapleStatus.StatusType[] AffectTypes;
    public MapleClass.ClassType Class => playerClass;
    
    public readonly byte[] PlayerImage;
    public readonly string PlayerName;
    public readonly string PlayerGuild;
    public int Level => level;
    

    public double this[StatSources source, MapleStatus.StatusType statusType]
    {
        get => statContainers.TryGetValue(source, out MapleStatContainer? msc) ? msc[statusType] : 0.0;
        set
        {
            if (!statContainers.ContainsKey(source))
                statContainers.TryAdd(source, new MapleStatContainer());
            statContainers[source][statusType] = value;
            AlertUpdate();
        }
    }

    public MapleStatContainer? this[StatSources source]
    {
        get => statContainers.GetValueOrDefault(source);
        set
        {
            statContainers[source] = value ??
                                     throw new NullReferenceException(
                                         "The setter for MapleStatContainer must be non-null.");
            AlertUpdate();
        }
    }

    public int this[MapleSymbol.SymbolType symbolType]
    {
        get => symbolLevels.GetValueOrDefault(symbolType, 0);
        set
        {
            if (symbolType == MapleSymbol.SymbolType.UNKNOWN) return;
            symbolLevels.TryAdd(symbolType, 0);
            int prevLevel = symbolLevels[symbolType];
            symbolLevels[symbolType] = Math.Clamp(value, 0, symbolType <= MapleSymbol.SymbolType.ESFERA ? 20 : 11);
            ChangeSymbolLevel(symbolType, prevLevel, symbolLevels[symbolType]);
            AlertUpdate();
        }
    }

    public readonly HyperStatWrapper HyperStat;
    public readonly AbilityWrapper Ability;
    public readonly UnionClaimWrapper UnionClaim;
    public readonly ArtifactWrapper Artifact;
    public readonly UnionRaiderWrapper Raider;
    public readonly EquipWrapper Equipment;
    public readonly PetEquipWrapper PetEquip;

    public MapleStatContainer GetStatus()
    {
        MapleStatContainer statusContainer = new MapleStatContainer
        {
            MainStatType = AffectTypes[0],
            SubStatType = AffectTypes[1],
            SubStat2Type = AffectTypes[2]
        };
        statusContainer = statContainers.Aggregate(statusContainer, (current, container) => current + container.Value);
        statusContainer.Flush();

        for (byte id = 0x31; id <= (byte) MapleStatus.StatusType.MAG_PER_LEVEL; id++)
        {
            if (id == 0x35) continue; // 0x05 is ALL_STAT
            MapleStatus.StatusType statusType = (MapleStatus.StatusType) id;
            if (statusContainer[statusType] <= 0) continue;

            if (id < 0x35) // 스탯은 9레벨당 STAT +[VALUE]
                statusContainer[(MapleStatus.StatusType) id - 0x30] +=
                    Math.Floor(level / 9.0) * statusContainer[statusType];
            else // 공,마는 어빌에서 [VALUE] 레벨당 공,마 + 1
                statusContainer[(MapleStatus.StatusType) id - 0x30] += Math.Floor(level / statusContainer[statusType]);
        }

        return statusContainer;
    }

#if DEBUG
    public void DEBUG_statContainers()
    {
        foreach (var pair in statContainers)
        {
            Console.WriteLine("");
            Console.WriteLine($" ] === === === {pair.Key}'s StatContainer === === === [");
            pair.Value.Flush();
            foreach(var pair2 in pair.Value)
                Console.WriteLine($"    {pair2.Key} : {pair2.Value:F2}");
        }
    }
#endif

    private void ChangeSymbolLevel(MapleSymbol.SymbolType symbolType, int prev, int next)
    {
        if (symbolType == MapleSymbol.SymbolType.UNKNOWN) return;

        int prevStat, nextStat;
        
        if (symbolType <= MapleSymbol.SymbolType.ESFERA)
        {
            prevStat = prev == 0 ? 0 : prev + 2;
            nextStat = next == 0 ? 0 : next + 2;
            this[StatSources.SYMBOL, MapleStatus.StatusType.ARCANE_FORCE] += (nextStat - prevStat) * 10;
        }
        else
        {
            prevStat = prev == 0 ? 0 : 2 * prev + 3;
            nextStat = next == 0 ? 0 : 2 * next + 3;
            this[StatSources.SYMBOL, MapleStatus.StatusType.AUTHENTIC_FORCE] += (next - prev) * 10;
        }

        int dLv = nextStat - prevStat;
        if (playerClass == MapleClass.ClassType.XENON)
        {
            this[StatSources.SYMBOL, MapleStatus.StatusType.STR_FLAT] += 48 * dLv;
            this[StatSources.SYMBOL, MapleStatus.StatusType.DEX_FLAT] += 48 * dLv;
            this[StatSources.SYMBOL, MapleStatus.StatusType.LUK_FLAT] += 48 * dLv;
        }
        else
        {
            MapleStatus.StatusType flat = playerClass == MapleClass.ClassType.DEMON_AVENGER ? AffectTypes[0] : AffectTypes[0] + 0x20;
            this[StatSources.SYMBOL, flat] +=
                playerClass == MapleClass.ClassType.DEMON_AVENGER ? 2100 * dLv : 100 * dLv;
        }
    }

    private void AlertUpdate()
    {
        if (!isInitialized) return;
        GlobalDataController.OnDataUpdated!.Invoke(this);
    }
    
    private void OnStatusChanged(StatSources source, MapleStatus.StatusType type, double prev, double next)
    {
        this[source, type] -= prev;
        this[source, type] += next;
        AlertUpdate();
    }

}