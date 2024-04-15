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
         
         // 주흔작
        ,SPELL_TRACE_STR_100     = 0x01
        ,SPELL_TRACE_STR_70      = 0x02
        ,SPELL_TRACE_STR_30      = 0x03
        ,SPELL_TRACE_STR_15      = 0x04
        ,SPELL_TRACE_DEX_100     = 0x05
        ,SPELL_TRACE_DEX_70      = 0x06
        ,SPELL_TRACE_DEX_30      = 0x07
        ,SPELL_TRACE_DEX_15      = 0x08
        ,SPELL_TRACE_INT_100     = 0x09
        ,SPELL_TRACE_INT_70      = 0x0A
        ,SPELL_TRACE_INT_30      = 0x0B
        ,SPELL_TRACE_INT_15      = 0x0C
        ,SPELL_TRACE_LUK_100     = 0x0D
        ,SPELL_TRACE_LUK_70      = 0x0E
        ,SPELL_TRACE_LUK_30      = 0x0F
        ,SPELL_TRACE_LUK_15      = 0x10
        ,SPELL_TRACE_HP_100      = 0x11
        ,SPELL_TRACE_HP_70       = 0x12
        ,SPELL_TRACE_HP_30       = 0x13
        ,SPELL_TRACE_HP_15       = 0x14
        ,SPELL_TRACE_ALL_100     = 0x15
        ,SPELL_TRACE_ALL_70      = 0x16
        ,SPELL_TRACE_ALL_30      = 0x17
        ,SPELL_TRACE_ALL_15      = 0x18
        
        // 파편작
        ,DOMINATOR_FRAGMENT      = 0x19
        
        // 귀장식 지력
        ,EARRING_INT_10          = 0x1A
        
        // 혼돈의 주문서
        ,CHAOS                   = 0x1C
        
        // 프악공
        ,PREMIUM_ACCESSORY_ATK_4 = 0x31
        ,PREMIUM_ACCESSORY_ATK_5 = 0x32
        
        // 프악마
        ,PREMIUM_ACCESSORY_MAG_4 = 0x41
        ,PREMIUM_ACCESSORY_MAG_5 = 0x42
        
        // 악세서리 공격력 스크롤
        ,ACCESSORY_ATK_2         = 0x51
        ,ACCESSORY_ATK_3         = 0x52
        ,ACCESSORY_ATK_4         = 0x53
        
        // 악세서리 마력 스크롤
        ,ACCESSORY_MAG_2         = 0x61
        ,ACCESSORY_MAG_3         = 0x62
        ,ACCESSORY_MAG_4         = 0x63
        
        // 악세서리 공격력 주문서 70%
        ,ACCESSORY_70_ATK_1      = 0x71
        ,ACCESSORY_70_ATK_2      = 0x72
        
        // 악세서리 마력 주문서 70%
        ,ACCESSORY_70_MAG_1      = 0x81
        ,ACCESSORY_70_MAG_2      = 0x82
        
        // 매지컬 공격력 주문서
        ,MAGICAL_ATK_9           = 0x91
        ,MAGICAL_ATK_10          = 0x92
        ,MAGICAL_ATK_11          = 0x93
        
        // 매지컬 마력 주문서
        ,MAGICAL_MAG_9           = 0xA1
        ,MAGICAL_MAG_10          = 0xA2
        ,MAGICAL_MAG_11          = 0xA3
        
        // 미라클 방어구 공격력 50%
        ,MIRACLE_50_ATK_2        = 0xB1
        ,MIRACLE_50_ATK_3        = 0xB2
        
        // 미라클 방어구 마력 50%
        ,MIRACLE_50_MAG_2        = 0xC1
        ,MIRACLE_50_MAG_3        = 0xC2
        
        // 방어구 공격력 주문서 70%
        ,ARMOR_70_ATK_1          = 0xD1
        ,ARMOR_70_ATK_2          = 0xD2
        
        // 방어구 마력 주문서 70%
        ,ARMOR_70_MAG_1          = 0xE1
        ,ARMOR_70_MAG_2          = 0xE2
        
        // 프리미엄 펫장비 공격력 
        ,PREMIUM_PET_ATK_4       = 0xF1
        ,PREMIUM_PET_ATK_5       = 0xF2
        
        // 프리미엄 펫장비 마력 
        ,PREMIUM_PET_MAG_4       = 0x101
        ,PREMIUM_PET_MAG_5       = 0x102
        
        // 펫장비 공격력 스크롤
        ,PET_ATK_2               = 0x111
        ,PET_ATK_3               = 0x112
        ,PET_ATK_4               = 0x113
        
        // 펫장비 마력 스크롤
        ,PET_MAG_2               = 0x121
        ,PET_MAG_3               = 0x122
        ,PET_MAG_4               = 0x123
        
        // 펫장비 공격력 주문서 70%
        ,PET_70_ATK_1            = 0x131
        ,PET_70_ATK_2            = 0x132
        
        // 펫장비 마력 주문서 70%
        ,PET_70_MAG_1            = 0x141
        ,PET_70_MAG_2            = 0x142
        
        // 하트 신규 주문서
        ,HEART_ATK               = 0x151
        ,HEART_MAG               = 0x152
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
        MapleStatus.StatusType atkType = MapleStatus.StatusType.OTHER, statType = MapleStatus.StatusType.OTHER;
        if (type.IsSpellTrace())
        {
            atkType = type is >= UpgradeType.SPELL_TRACE_INT_100 and <= UpgradeType.SPELL_TRACE_INT_15
                ? MapleStatus.StatusType.MAGIC_POWER
                : MapleStatus.StatusType.ATTACK_POWER;
            statType = type switch
            {
                >= UpgradeType.SPELL_TRACE_STR_100 and <= UpgradeType.SPELL_TRACE_STR_15 => MapleStatus.StatusType.STR,
                <= UpgradeType.SPELL_TRACE_DEX_15 => MapleStatus.StatusType.DEX,
                <= UpgradeType.SPELL_TRACE_INT_15 => MapleStatus.StatusType.INT,
                <= UpgradeType.SPELL_TRACE_LUK_15 => MapleStatus.StatusType.LUK,
                <= UpgradeType.SPELL_TRACE_HP_15  => MapleStatus.StatusType.HP,
                <= UpgradeType.SPELL_TRACE_ALL_15 => MapleStatus.StatusType.ALL_STAT,
                _ => MapleStatus.StatusType.OTHER
            };
        }
        else
        {
            atkType = type.ToString().Contains("ATK")
                ? MapleStatus.StatusType.ATTACK_POWER
                : MapleStatus.StatusType.MAGIC_POWER;
        }

        bool isHP = statType == MapleStatus.StatusType.HP;
        bool isAll = statType == MapleStatus.StatusType.ALL_STAT;

        switch (type)
        {
            case UpgradeType.NONE:
                break;
            case UpgradeType.SPELL_TRACE_STR_100:
            case UpgradeType.SPELL_TRACE_DEX_100:
            case UpgradeType.SPELL_TRACE_INT_100:
            case UpgradeType.SPELL_TRACE_LUK_100:
            case UpgradeType.SPELL_TRACE_HP_100:
            case UpgradeType.SPELL_TRACE_ALL_100:
                if (equipType.IsWeapon() || equipType == MapleEquipType.EquipType.HEART)
                {
                    msc[atkType] += level.GetRangeValue(1, 2, 3);
                    if (equipType != MapleEquipType.EquipType.HEART)
                        msc[statType] += level.GetRangeValue(0, 0, isHP ? 50 : 1);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[atkType] += level.GetRangeValue(0, 1, 1);
                }
                else if (equipType.IsArmor())
                {
                    msc[statType] += level.GetRangeValue(isHP ? 55 : 1, isHP ? 120 : 2, isHP ? 180 : 3);
                    if (!isHP) msc[MapleStatus.StatusType.HP] += level.GetRangeValue(5, 20, 30);
                }
                else if (equipType.IsAccessory())
                {
                    msc[statType] += level.GetRangeValue(isHP ? 50 : 1, isHP ? 50 : 1, isHP ? 100 : 2);
                }

                break;
            case UpgradeType.SPELL_TRACE_STR_70:
            case UpgradeType.SPELL_TRACE_DEX_70:
            case UpgradeType.SPELL_TRACE_INT_70:
            case UpgradeType.SPELL_TRACE_LUK_70:
            case UpgradeType.SPELL_TRACE_HP_70:
            case UpgradeType.SPELL_TRACE_ALL_70:
                if (equipType.IsWeapon() || equipType == MapleEquipType.EquipType.HEART)
                {
                    msc[atkType] += level.GetRangeValue(2, 3, 5);
                    if (equipType != MapleEquipType.EquipType.HEART)
                        msc[statType] += level.GetRangeValue(0, isHP ? 50 : 1, isHP ? 100 : 2);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[atkType] += level.GetRangeValue(1, 2, 2);
                }
                else if (equipType.IsArmor())
                {
                    msc[statType] += level.GetRangeValue(isHP ? 115 : 2, isHP ? 190 : 3, isHP ? 270 : 4);
                    if (!isHP) msc[MapleStatus.StatusType.HP] += level.GetRangeValue(15, 40, 70);
                }
                else if (equipType.IsAccessory())
                {
                    msc[statType] += level.GetRangeValue(isHP ? 100 : 2, isHP ? 100 : 2, isHP ? 150 : 3);
                }

                break;
            case UpgradeType.SPELL_TRACE_STR_30:
            case UpgradeType.SPELL_TRACE_DEX_30:
            case UpgradeType.SPELL_TRACE_INT_30:
            case UpgradeType.SPELL_TRACE_LUK_30:
            case UpgradeType.SPELL_TRACE_HP_30:
            case UpgradeType.SPELL_TRACE_ALL_30:
                if (equipType.IsWeapon() || equipType == MapleEquipType.EquipType.HEART)
                {
                    msc[atkType] += level.GetRangeValue(3, 5, 7);
                    if (equipType != MapleEquipType.EquipType.HEART)
                        msc[statType] += level.GetRangeValue(isHP ? 50 : 1, isHP ? 100 : 2, isHP ? 150 : 3);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[atkType] += level.GetRangeValue(2, 3, 3);
                }
                else if (equipType.IsArmor())
                {
                    msc[statType] += level.GetRangeValue(
                        isHP ? 180 : isAll ? 1 : 3,
                        isHP ? 320 : isAll ? 2 : 5,
                        isHP ? 470 : isAll ? 3 : 7);
                    if (!isHP) msc[MapleStatus.StatusType.HP] += level.GetRangeValue(30, 70, 120);
                }
                else if (equipType.IsAccessory())
                {
                    msc[statType] += level.GetRangeValue(
                        isHP ? 150 : isAll ? 1 : 3,
                        isHP ? 200 : isAll ? 1 : 4,
                        isHP ? 250 : isAll ? 1 : 5);
                }

                break;
            case UpgradeType.SPELL_TRACE_STR_15:
            case UpgradeType.SPELL_TRACE_DEX_15:
            case UpgradeType.SPELL_TRACE_INT_15:
            case UpgradeType.SPELL_TRACE_LUK_15:
            case UpgradeType.SPELL_TRACE_HP_15:
            case UpgradeType.SPELL_TRACE_ALL_15:
                if (equipType.IsWeapon())
                {
                    msc[atkType] += level.GetRangeValue(5, 7, 9);
                    msc[statType] += level.GetRangeValue(isHP ? 100 : 2, isHP ? 150 : 3, isHP ? 200 : 4);
                }
                else if (equipType.IsArmor() && equipType == MapleEquipType.EquipType.GLOVE)
                {
                    msc[atkType] += level.GetRangeValue(3, 4, 4);
                }
                else if (equipType.IsArmor())
                {
                    msc[statType] += level.GetRangeValue(
                        isHP ? 245 : isAll ? 2 : 4,
                        isHP ? 460 : isAll ? 3 : 7,
                        isHP ? 670 : isAll ? 4 : 10);
                    if (!isHP) msc[MapleStatus.StatusType.HP] += level.GetRangeValue(45, 110, 170);
                }
                else if (equipType.IsAccessory())
                {
                    msc[statType] += level.GetRangeValue(
                        isHP ? 150 : isAll ? 1 : 3,
                        isHP ? 200 : isAll ? 1 : 4,
                        isHP ? 250 : isAll ? 1 : 5);
                }

                break;
            case UpgradeType.DOMINATOR_FRAGMENT:
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                msc[MapleStatus.StatusType.HP] += 40;
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
                break;
            case UpgradeType.EARRING_INT_10:
                msc[MapleStatus.StatusType.INT] += 3;
                msc[MapleStatus.StatusType.MAGIC_POWER] += 5;
                break;
            case UpgradeType.CHAOS:
                msc[MapleStatus.StatusType.ALL_STAT] += 6;
                msc[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 6;
                msc[MapleStatus.StatusType.HP] += 60;
                break;
            case UpgradeType.PREMIUM_ACCESSORY_ATK_4:
            case UpgradeType.PREMIUM_ACCESSORY_ATK_5:
            case UpgradeType.PREMIUM_ACCESSORY_MAG_4:
            case UpgradeType.PREMIUM_ACCESSORY_MAG_5:
            case UpgradeType.ACCESSORY_ATK_2:
            case UpgradeType.ACCESSORY_ATK_3:
            case UpgradeType.ACCESSORY_ATK_4:
            case UpgradeType.ACCESSORY_MAG_2:
            case UpgradeType.ACCESSORY_MAG_3:
            case UpgradeType.ACCESSORY_MAG_4:
            case UpgradeType.ACCESSORY_70_ATK_1:
            case UpgradeType.ACCESSORY_70_ATK_2:
            case UpgradeType.ACCESSORY_70_MAG_1:
            case UpgradeType.ACCESSORY_70_MAG_2:
            case UpgradeType.MIRACLE_50_ATK_2:
            case UpgradeType.MIRACLE_50_ATK_3:
            case UpgradeType.MIRACLE_50_MAG_2:
            case UpgradeType.MIRACLE_50_MAG_3:
            case UpgradeType.ARMOR_70_ATK_1:
            case UpgradeType.ARMOR_70_ATK_2:
            case UpgradeType.ARMOR_70_MAG_1:
            case UpgradeType.ARMOR_70_MAG_2:
            case UpgradeType.PREMIUM_PET_ATK_4:
            case UpgradeType.PREMIUM_PET_ATK_5:
            case UpgradeType.PREMIUM_PET_MAG_4:
            case UpgradeType.PREMIUM_PET_MAG_5:
            case UpgradeType.PET_ATK_2:
            case UpgradeType.PET_ATK_3:
            case UpgradeType.PET_ATK_4:
            case UpgradeType.PET_MAG_2:
            case UpgradeType.PET_MAG_3:
            case UpgradeType.PET_MAG_4:
            case UpgradeType.PET_70_ATK_1:
            case UpgradeType.PET_70_ATK_2:
            case UpgradeType.PET_70_MAG_2:
                int value1 = type.ToString()[^1] - '0';
                msc[atkType] += value1;
                break;
            case UpgradeType.MAGICAL_ATK_9:
            case UpgradeType.MAGICAL_ATK_10:
            case UpgradeType.MAGICAL_ATK_11:
            case UpgradeType.MAGICAL_MAG_9:
            case UpgradeType.MAGICAL_MAG_10:
            case UpgradeType.MAGICAL_MAG_11:
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                int value2 = type.ToString()[^1] - '0';
                msc[atkType] += value2;
                break;
            case UpgradeType.HEART_ATK:
            case UpgradeType.HEART_MAG:
                msc[MapleStatus.StatusType.ALL_STAT] += 3;
                msc[atkType] += 9;
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

    private static bool IsSpellTrace(this UpgradeType type)
    {
        return 0x01 <= (int) type && (int) type <= 0x18;
    }

    private static UpgradeType[] TestPresets(int itemLevel, int upgCnt, MapleEquipType.EquipType equipType,
        MapleStatContainer target, List<UpgradeType> candidates)
    {
        MapleStatContainer self = new MapleStatContainer();
        
        // Single Upgrade
        foreach (UpgradeType single in candidates)
        {
            for(int upg = 0 ; upg < upgCnt ; upg++) self.Simulate(itemLevel, equipType, single);
            if(single.IsSpellTrace()) self[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
            self.Flush();
            if (self.Compare(target)) return MakeUpgradeTypes(upgCnt, single);
            self.Clear();
        }
        
        // Chaos 1 + Single
        foreach (UpgradeType single in candidates)
        {
            self.Simulate(itemLevel, equipType, UpgradeType.CHAOS);
            for(int upg = 1 ; upg < upgCnt ; upg++) self.Simulate(itemLevel, equipType, single);
            if(single.IsSpellTrace()) self[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 1;
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
            if (type.IsSpellTrace() && ++traceCnt == 4)
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
            UpgradeType.SPELL_TRACE_STR_100,
            UpgradeType.SPELL_TRACE_DEX_100,
            UpgradeType.SPELL_TRACE_INT_100,
            UpgradeType.SPELL_TRACE_LUK_100,
            UpgradeType.SPELL_TRACE_HP_100,
            UpgradeType.SPELL_TRACE_ALL_100,
            UpgradeType.SPELL_TRACE_STR_70,
            UpgradeType.SPELL_TRACE_DEX_70,
            UpgradeType.SPELL_TRACE_INT_70,
            UpgradeType.SPELL_TRACE_LUK_70,
            UpgradeType.SPELL_TRACE_HP_70,
            UpgradeType.SPELL_TRACE_ALL_70,
            UpgradeType.SPELL_TRACE_STR_30,
            UpgradeType.SPELL_TRACE_DEX_30,
            UpgradeType.SPELL_TRACE_INT_30,
            UpgradeType.SPELL_TRACE_LUK_30,
            UpgradeType.SPELL_TRACE_HP_30,
            UpgradeType.SPELL_TRACE_ALL_30,
        };

        if (item.EquipType.IsWeapon() || item.EquipType.IsArmor())
        {
            candidateTypes.Add(UpgradeType.SPELL_TRACE_STR_15);
            candidateTypes.Add(UpgradeType.SPELL_TRACE_DEX_15);
            candidateTypes.Add(UpgradeType.SPELL_TRACE_INT_15);
            candidateTypes.Add(UpgradeType.SPELL_TRACE_LUK_15);
            candidateTypes.Add(UpgradeType.SPELL_TRACE_HP_15);
            candidateTypes.Add(UpgradeType.SPELL_TRACE_ALL_15);

            if (item.EquipType is not MapleEquipType.EquipType.WEAPON and MapleEquipType.EquipType.SUB_WEAPON)
            {
                candidateTypes.Add(UpgradeType.ARMOR_70_ATK_2);
                candidateTypes.Add(UpgradeType.ARMOR_70_MAG_2);
                candidateTypes.Add(UpgradeType.MIRACLE_50_ATK_3);
                candidateTypes.Add(UpgradeType.MIRACLE_50_MAG_3);
            }
        }

        if (item.EquipType.IsAccessory())
        {
            candidateTypes.Add(UpgradeType.ACCESSORY_70_ATK_2);
            candidateTypes.Add(UpgradeType.ACCESSORY_70_MAG_2);
            candidateTypes.Add(UpgradeType.ACCESSORY_ATK_4);
            candidateTypes.Add(UpgradeType.ACCESSORY_MAG_4);
            candidateTypes.Add(UpgradeType.PREMIUM_ACCESSORY_ATK_5);
            candidateTypes.Add(UpgradeType.PREMIUM_ACCESSORY_MAG_5);
        }

        if (item.EquipType is MapleEquipType.EquipType.EARRING)
        {
            candidateTypes.Add(UpgradeType.EARRING_INT_10);
        }

        if (item.EquipType.IsWeapon() || item.EquipType is MapleEquipType.EquipType.HEART)
        {
            candidateTypes.Add(UpgradeType.MAGICAL_ATK_11);
            candidateTypes.Add(UpgradeType.MAGICAL_MAG_11);
        }

        if (item.EquipType is MapleEquipType.EquipType.HEART)
        {
            candidateTypes.Add(UpgradeType.HEART_ATK);
            candidateTypes.Add(UpgradeType.HEART_MAG);
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

    public static MapleStatContainer ConvertUpgrades(this UpgradeType[]? upgrades, CommonItem item, Dictionary<MapleStatus.StatusType, double>? chaos)
    {
        MapleStatContainer msc = new MapleStatContainer();
        if (upgrades == null || chaos == null) return msc;

        int chaosCnt = 0;
        int spelltraces = 0;
        foreach (UpgradeType type in upgrades)
        {
            if (type == UpgradeType.CHAOS)
            {
                chaosCnt++;
                continue;
            }

            if (type.IsSpellTrace()) spelltraces++;
            msc.Simulate(item.ItemLevel, item.EquipType, type);
        }

        if (spelltraces >= 4 && item.EquipType.IsArmor() && item.EquipType is not MapleEquipType.EquipType.GLOVE)
        {
            msc[MapleStatus.StatusType.ATTACK_POWER] += 1;
            msc[MapleStatus.StatusType.MAGIC_POWER] += 1;
        }

        foreach (var pair in chaos)
        {
            if (!double.IsFinite(pair.Value)) continue;
            msc[pair.Key] += (int) Math.Round(pair.Value * chaosCnt);
        }

        msc.Flush();
        return msc;
    }
}