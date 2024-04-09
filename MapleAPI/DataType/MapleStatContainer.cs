﻿using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleStatContainer
{
    #region Constructor
    
    public MapleStatContainer()
    {
        statContainer = new Dictionary<MapleStatus.StatusType, double>();
        mainStatType = MapleStatus.StatusType.OTHER;
        subStatType = MapleStatus.StatusType.OTHER;
        subStat2Type = MapleStatus.StatusType.OTHER;
    }

    protected internal static MapleStatContainer LoadFromJson(JsonObject itemJson)
    {
        MapleStatContainer msc = new MapleStatContainer();
        msc[MapleStatus.StatusType.STR] = ReadNumericData(itemJson, "str");
        msc[MapleStatus.StatusType.DEX] = ReadNumericData(itemJson, "dex");
        msc[MapleStatus.StatusType.INT] = ReadNumericData(itemJson, "int");
        msc[MapleStatus.StatusType.LUK] = ReadNumericData(itemJson, "luk");
        msc[MapleStatus.StatusType.HP] = ReadNumericData(itemJson, "max_hp");
        msc[MapleStatus.StatusType.MP] = ReadNumericData(itemJson, "max_mp");
        msc[MapleStatus.StatusType.ATTACK_POWER] = ReadNumericData(itemJson, "attack_power");
        msc[MapleStatus.StatusType.MAGIC_POWER] = ReadNumericData(itemJson, "magic_power");
        msc[MapleStatus.StatusType.DAMAGE] = ReadNumericData(itemJson, "boss_damage");
        msc[MapleStatus.StatusType.BOSS_DAMAGE] = ReadNumericData(itemJson, "damage");
        msc[MapleStatus.StatusType.IGNORE_DEF] = ReadNumericData(itemJson, "ignore_monster_armor");
        msc[MapleStatus.StatusType.ALL_STAT_RATE] = ReadNumericData(itemJson, "all_stat");
        msc[MapleStatus.StatusType.HP_RATE] = ReadNumericData(itemJson, "max_hp_rate");
        msc[MapleStatus.StatusType.MP_RATE] = ReadNumericData(itemJson, "max_mp_rate");
        return msc;
    }
    
    #endregion
    
    private Dictionary<MapleStatus.StatusType, double> statContainer;
    public MapleStatus.StatusType mainStatType;
    public MapleStatus.StatusType subStatType;
    public MapleStatus.StatusType subStat2Type;

    public double this[MapleStatus.StatusType type]
    {
        get => statContainer.GetValueOrDefault(type, 0);
        set
        {
            statContainer.TryAdd(type, 0);
            statContainer[type] = value;
        }
    }

    private double ClearAndGet(MapleStatus.StatusType statusType)
    {
        double value = 0;
        if (!statContainer.ContainsKey(statusType)) return value;
        
        value = statContainer[statusType];
        statContainer.Remove(statusType);
        return value;
    }
    
    public void Flush()
    {
        if (mainStatType != MapleStatus.StatusType.OTHER)
            statContainer[mainStatType] += ClearAndGet(MapleStatus.StatusType.MAIN_STAT);

        if (subStatType != MapleStatus.StatusType.OTHER)
        {
            double subStatValue = ClearAndGet(MapleStatus.StatusType.SUB_STAT);
            statContainer[subStatType] += subStatValue;
            if (subStat2Type != MapleStatus.StatusType.OTHER)
                statContainer[subStat2Type] += subStatValue;
        }

        double allStatFlat = ClearAndGet(MapleStatus.StatusType.ALL_STAT);
        statContainer[MapleStatus.StatusType.STR] += allStatFlat;
        statContainer[MapleStatus.StatusType.DEX] += allStatFlat;
        statContainer[MapleStatus.StatusType.INT] += allStatFlat;
        statContainer[MapleStatus.StatusType.LUK] += allStatFlat;

        double allStatRate = ClearAndGet(MapleStatus.StatusType.ALL_STAT_RATE);
        statContainer[MapleStatus.StatusType.STR_RATE] += allStatRate;
        statContainer[MapleStatus.StatusType.DEX_RATE] += allStatRate;
        statContainer[MapleStatus.StatusType.INT_RATE] += allStatRate;
        statContainer[MapleStatus.StatusType.LUK_RATE] += allStatRate;

        double atkmag = ClearAndGet(MapleStatus.StatusType.ATTACK_AND_MAGIC);
        statContainer[MapleStatus.StatusType.ATTACK_POWER] += atkmag;
        statContainer[MapleStatus.StatusType.MAGIC_POWER] += atkmag;

    }

    #region Operator Related
    private static int ReadNumericData(JsonObject data, string key)
    {
         return data.ContainsKey(key) && int.TryParse(data[key]!.ToString(), out var val) ? val : 0;
    }
    
    private static double ApplyMultipleCalc(double lhs, double rhs)
    {
        double cvtBase = (100 - Math.Abs(lhs)) / 100.0;
        double cvtAdd = (100 - Math.Abs(rhs)) / 100.0;
        return rhs >= 0 ? (1 - cvtBase * cvtAdd) * 100 : (1 - cvtBase / cvtAdd) * 100;
    }
    
    public static MapleStatContainer operator +(MapleStatContainer lhs, MapleStatContainer rhs)
    {
        foreach (var pair in rhs.statContainer)
        {
            lhs.statContainer.TryAdd(pair.Key, 0);
            if (pair.Key is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
            {
                lhs.statContainer[pair.Key] =
                    ApplyMultipleCalc(lhs.statContainer[pair.Key], rhs.statContainer[pair.Key]);
            }
            else lhs.statContainer[pair.Key] += rhs.statContainer[pair.Key];
        }
        return lhs;
    }
    
    public static MapleStatContainer operator -(MapleStatContainer lhs, MapleStatContainer rhs)
    {
        foreach (var pair in rhs.statContainer)
        {
            lhs.statContainer.TryAdd(pair.Key, 0);
            if (pair.Key is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
            {
                lhs.statContainer[pair.Key] =
                    ApplyMultipleCalc(lhs.statContainer[pair.Key], -rhs.statContainer[pair.Key]);
            }
            else lhs.statContainer[pair.Key] -= rhs.statContainer[pair.Key];
        }
        return lhs;
    }

    public static MapleStatContainer operator -(MapleStatContainer self)
    {
        foreach (var pair in self.statContainer)
        {
            self[pair.Key] = -pair.Value;
        }

        return self;
    }
    #endregion


}