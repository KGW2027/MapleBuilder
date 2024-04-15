using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item.Interface;

namespace MapleBuilder.Control.Data.Item.Option;

public static class AddOptions
{
    public enum AddOptionType
    {
        STR, DEX, INT, LUK,
        STR_DEX, STR_INT, STR_LUK,
        DEX_INT, DEX_LUK, INT_LUK,
        HP, MP, ATTACK, MAGIC, BOSS_DAMAGE, DAMAGE, ALL_STAT
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

    public static Dictionary<AddOptionType, int> ParseAddOption(this CommonItem item, MapleStatContainer msc)
    {
        Dictionary<AddOptionType, int> addOptions = new Dictionary<AddOptionType, int>();

        
        
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
        return (int) Math.Ceiling((Math.Floor(level / 40.0) + 1) * grade * Math.Pow(1.1, Math.Max(grade - 3, 0)) * baseAtk);
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
        return (isDoubleStat ? (int) (Math.Floor(baseLevel / 20.0) + 1) : (int) (Math.Floor(baseLevel / 40.0) + 1)) * grade;
    }

    public static int GetAllStatOption(this CommonItem item, int grade)
    {
        return 1 * grade;
    }

    public static int GetHpMpOption(this CommonItem item, int grade)
    {
        return item.EquipData!.Level * 3 * grade;
    } 
}