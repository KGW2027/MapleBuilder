using System;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item.Option;

internal static class StarforceOption
{
    private static int GetStatIncreaseByStarforce(this CommonItem item, int starforce)
    {
        if (starforce <= 5) return 2;
        if (starforce <= 15) return 3;
        if (starforce >= 23) return 0;
        if (item.ItemLevel >= 248) return 17;
        if (item.ItemLevel >= 198) return 15;
        if (item.ItemLevel >= 158) return 13;
        if (item.ItemLevel >= 148) return 11;
        if (item.ItemLevel >= 138) return 9;
        if (item.ItemLevel >= 128) return 7;
        return 0;
    }
    
    private static int GetWeaponAttackIncreaseByStarforce(this CommonItem item, int starforce, int increase)
    {
        bool isMage = item.EquipData![MapleStatus.StatusType.ATTACK_POWER] < item.EquipData![MapleStatus.StatusType.MAGIC_POWER];
        if(starforce <= 15)
        {
            double refValue = isMage
                ? item.EquipData![MapleStatus.StatusType.MAGIC_POWER]
                : item.EquipData![MapleStatus.StatusType.ATTACK_POWER];
            refValue += increase;
            return (int) Math.Floor(refValue / 50.0) + 1;
        }
    
        if (item.ItemLevel >= 198)
        {
            return starforce switch
            {
                16 => 13,
                17 => 13,
                18 => 14,
                19 => 14,
                20 => 15,
                21 => 16,
                22 => 17,
                23 => 34,
                24 => 35,
                25 => 36,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 158)
        {
            return starforce switch
            {
                16 => 9,
                17 => 9,
                18 => 10,
                19 => 11,
                20 => 12,
                21 => 13,
                22 => 14,
                23 => 32,
                24 => 33,
                25 => 34,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 148)
        {
            return starforce switch
            {
                16 => 8,
                17 => 9,
                18 => 9,
                19 => 10,
                20 => 11,
                21 => 12,
                22 => 13,
                23 => 31,
                24 => 32,
                25 => 33,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 138)
        {
            return starforce switch
            {
                16 => 7,
                17 => 8,
                18 => 8,
                19 => 9,
                20 => 10,
                21 => 11,
                22 => 12,
                23 => 30,
                24 => 31,
                25 => 32,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 128)
        {
            return starforce switch
            {
                16 => 6,
                17 => 7,
                18 => 7,
                19 => 8,
                20 => 9,
                _ => 0
            };
        }
        
        return 0;
    }
    
    private static int GetArmorAttackIncreaseByStarforce(this CommonItem item, int starforce)
    {
        if (starforce <= 15)
        {
            if (item.EquipType != MapleEquipType.EquipType.GLOVE) return 0;
            return starforce is 5 or 7 or 9 or 11 or 13 or 14 or 15 ? 1 : 0;
        }
    
        if (item.ItemLevel >= 248)
        {
            return starforce switch
            {
                16 => 14,
                17 => 15,
                18 => 16,
                19 => 17,
                20 => 18,
                21 => 19,
                22 => 21,
                23 => 23,
                24 => 25,
                25 => 27,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 198)
        {
            return starforce switch
            {
                16 => 12,
                17 => 13,
                18 => 14,
                19 => 15,
                20 => 16,
                21 => 17,
                22 => 19,
                23 => 21,
                24 => 23,
                25 => 25,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 158)
        {
            return starforce switch
            {
                16 => 10,
                17 => 11,
                18 => 12,
                19 => 13,
                20 => 14,
                21 => 15,
                22 => 17,
                23 => 19,
                24 => 21,
                25 => 23,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 148)
        {
            return starforce switch
            {
                16 => 9,
                17 => 10,
                18 => 11,
                19 => 12,
                20 => 13,
                21 => 14,
                22 => 16,
                23 => 18,
                24 => 20,
                25 => 22,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 138)
        {
            return starforce switch
            {
                16 => 8,
                17 => 9,
                18 => 10,
                19 => 11,
                20 => 12,
                21 => 13,
                22 => 15,
                23 => 17,
                24 => 19,
                25 => 21,
                _ => 0
            };
        }
        
        if (item.ItemLevel >= 128)
        {
            return starforce switch
            {
                16 => 7,
                17 => 8,
                18 => 9,
                19 => 10,
                20 => 11,
                _ => 0
            };
        }
        return 0;
    }
    
    public static MapleStatContainer ParseStarforceOption(this CommonItem item, double upgAttack, double upgMagic)
    {
        MapleStatContainer option = new MapleStatContainer();
        if (item.Starforce is null or 0) return option;
    
        for (int idx = 1; idx <= item.Starforce; idx++)
        {
            option[MapleStatus.StatusType.ALL_STAT] += item.GetStatIncreaseByStarforce(idx);

            if (item.EquipType == MapleEquipType.EquipType.WEAPON)
            {
                int atk = item.GetWeaponAttackIncreaseByStarforce(idx,
                    (int) (option[MapleStatus.StatusType.ATTACK_POWER] + upgAttack));
                int mag = item.GetWeaponAttackIncreaseByStarforce(idx,
                    (int) (option[MapleStatus.StatusType.MAGIC_POWER] + upgMagic));

                option[MapleStatus.StatusType.ATTACK_POWER] += atk;
                option[MapleStatus.StatusType.MAGIC_POWER] += mag;
            }
            else option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += item.GetArmorAttackIncreaseByStarforce(idx);
        }
        option.Flush();
        return option;
    }
}