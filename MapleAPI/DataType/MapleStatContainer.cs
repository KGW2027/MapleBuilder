﻿using System.Diagnostics;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleStatContainer
{
    #region Constructor
    
    public MapleStatContainer()
    {
        statContainer = new Dictionary<MapleStatus.StatusType, double>();
        MainStatType = MapleStatus.StatusType.OTHER;
        SubStatType = MapleStatus.StatusType.OTHER;
        SubStat2Type = MapleStatus.StatusType.OTHER;

        #if DEBUG
            SourceTrace = new Dictionary<string, MapleStatContainer>();
        #endif
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
        msc[MapleStatus.StatusType.BOSS_DAMAGE] = ReadNumericData(itemJson, "boss_damage");
        msc[MapleStatus.StatusType.DAMAGE] = ReadNumericData(itemJson, "damage");
        msc[MapleStatus.StatusType.IGNORE_DEF] = ReadNumericData(itemJson, "ignore_monster_armor");
        msc[MapleStatus.StatusType.ALL_STAT_RATE] = ReadNumericData(itemJson, "all_stat");
        msc[MapleStatus.StatusType.HP_RATE] = ReadNumericData(itemJson, "max_hp_rate");
        msc[MapleStatus.StatusType.MP_RATE] = ReadNumericData(itemJson, "max_mp_rate");
        return msc;
    }
    
    #endregion
    
    private Dictionary<MapleStatus.StatusType, double> statContainer;
    public MapleStatus.StatusType MainStatType;
    public MapleStatus.StatusType SubStatType;
    public MapleStatus.StatusType SubStat2Type;

    #if DEBUG
        public Dictionary<string, MapleStatContainer> SourceTrace;
    #endif
    
    public double this[MapleStatus.StatusType type]
    {
        get => statContainer.GetValueOrDefault(type, 0);
        set
        {
            statContainer.TryAdd(type, 0);
            #if DEBUG
                StackTrace st = new StackTrace();
                StackFrame? stackFrame = st.GetFrame(1);
                if (stackFrame != null)
                {
                    if (!stackFrame.GetMethod()!.DeclaringType!.FullName!.Equals("MapleAPI.DataType.MapleStatContainer")
                        && !stackFrame.GetMethod()!.Name.Equals("set_item"))
                    {
                        string trace = $"{stackFrame.GetMethod()!.DeclaringType!.FullName}::{stackFrame.GetMethod()!.Name}";
                        // Console.WriteLine(trace);
                        SourceTrace.TryAdd(trace, new MapleStatContainer());

                        if (type is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                            SourceTrace[trace][type] =
                                ApplyMultipleCalc(SourceTrace[trace][type], value - statContainer[type]);
                        else SourceTrace[trace][type] += value - statContainer[type];
                    }
                }
            #endif
            
            if (type is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                statContainer[type] = ApplyMultipleCalc(statContainer[type], value - statContainer[type]);
            else statContainer[type] = value;
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

    private void SafeAdd(MapleStatus.StatusType type, double value)
    {
        statContainer.TryAdd(type, 0);
        statContainer[type] += value;
    }

    private void SafeSubtract(MapleStatus.StatusType type, double value)
    {
        statContainer.TryAdd(type, 0);
        statContainer[type] -= value;
    }
    
    public void Flush()
    {
        if (MainStatType != MapleStatus.StatusType.OTHER)
            SafeAdd(MainStatType, ClearAndGet(MapleStatus.StatusType.MAIN_STAT));

        if (SubStatType != MapleStatus.StatusType.OTHER)
        {
            double subStatValue = ClearAndGet(MapleStatus.StatusType.SUB_STAT);
            SafeAdd(SubStatType, subStatValue);
            if (SubStat2Type != MapleStatus.StatusType.OTHER)
                SafeAdd(SubStat2Type, subStatValue);
        }

        double strdexluk = ClearAndGet(MapleStatus.StatusType.STR_DEX_LUK);
        SafeAdd(MapleStatus.StatusType.STR_FLAT, strdexluk);
        SafeAdd(MapleStatus.StatusType.DEX_FLAT, strdexluk);
        SafeAdd(MapleStatus.StatusType.LUK_FLAT, strdexluk);

        double allStatFlat = ClearAndGet(MapleStatus.StatusType.ALL_STAT);
        SafeAdd(MapleStatus.StatusType.STR, allStatFlat);
        SafeAdd(MapleStatus.StatusType.DEX, allStatFlat);
        SafeAdd(MapleStatus.StatusType.INT, allStatFlat);
        SafeAdd(MapleStatus.StatusType.LUK, allStatFlat);

        double allStatRate = ClearAndGet(MapleStatus.StatusType.ALL_STAT_RATE);
        SafeAdd(MapleStatus.StatusType.STR_RATE, allStatRate);
        SafeAdd(MapleStatus.StatusType.DEX_RATE, allStatRate);
        SafeAdd(MapleStatus.StatusType.INT_RATE, allStatRate);
        SafeAdd(MapleStatus.StatusType.LUK_RATE, allStatRate);

        double atkmag = ClearAndGet(MapleStatus.StatusType.ATTACK_AND_MAGIC);
        SafeAdd(MapleStatus.StatusType.ATTACK_POWER, atkmag);
        SafeAdd(MapleStatus.StatusType.MAGIC_POWER, atkmag);

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
        MapleStatContainer msc = new MapleStatContainer
        {
            MainStatType = lhs.MainStatType,
            SubStatType = lhs.SubStatType,
            SubStat2Type = lhs.SubStat2Type,
            SourceTrace = lhs.SourceTrace,
        };

        foreach (var pair in lhs.statContainer)
        {
            msc.statContainer.TryAdd(pair.Key, pair.Value);
        }
        foreach (var pair in rhs.statContainer)
        {
            msc.statContainer.TryAdd(pair.Key, 0);
            if (pair.Key is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                msc.statContainer[pair.Key] = ApplyMultipleCalc(msc.statContainer[pair.Key], pair.Value);
            else
                msc.statContainer[pair.Key] += pair.Value;
        }
        
#if DEBUG
        StackTrace st = new StackTrace();
        StackFrame? stackFrame = st.GetFrame(1);
        if (stackFrame != null)
        {
            foreach (var pair in rhs.statContainer)
            {
                string trace = $"{stackFrame.GetMethod()!.DeclaringType!.FullName}::{stackFrame.GetMethod()!.Name}";
                msc.SourceTrace.TryAdd(trace, new MapleStatContainer());

                MapleStatus.StatusType type = pair.Key;
                if (type is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                    msc.SourceTrace[trace][type] = ApplyMultipleCalc(msc.SourceTrace[trace][type], pair.Value);
                else msc.SourceTrace[trace][type] += pair.Value;
            }
        }
#endif
        
        return msc;
    }
    
    public static MapleStatContainer operator -(MapleStatContainer lhs, MapleStatContainer rhs)
    {
        MapleStatContainer msc = new MapleStatContainer
        {
            MainStatType = lhs.MainStatType,
            SubStatType = lhs.SubStatType,
            SubStat2Type = lhs.SubStat2Type
        };
        
        foreach (var pair in lhs.statContainer)
        {
            msc.statContainer.TryAdd(pair.Key, pair.Value);
        }
        foreach (var pair in rhs.statContainer)
        {
            msc.statContainer.TryAdd(pair.Key, 0);
            if (pair.Key is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                msc.statContainer[pair.Key] = ApplyMultipleCalc(msc.statContainer[pair.Key], -pair.Value);
            else
                msc.statContainer[pair.Key] -= pair.Value;
        }
        
#if DEBUG
        StackTrace st = new StackTrace();
        StackFrame? stackFrame = st.GetFrame(1);
        if (stackFrame != null)
        {
            foreach (var pair in rhs.statContainer)
            {
                string trace = $"{stackFrame.GetMethod()!.DeclaringType!.FullName}::{stackFrame.GetMethod()!.Name}";
                msc.SourceTrace.TryAdd(trace, new MapleStatContainer());

                MapleStatus.StatusType type = pair.Key;
                if (type is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                    msc.SourceTrace[trace][type] = ApplyMultipleCalc(msc.SourceTrace[trace][type], -pair.Value);
                else msc.SourceTrace[trace][type] -= pair.Value;
            }
        }
#endif
        return msc;
    }

    public static MapleStatContainer operator -(MapleStatContainer self)
    {
        MapleStatContainer msc = new MapleStatContainer
        {
            MainStatType = self.MainStatType,
            SubStatType = self.SubStatType,
            SubStat2Type = self.SubStat2Type
        };
        foreach (var pair in self.statContainer)
        {
            msc.statContainer.TryAdd(pair.Key, -pair.Value);
        }

        return msc;
    }
    #endregion


}