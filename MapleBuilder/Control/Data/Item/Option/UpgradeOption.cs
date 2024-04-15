using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item.Option;

public static class UpgradeOption
{
    public enum UpgradeType
    {
         NONE                = 0x00
        ,SPELL_TRACE_100     = 0x01
        ,SPELL_TRACE_70      = 0x02
        ,SPELL_TRACE_30      = 0x03
        ,SPELL_TRACE_15      = 0x04
        ,DOMINATOR_FRAGMENT  = 0x05
        ,EARRING_INT_10      = 0x06
        ,HEART               = 0x07
        ,CHAOS               = 0x08
        ,PREMIUM_ACCESSORY_4 = 0x11
        ,PREMIUM_ACCESSORY_5 = 0x12
        ,ACCESSORY_2         = 0x21
        ,ACCESSORY_3         = 0x22
        ,ACCESSORY_4         = 0x23
        ,ACCESSORY_70_1      = 0x31
        ,ACCESSORY_70_2      = 0x32
        ,MAGICAL_9           = 0x41
        ,MAGICAL_10          = 0x42
        ,MAGICAL_11          = 0x43
        ,MIRACLE_50_2        = 0x51
        ,MIRACLE_50_3        = 0x52
        ,ARMOR_70_1          = 0x61
        ,ARMOR_70_2          = 0x62
        ,PREMIUM_PET_4       = 0x71
        ,PREMIUM_PET_5       = 0x72
        ,PET_2               = 0x81
        ,PET_3               = 0x82
        ,PET_4               = 0x83
        ,PET_70_1            = 0x91
        ,PET_70_2            = 0x92
    }

    private static bool IsWeapon(this MapleEquipType.EquipType equipType)
    {
        return equipType is MapleEquipType.EquipType.WEAPON or MapleEquipType.EquipType.SUB_WEAPON;
    }

    private static bool IsArmor(this MapleEquipType.EquipType equipType)
    {
        return equipType is MapleEquipType.EquipType.HELMET
            or MapleEquipType.EquipType.TOP or MapleEquipType.EquipType.BOTTOM or MapleEquipType.EquipType.TOP_BOTTOM
            or MapleEquipType.EquipType.BOOT or MapleEquipType.EquipType.CAPE or MapleEquipType.EquipType.SHOULDER
            or MapleEquipType.EquipType.GLOVE;
    }

    private static bool IsAccessory(this MapleEquipType.EquipType equipType)
    {
        return equipType is MapleEquipType.EquipType.EYE or MapleEquipType.EquipType.FACE
            or MapleEquipType.EquipType.EARRING or MapleEquipType.EquipType.RING
            or MapleEquipType.EquipType.PENDANT or MapleEquipType.EquipType.BELT;
    }

    private static double GetRangeValue(this int level, int low, int mid, int high)
    {
        return level <= 70 ? low : level <= 110 ? mid : high;
    }

    private static void Simulate(this MapleStatContainer msc, int level, MapleEquipType.EquipType equipType,
        UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.SPELL_TRACE_100:
                if (equipType.IsWeapon() || equipType is MapleEquipType.EquipType.HEART)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(1, 2, 3);
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(0, 0, 1);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(0, 0, 50);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(0, 1, 1);
                }
                else if (equipType.IsArmor())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(1, 2, 3);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(55, 120, 180);
                }
                else if (equipType.IsAccessory())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(1, 1, 2);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(50, 50, 100);
                }

                break;
            case UpgradeType.SPELL_TRACE_70:
                if (equipType.IsWeapon() || equipType is MapleEquipType.EquipType.HEART)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(2, 3, 5);
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(0, 1, 2);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(0, 50, 100);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(1, 2, 2);
                }
                else if (equipType.IsArmor())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(2, 3, 4);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(115, 190, 270);
                }
                else if (equipType.IsAccessory())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(1, 1, 2);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(50, 50, 100);
                }

                break;
            case UpgradeType.SPELL_TRACE_30:
                if (equipType.IsWeapon() || equipType is MapleEquipType.EquipType.HEART)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(3, 5, 7);
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(1, 2, 3);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(50, 100, 150);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(2, 3, 3);
                }
                else if (equipType.IsArmor())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(3, 5, 7);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(180, 320, 470);
                }
                else if (equipType.IsAccessory())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(3, 4, 5);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(150, 200, 250);
                }

                break;
            case UpgradeType.SPELL_TRACE_15:
                if (equipType.IsWeapon())
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(5, 7, 9);
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(2, 3, 4);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(100, 150, 200);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += level.GetRangeValue(3, 4, 4);
                }
                else if (equipType.IsArmor())
                {
                    msc[MapleStatus.StatusType.ALL_STAT] += level.GetRangeValue(4, 7, 10);
                    msc[MapleStatus.StatusType.HP] += level.GetRangeValue(245, 460, 670);
                }

                break;
            case UpgradeType.PREMIUM_ACCESSORY_4:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 4;
                break;
            case UpgradeType.PREMIUM_ACCESSORY_5:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 5;
                break;
            case UpgradeType.ACCESSORY_2:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 2;
                break;
            case UpgradeType.ACCESSORY_3:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
                break;
            case UpgradeType.ACCESSORY_4:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 4;
                break;
            case UpgradeType.ACCESSORY_70_1:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
                break;
            case UpgradeType.ACCESSORY_70_2:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 2;
                break;
            case UpgradeType.DOMINATOR_FRAGMENT:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                msc[MapleStatus.StatusType.HP] += 40;
                break;
            case UpgradeType.MAGICAL_9:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 9;
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                break;
            case UpgradeType.MAGICAL_10:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                break;
            case UpgradeType.MAGICAL_11:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 11;
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                break;
            case UpgradeType.EARRING_INT_10:
                msc[MapleStatus.StatusType.INT] += 3;
                msc[MapleStatus.StatusType.MAGIC_POWER] += 5;
                break;
            case UpgradeType.MIRACLE_50_2:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 2;
                break;
            case UpgradeType.MIRACLE_50_3:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
                break;
            case UpgradeType.ARMOR_70_1:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
                break;
            case UpgradeType.ARMOR_70_2:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 2;
                break;
            case UpgradeType.HEART:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 9;
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                break;
            case UpgradeType.PREMIUM_PET_4:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 4;
                break;
            case UpgradeType.PREMIUM_PET_5:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 5;
                break;
            case UpgradeType.PET_2:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 2;
                break;
            case UpgradeType.PET_3:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
                break;
            case UpgradeType.PET_4:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 4;
                break;
            case UpgradeType.PET_70_1:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
                break;
            case UpgradeType.PET_70_2:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 2;
                break;
            case UpgradeType.CHAOS:
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 6;
                msc[MapleStatus.StatusType.ALL_STAT] += 6;
                msc[MapleStatus.StatusType.HP] += 60;
                break;
            default:
                break;
        }
    }

    private static bool Compare(this MapleStatContainer msc, MapleStatContainer target)
    {
        foreach (var pair in target)
        {
            if (pair.Key is not (MapleStatus.StatusType.STR or MapleStatus.StatusType.DEX or MapleStatus.StatusType.INT
                or MapleStatus.StatusType.LUK or MapleStatus.StatusType.HP or MapleStatus.StatusType.ATTACK_POWER
                or MapleStatus.StatusType.MAGIC_POWER)) continue;
            if (msc[pair.Key] < pair.Value) return false;
        }

        return true;
    }

    private static UpgradeType[] MakeUpgradeTypes(int size, params UpgradeType[] upgrades)
    {
        UpgradeType[] arr = new UpgradeType[size];
        for (int idx = 0; idx < arr.Length; idx++)
            arr[idx] = idx < upgrades.Length ? upgrades[idx] : upgrades[^1];
        return arr;
    }

    private static UpgradeType[] TestPresets(int itemLevel, int upgCnt, MapleEquipType.EquipType equipType,
        MapleStatContainer target, List<UpgradeType> candidates)
    {
        MapleStatContainer self = new MapleStatContainer();
        
        // Single Upgrade
        foreach (UpgradeType single in candidates)
        {
            for(int upg = 0 ; upg < upgCnt ; upg++) self.Simulate(itemLevel, equipType, single);
            if(single <= UpgradeType.SPELL_TRACE_15) self[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
            self.Flush();
            if (self.Compare(target)) return MakeUpgradeTypes(upgCnt, single);
            self.Clear();
        }
        
        // Chaos 1 + Single
        foreach (UpgradeType single in candidates)
        {
            self.Simulate(itemLevel, equipType, UpgradeType.CHAOS);
            for(int upg = 1 ; upg < upgCnt ; upg++) self.Simulate(itemLevel, equipType, single);
            if(single <= UpgradeType.SPELL_TRACE_15) self[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
            self.Flush();
            if (self.Compare(target)) return MakeUpgradeTypes(upgCnt, UpgradeType.CHAOS, single);
            self.Clear();
        }
        
        return MakeUpgradeTypes(upgCnt, UpgradeType.CHAOS);
    }

    private static void SortUpgradeTypes(UpgradeType[] types, MapleStatContainer msc)
    {
        int total = 0;
        int maxAtk = (int) Math.Max(msc[MapleStatus.StatusType.ATTACK_POWER], msc[MapleStatus.StatusType.MAGIC_POWER]);
        foreach (var type in types)
        {
            if (type == UpgradeType.CHAOS)
            {
                total += 6;
                continue;
            }
            if ((int) type <= 0x10) continue;
            total += type.ToString()[^1] - '0';
        }

        int idx = types.Length - 1;
        while (total > maxAtk && idx >= 0)
        {
            if (types[idx] == UpgradeType.CHAOS || types[idx] == UpgradeType.NONE || ((int) types[idx] & 0xF) == 1)
            {
                idx--;
                continue;
            }

            types[idx]--;
            total--;
        }
    }

    private static Dictionary<MapleStatus.StatusType, double> GetChaosAverage(MapleStatContainer msc, int level, 
        MapleEquipType.EquipType equipType, UpgradeType[] upgs)
    {
        Dictionary<MapleStatus.StatusType, double> dict = new Dictionary<MapleStatus.StatusType, double>();
        MapleStatContainer expectChaos = new MapleStatContainer();

        int chaos = upgs.Length;
        int traceCnt = 0;
        foreach (var type in upgs)
        {
            if (type == UpgradeType.CHAOS) continue;
            chaos--;
            expectChaos.Simulate(level, equipType, type);
            if (type is <= UpgradeType.SPELL_TRACE_15 and > UpgradeType.NONE && ++traceCnt == 4)
                expectChaos[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
        }

        int statIncrease = (int) expectChaos[MapleStatus.StatusType.ALL_STAT];
        int atkIncrease = (int) expectChaos[MapleStatus.StatusType.ATTACK_AND_MAGIC];
        // int hpIncrease = (int) expectChaos[MapleStatus.StatusType.HP];
        expectChaos.Flush();
        msc -= expectChaos;

        foreach (var pair in msc)
        {
            double value = pair.Value;
            switch (pair) // 올스텟, 공마 동시 적용으로 인한 추가 보정
            {
                case {Value: < 0, Key: MapleStatus.StatusType.STR or MapleStatus.StatusType.DEX
                    or MapleStatus.StatusType.INT or MapleStatus.StatusType.LUK}:
                    value += statIncrease;
                    break;
                case {Value: < 0, Key: MapleStatus.StatusType.ATTACK_POWER or MapleStatus.StatusType.MAGIC_POWER}:
                    value += atkIncrease;
                    break;
                case {Value: < 0, Key: MapleStatus.StatusType.HP}:
                    value = expectChaos[MapleStatus.StatusType.MP];
                    break;
            }
            
            dict.TryAdd(pair.Key, value / chaos);
        }
        
        
        return dict;
    }

    public static UpgradeType[] ParseUpgrades(this CommonItem item, MapleStatContainer msc, out Dictionary<MapleStatus.StatusType, double>? chaosAvg)
    {
        List<UpgradeType> candidateTypes = new List<UpgradeType>
        {
            UpgradeType.SPELL_TRACE_100,
            UpgradeType.SPELL_TRACE_70,
            UpgradeType.SPELL_TRACE_30
        };

        if (item.EquipType.IsWeapon() || item.EquipType.IsArmor())
        {
            candidateTypes.Add(UpgradeType.SPELL_TRACE_15);

            if (item.EquipType is not MapleEquipType.EquipType.WEAPON and MapleEquipType.EquipType.SUB_WEAPON)
            {
                candidateTypes.Add(UpgradeType.ARMOR_70_2);
                candidateTypes.Add(UpgradeType.MIRACLE_50_3);
            }
        }

        if (item.EquipType.IsAccessory())
        {
            candidateTypes.Add(UpgradeType.ACCESSORY_70_2);
            candidateTypes.Add(UpgradeType.ACCESSORY_4);
            candidateTypes.Add(UpgradeType.PREMIUM_ACCESSORY_5);
        }

        if (item.EquipType is MapleEquipType.EquipType.EARRING)
        {
            candidateTypes.Add(UpgradeType.EARRING_INT_10);
        }

        if (item.EquipType.IsWeapon() || item.EquipType is MapleEquipType.EquipType.HEART)
        {
            candidateTypes.Add(UpgradeType.MAGICAL_11);
        }

        if (item.EquipType is MapleEquipType.EquipType.HEART)
        {
            candidateTypes.Add(UpgradeType.HEART);
        }

        if (item.EquipType is MapleEquipType.EquipType.PENDANT && item.UniqueName.Equals("도미네이터 펜던트"))
        {
            candidateTypes.Add(UpgradeType.DOMINATOR_FRAGMENT);
        }
        
        int upgradeCount = item.MaxUpgradeCount!.Value - item.RemainUpgradeCount!.Value;
        UpgradeType[] result = MakeUpgradeTypes(item.MaxUpgradeCount!.Value, UpgradeType.NONE);
        UpgradeType[] found = TestPresets(item.ItemLevel, upgradeCount, item.EquipType, msc, candidateTypes);
        for (int idx = 0; idx < found.Length; idx++)
            result[idx] = found[idx];
        SortUpgradeTypes(result, msc);

        chaosAvg = GetChaosAverage(msc, item.ItemLevel, item.EquipType, result);
        return result;
    }
}