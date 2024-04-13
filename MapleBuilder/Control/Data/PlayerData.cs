using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;

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
    
    public PlayerData(int level, MapleClass.ClassType playerClass, int[] apStats)
    {
        this.level = level;
        this.playerClass = playerClass;
        affectTypes = MapleClass.GetClassStatType(playerClass);
        statContainers = new Dictionary<StatSources, MapleStatContainer>();
        equipData = new Dictionary<MapleEquipType.EquipType, ItemBase?>();

        this[StatSources.DEFAULT, MapleStatus.StatusType.STR] += apStats[0];
        this[StatSources.DEFAULT, MapleStatus.StatusType.DEX] += apStats[1];
        this[StatSources.DEFAULT, MapleStatus.StatusType.INT] += apStats[2];
        this[StatSources.DEFAULT, MapleStatus.StatusType.LUK] += apStats[3];
        this[StatSources.DEFAULT, MapleStatus.StatusType.HP] += apStats[4];
        this[StatSources.DEFAULT, MapleStatus.StatusType.CRITICAL_CHANCE] += 5;
    }

    private int level;
    private MapleClass.ClassType playerClass;
    private readonly MapleStatus.StatusType[] affectTypes;
    private readonly Dictionary<StatSources, MapleStatContainer> statContainers;
    private readonly Dictionary<MapleEquipType.EquipType, ItemBase?> equipData;

    public double this[StatSources source, MapleStatus.StatusType statusType]
    {
        get => statContainers.TryGetValue(source, out MapleStatContainer? msc) ? msc[statusType] : 0.0;
        set
        {
            if (!statContainers.ContainsKey(source))
                statContainers.TryAdd(source, new MapleStatContainer());
            statContainers[source][statusType] = value;
            GlobalDataController.OnDataUpdated!.Invoke(this);
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
            GlobalDataController.OnDataUpdated!.Invoke(this);
        }
    }

    public ItemBase? this[MapleEquipType.EquipType equipType]
    {
        get => equipData.GetValueOrDefault(equipType);
        set
        {
            ItemBase? curItem = this[equipType];
            if (curItem != null)
                RemoveItem(curItem);
            AddItem(value);
        }
    }

    private void AddItem(ItemBase? item)
    {
        if (item == null) return;
        if (this[StatSources.EQUIPMENT] == null) this[StatSources.EQUIPMENT] = new MapleStatContainer();
        this[StatSources.EQUIPMENT] += item.StatContainer;
    }

    private void RemoveItem(ItemBase item)
    {
        equipData[item.EquipType] = null;
        this[StatSources.EQUIPMENT] -= item.StatContainer;
    }

}