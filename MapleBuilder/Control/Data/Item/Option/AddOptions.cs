using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item.Option;

public static class AddOptions
{
    [Flags]
    public enum AddOptionType
    {
         STR        = 1 << 0 
        ,DEX        = 1 << 1
        ,INT        = 1 << 2
        ,LUK        = 1 << 3
        ,STR_DEX    = STR | DEX
        ,STR_INT    = STR | INT
        ,STR_LUK    = STR | LUK
        ,DEX_INT    = DEX | INT
        ,DEX_LUK    = DEX | LUK
        ,INT_LUK    = INT | LUK
        ,HP         = 1 << 4
        ,MP         = HP + 1
        ,ATTACK     = HP + 2
        ,MAGIC      = HP + 3
        ,BOSS_DAMAGE= HP + 4
        ,DAMAGE     = HP + 5
        ,ALL_STAT   = HP + 6
    }

    private static readonly AddOptionType[] WEAPON_OPTIONS = {
        AddOptionType.STR, AddOptionType.DEX, AddOptionType.INT, AddOptionType.LUK,
        AddOptionType.STR_DEX, AddOptionType.STR_INT, AddOptionType.STR_LUK,
        AddOptionType.DEX_INT, AddOptionType.DEX_LUK, AddOptionType.INT_LUK,
        AddOptionType.HP, AddOptionType.MP, AddOptionType.ATTACK, AddOptionType.MAGIC,
        AddOptionType.BOSS_DAMAGE, AddOptionType.DAMAGE, AddOptionType.ALL_STAT
    };
    
    private static readonly AddOptionType[] ARMOR_OPTIONS = {
        AddOptionType.STR, AddOptionType.DEX, AddOptionType.INT, AddOptionType.LUK,
        AddOptionType.STR_DEX, AddOptionType.STR_INT, AddOptionType.STR_LUK,
        AddOptionType.DEX_INT, AddOptionType.DEX_LUK, AddOptionType.INT_LUK,
        AddOptionType.HP, AddOptionType.MP, AddOptionType.ATTACK, AddOptionType.MAGIC, AddOptionType.ALL_STAT
    };

    private static readonly AddOptionType[] STATS =
    {
        AddOptionType.STR, AddOptionType.DEX, AddOptionType.INT, AddOptionType.LUK,
        AddOptionType.STR_DEX, AddOptionType.STR_INT, AddOptionType.STR_LUK,
        AddOptionType.DEX_INT, AddOptionType.DEX_LUK, AddOptionType.INT_LUK
    };

    private static IEnumerable<IEnumerable<T>> GetCombinations<T>(T[] list, int rem)
    {
        var combinations = new List<List<T>>();

        foreach (T stat in list)
            combinations.Add(new List<T> {stat});

        for (int i = 0; i < rem - 1; i++)
        {
            var newCombinations = new List<List<T>>();
            foreach (var combination in combinations)
            {
                foreach (T nextStat in list)
                {
                    var newCombination = new List<T>(combination)
                    {
                        nextStat
                    };
                    newCombinations.Add(newCombination);
                }
            }
            combinations = newCombinations;
        }
        return combinations;
    }

    private static bool StatAnalysis(this CommonItem item, Dictionary<AddOptionType, int> dict, int str, int dex,
        int lnt, int luk, int rem)
    {
        var statCombinations = GetCombinations(STATS, rem).ToArray();
        var grades = GetCombinations(new[] {1, 2, 3, 4, 5, 6, 7}, rem).ToArray();
        int singleSt = item.GetStatOption(1, false);
        int doubleSt = item.GetStatOption(1, true);
        
        foreach (var v in statCombinations)
        {
            // Zero Test
            bool bClearZeroTest = true;
            var addOptionTypes = v as AddOptionType[] ?? v.ToArray();
            foreach (AddOptionType type in addOptionTypes)
            {
                if ((type & AddOptionType.STR) == AddOptionType.STR && str == 0) bClearZeroTest = false;
                if ((type & AddOptionType.DEX) == AddOptionType.DEX && dex == 0) bClearZeroTest = false;
                if ((type & AddOptionType.INT) == AddOptionType.INT && lnt == 0) bClearZeroTest = false;
                if ((type & AddOptionType.LUK) == AddOptionType.LUK && luk == 0) bClearZeroTest = false;
            }
            if (!bClearZeroTest) continue;
            
            // Duplicate Test
            bool bClearDuplicateTest = true;
            for (int idx = 0; idx < addOptionTypes.Length; idx++)
            {
                for (int jdx = idx+1; jdx < addOptionTypes.Length; jdx++)
                {
                    if (addOptionTypes[idx] == addOptionTypes[jdx]) bClearDuplicateTest = false;
                }
            }

            if (!bClearDuplicateTest) continue;
            
            // Test Status
            foreach (var enumerable in grades)
            {
                int[] levels = enumerable.ToArray();
                int testStr, testDex, testInt, testLuk;
                testStr = testDex = testInt = testLuk = 0;
                for (int i = 0; i < rem; i++)
                {
                    switch (addOptionTypes[i])
                    {
                        case AddOptionType.STR:
                            testStr += singleSt * levels[i];
                            break;
                        case AddOptionType.DEX:
                            testDex += singleSt * levels[i];
                            break;
                        case AddOptionType.INT:
                            testInt += singleSt * levels[i];
                            break;
                        case AddOptionType.LUK:
                            testLuk += singleSt * levels[i];
                            break;
                        case AddOptionType.STR_DEX:
                            testStr += doubleSt * levels[i];
                            testDex += doubleSt * levels[i];
                            break;
                        case AddOptionType.STR_INT:
                            testStr += doubleSt * levels[i];
                            testInt += doubleSt * levels[i];
                            break;
                        case AddOptionType.STR_LUK:
                            testStr += doubleSt * levels[i];
                            testLuk += doubleSt * levels[i];
                            break;
                        case AddOptionType.DEX_INT:
                            testDex += doubleSt * levels[i];
                            testInt += doubleSt * levels[i];
                            break;
                        case AddOptionType.DEX_LUK:
                            testDex += doubleSt * levels[i];
                            testLuk += doubleSt * levels[i];
                            break;
                        case AddOptionType.INT_LUK:
                            testInt += doubleSt * levels[i];
                            testLuk += doubleSt * levels[i];
                            break;
                    }
                }

                if (testStr != str || testDex != dex || testInt != lnt || testLuk != luk) continue;
                for (int i = 0; i < rem; i++)
                    dict.TryAdd(addOptionTypes[i], levels[i]);
                return true;
            }
        }

        return false;
    }

    public static Dictionary<AddOptionType, int> ParseAddOption(this CommonItem item, MapleStatContainer msc)
    {
        Dictionary<AddOptionType, int> addOptions = new Dictionary<AddOptionType, int>();

        if (msc[MapleStatus.StatusType.ALL_STAT_RATE] > 0)
            addOptions.TryAdd(AddOptionType.ALL_STAT, (int) msc[MapleStatus.StatusType.ALL_STAT_RATE]);
        if (msc[MapleStatus.StatusType.DAMAGE] > 0)
            addOptions.TryAdd(AddOptionType.DAMAGE, (int) msc[MapleStatus.StatusType.DAMAGE]);
        if (msc[MapleStatus.StatusType.BOSS_DAMAGE] > 0)
            addOptions.TryAdd(AddOptionType.BOSS_DAMAGE, (int) msc[MapleStatus.StatusType.BOSS_DAMAGE] / 2);
        if (msc[MapleStatus.StatusType.HP] > 0)
            addOptions.TryAdd(AddOptionType.HP, (int) (msc[MapleStatus.StatusType.HP] / item.GetHpMpOption(1)));
        if (msc[MapleStatus.StatusType.MP] > 0)
            addOptions.TryAdd(AddOptionType.MP, (int) (msc[MapleStatus.StatusType.MP] / item.GetHpMpOption(1)));

        if (item.EquipType is MapleEquipType.EquipType.WEAPON)
        {
            for (int grade = 1; grade <= 7; grade++)
            {
                if ((int) msc[MapleStatus.StatusType.ATTACK_POWER] == item.GetAttackOption(grade, true))
                    addOptions.TryAdd(AddOptionType.ATTACK, grade);
                if ((int) msc[MapleStatus.StatusType.MAGIC_POWER] == item.GetAttackOption(grade, false))
                    addOptions.TryAdd(AddOptionType.MAGIC, grade);
            }
        }
        else
        {
            if (msc[MapleStatus.StatusType.ATTACK_POWER] > 0)
                addOptions.TryAdd(AddOptionType.ATTACK, (int) msc[MapleStatus.StatusType.ATTACK_POWER]);
            if (msc[MapleStatus.StatusType.MAGIC_POWER] > 0)
                addOptions.TryAdd(AddOptionType.MAGIC, (int) msc[MapleStatus.StatusType.MAGIC_POWER]);
        }

        int str = (int) msc[MapleStatus.StatusType.STR];
        int dex = (int) msc[MapleStatus.StatusType.DEX];
        int lnt = (int) msc[MapleStatus.StatusType.INT];
        int luk = (int) msc[MapleStatus.StatusType.LUK];
        int remSlot = 4 - addOptions.Count;
        
        // Console.WriteLine($"[추가옵션 분석 :: {item.UniqueName}] 남은 슬롯 - {remSlot}, STR {str}, DEX {dex}, INT {lnt}, LUK {luk}");
        // 스텟 분석
        do if (item.StatAnalysis(addOptions, str, dex, lnt, luk, remSlot)) break;
        while (--remSlot > 0);
        
        return addOptions;
    }

    public static AddOptionType[] GetAddOptionTypes(this CommonItem item)
    {
        return item.EquipType == MapleEquipType.EquipType.WEAPON ? WEAPON_OPTIONS : ARMOR_OPTIONS;
    }

    public static int GetAttackOption(this CommonItem item, int grade, bool isAttack = true)
    {
        if (item.EquipType != MapleEquipType.EquipType.WEAPON) return 1 * grade;
        int level = item.EquipData!.Level;
        int baseAtk = isAttack
            ? item.EquipData[MapleStatus.StatusType.ATTACK_POWER]
            : item.EquipData[MapleStatus.StatusType.MAGIC_POWER];

        double levelVal = Math.Floor(level / 40.0) + 1;
        double gradeVal = grade;
        double constVal = Math.Pow(1.1, Math.Max(grade - 3, 0));
        return (int) Math.Ceiling(levelVal * gradeVal * constVal / 100.0 * baseAtk);
    }

    public static int GetDamageOption(this CommonItem item, int grade)
    {
        return 1 * grade;
    }

    public static int GetBossDamageOption(this CommonItem item, int grade)
    {
        return 2 * grade;
    }

    public static int GetStatOption(this CommonItem item, int grade, bool isDoubleStat)
    {
        int baseLevel = item.EquipData!.Level;
        if (!isDoubleStat && baseLevel == 250) baseLevel = 220;
        return (!isDoubleStat ? (int) (Math.Floor(baseLevel / 20.0) + 1) : (int) (Math.Floor(baseLevel / 40.0) + 1)) * grade;
    }

    public static int GetAllStatOption(this CommonItem item, int grade)
    {
        return 1 * grade;
    }

    public static int GetHpMpOption(this CommonItem item, int grade)
    {
        return item.EquipData!.Level * 3 * grade;
    }

    public static int GetOptionStatus(this AddOptionType optionType, CommonItem item, int grade)
    {
        switch (optionType)
        {
            case AddOptionType.STR:
            case AddOptionType.DEX:
            case AddOptionType.INT:
            case AddOptionType.LUK:
                return item.GetStatOption(grade, false);
            case AddOptionType.STR_DEX:
            case AddOptionType.STR_INT:
            case AddOptionType.STR_LUK:
            case AddOptionType.DEX_INT:
            case AddOptionType.DEX_LUK:
            case AddOptionType.INT_LUK:
                return item.GetStatOption(grade, true);
            case AddOptionType.HP:
            case AddOptionType.MP:
                return item.GetHpMpOption(grade);
            case AddOptionType.ATTACK:
            case AddOptionType.MAGIC:
                return item.GetAttackOption(grade, optionType == AddOptionType.ATTACK);
            case AddOptionType.BOSS_DAMAGE:
                return item.GetBossDamageOption(grade);
            case AddOptionType.DAMAGE:
                return item.GetDamageOption(grade);
            case AddOptionType.ALL_STAT:
                return item.GetAllStatOption(grade);
            default:
                return 0;
        }
    }
}