using System.Collections;
using System.Diagnostics;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleStatContainer : IEnumerable<KeyValuePair<MapleStatus.StatusType, double>>, IDisposable
{
    #region Constructor

    protected internal static MapleStatContainer LoadFromJson(JsonObject itemJson)
    {
        MapleStatContainer msc = new MapleStatContainer
        {
            [MapleStatus.StatusType.STR] = ReadNumericData(itemJson, "str"),
            [MapleStatus.StatusType.DEX] = ReadNumericData(itemJson, "dex"),
            [MapleStatus.StatusType.INT] = ReadNumericData(itemJson, "int"),
            [MapleStatus.StatusType.LUK] = ReadNumericData(itemJson, "luk"),
            [MapleStatus.StatusType.HP] = ReadNumericData(itemJson, "max_hp"),
            [MapleStatus.StatusType.MP] = ReadNumericData(itemJson, "max_mp"),
            [MapleStatus.StatusType.ATTACK_POWER] = ReadNumericData(itemJson, "attack_power"),
            [MapleStatus.StatusType.MAGIC_POWER] = ReadNumericData(itemJson, "magic_power"),
            [MapleStatus.StatusType.BOSS_DAMAGE] = ReadNumericData(itemJson, "boss_damage"),
            [MapleStatus.StatusType.DAMAGE] = ReadNumericData(itemJson, "damage"),
            [MapleStatus.StatusType.IGNORE_DEF] = ReadNumericData(itemJson, "ignore_monster_armor"),
            [MapleStatus.StatusType.ALL_STAT_RATE] = ReadNumericData(itemJson, "all_stat"),
            [MapleStatus.StatusType.HP_RATE] = ReadNumericData(itemJson, "max_hp_rate"),
            [MapleStatus.StatusType.MP_RATE] = ReadNumericData(itemJson, "max_mp_rate")
        };
        return msc;
    }
    
    #endregion
    
    private Dictionary<MapleStatus.StatusType, double> statContainer = new();
    private bool isEmpty = true;
    public MapleStatus.StatusType MainStatType = MapleStatus.StatusType.OTHER;
    public MapleStatus.StatusType SubStatType = MapleStatus.StatusType.OTHER;
    public MapleStatus.StatusType SubStat2Type = MapleStatus.StatusType.OTHER;
    public int Level = 0;
    
    public double this[MapleStatus.StatusType type]
    {
        get => statContainer.GetValueOrDefault(type, 0);
        set
        {
            isEmpty = false;
            statContainer.TryAdd(type, 0);
            if (type is MapleStatus.StatusType.IGNORE_DEF or MapleStatus.StatusType.FINAL_DAMAGE)
                statContainer[type] = ApplyMultipleCalc(statContainer[type], value - statContainer[type]);
            else statContainer[type] = value;
        }
    }

    public MapleStatus.StatusType AttackFlatType => MainStatType == MapleStatus.StatusType.INT
        ? MapleStatus.StatusType.MAGIC_POWER
        : MapleStatus.StatusType.ATTACK_POWER;
    public MapleStatus.StatusType AttackRateType => MainStatType == MapleStatus.StatusType.INT
        ? MapleStatus.StatusType.MAGIC_RATE
        : MapleStatus.StatusType.ATTACK_RATE;

    public ulong GetPower(int correctAtk, bool isGenesis)
    {
        // Stat const
        double mainStatVar = Math.Floor(this[MainStatType] * (100 + this[MainStatType + 0x10]) / 100) + this[MainStatType + 0x20];
        double subStatVar = Math.Floor(this[SubStatType] * (100 + this[SubStatType + 0x10]) / 100) + this[SubStatType + 0x20];
        if (SubStat2Type != MapleStatus.StatusType.OTHER)
            subStatVar += Math.Floor(this[SubStat2Type] * (100 + this[SubStat2Type + 0x10]) / 100) + this[SubStat2Type + 0x20];

        double stat = (mainStatVar * 4.0 + subStatVar) / 100;
        double atk = Math.Floor((this[AttackFlatType] + correctAtk + 30) * (100 + this[AttackRateType]) / 100); // TODO : 30 = 여축/정축 미리 입력(스킬 기능 추가 시 제거)
        double dam = (100 + this[MapleStatus.StatusType.DAMAGE] + this[MapleStatus.StatusType.BOSS_DAMAGE]) / 100;
        double cdam = (135 + this[MapleStatus.StatusType.CRITICAL_DAMAGE]) / 100;
        double fdam = isGenesis ? 1.1 : 1.0;

        return (ulong) Math.Floor(stat * atk * dam * cdam * fdam);
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

    public void Clear()
    {
        isEmpty = true;
        statContainer.Clear();
    }
    
    public void Flush()
    {
        if (MainStatType != MapleStatus.StatusType.OTHER)
        {
            SafeAdd(MainStatType, ClearAndGet(MapleStatus.StatusType.MAIN_STAT));

            double mainStatFlat = ClearAndGet(MapleStatus.StatusType.MAIN_STAT_FLAT);
            if(MainStatType == MapleStatus.StatusType.HP)
                SafeAdd(MapleStatus.StatusType.HP, mainStatFlat);
            else if (false)
            {
            } // TODO: 제논의 경우 메인스텟을 반영하는 기타 로직이 필요 
            else
                SafeAdd(MainStatType+0x20, mainStatFlat);
        }

        if (SubStatType != MapleStatus.StatusType.OTHER)
        {
            double subStatValue = ClearAndGet(MapleStatus.StatusType.SUB_STAT);
            SafeAdd(SubStatType, subStatValue);
            if (SubStat2Type != MapleStatus.StatusType.OTHER)
                SafeAdd(SubStat2Type, subStatValue);
        }

        double strdexluk = ClearAndGet(MapleStatus.StatusType.STR_DEX_LUK);
        if (strdexluk != 0.0)
        {
            SafeAdd(MapleStatus.StatusType.STR_FLAT, strdexluk);
            SafeAdd(MapleStatus.StatusType.DEX_FLAT, strdexluk);
            SafeAdd(MapleStatus.StatusType.LUK_FLAT, strdexluk);
        }

        double allStatFlat = ClearAndGet(MapleStatus.StatusType.ALL_STAT);
        if (allStatFlat != 0.0)
        {
            SafeAdd(MapleStatus.StatusType.STR, allStatFlat);
            SafeAdd(MapleStatus.StatusType.DEX, allStatFlat);
            SafeAdd(MapleStatus.StatusType.INT, allStatFlat);
            SafeAdd(MapleStatus.StatusType.LUK, allStatFlat);
        }

        double allStatRate = ClearAndGet(MapleStatus.StatusType.ALL_STAT_RATE);
        if (allStatRate != 0.0)
        {
            SafeAdd(MapleStatus.StatusType.STR_RATE, allStatRate);
            SafeAdd(MapleStatus.StatusType.DEX_RATE, allStatRate);
            SafeAdd(MapleStatus.StatusType.INT_RATE, allStatRate);
            SafeAdd(MapleStatus.StatusType.LUK_RATE, allStatRate);
        }

        double atkmag = ClearAndGet(MapleStatus.StatusType.ATTACK_AND_MAGIC);
        if (atkmag != 0.0)
        {
            SafeAdd(MapleStatus.StatusType.ATTACK_POWER, atkmag);
            SafeAdd(MapleStatus.StatusType.MAGIC_POWER, atkmag);
        }

        // if (Level > 0)
        // {
        //     MapleStatus.StatusType[] perLevels = {
        //         MapleStatus.StatusType.STR_PER_LEVEL, MapleStatus.StatusType.DEX_PER_LEVEL,
        //         MapleStatus.StatusType.INT_PER_LEVEL, MapleStatus.StatusType.LUK_PER_LEVEL,
        //     };
        //     foreach (MapleStatus.StatusType perLvType in perLevels)
        //     {
        //         double perValue = ClearAndGet(perLvType);
        //         if(perValue == 0.0) continue;
        //         // Console.WriteLine($"{perLvType} => {perValue}");
        //         SafeAdd(perLvType - 0x30, Math.Floor(perValue * (Level / 9.0)));
        //     }
        // }
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
    
    public IEnumerator<KeyValuePair<MapleStatus.StatusType, double>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<MapleStatus.StatusType, double>>) statContainer).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public static MapleStatContainer operator +(MapleStatContainer lhs, MapleStatContainer rhs)
    {
        MapleStatContainer msc = new MapleStatContainer
        {
            MainStatType = lhs.MainStatType,
            SubStatType = lhs.SubStatType,
            SubStat2Type = lhs.SubStat2Type,
            Level = lhs.Level
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
        
        return msc;
    }
    
    public static MapleStatContainer operator -(MapleStatContainer lhs, MapleStatContainer rhs)
    {
        MapleStatContainer msc = new MapleStatContainer
        {
            MainStatType = lhs.MainStatType,
            SubStatType = lhs.SubStatType,
            SubStat2Type = lhs.SubStat2Type,
            Level = lhs.Level
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
        return msc;
    }

    public static MapleStatContainer operator -(MapleStatContainer self)
    {
        MapleStatContainer msc = new MapleStatContainer
        {
            MainStatType = self.MainStatType,
            SubStatType = self.SubStatType,
            SubStat2Type = self.SubStat2Type,
            Level = self.Level
        };
        foreach (var pair in self.statContainer)
        {
            msc.statContainer.TryAdd(pair.Key, -pair.Value);
        }

        return msc;
    }
    #endregion

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public void Dispose()
    {
        
    }
}