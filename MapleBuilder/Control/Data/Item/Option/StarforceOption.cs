// using System;
// using MapleAPI.DataType;
// using MapleAPI.Enum;
//
// namespace MapleBuilder.Control.Data.Item.Option;
//
// internal static class StarforceOption
// {
//     public static int GetStatIncreaseByStarforce(this ItemBase item, int starforce, int itemLevel)
//     {
//         if (starforce <= 5) return 2;
//         if (starforce <= 15) return 3;
//         if (starforce >= 23) return 0;
//         if (itemLevel >= 248) return 17;
//         if (itemLevel >= 198) return 15;
//         if (itemLevel >= 158) return 13;
//         if (itemLevel >= 148) return 11;
//         if (itemLevel >= 138) return 9;
//         if (itemLevel >= 128) return 7;
//         return 0;
//     }
//     
//     private static int GetWeaponAttackIncreaseByStarforce(int starforce, int increase)
//     {
//         bool isMage = BaseOption[MapleStatus.StatusType.ATTACK_POWER] < BaseOption[MapleStatus.StatusType.MAGIC_POWER];
//         if(starforce <= 15)
//         {
//             double refValue = isMage
//                 ? BaseOption[MapleStatus.StatusType.MAGIC_POWER] + EtcOption[MapleStatus.StatusType.MAGIC_POWER]
//                 : BaseOption[MapleStatus.StatusType.ATTACK_POWER] + EtcOption[MapleStatus.StatusType.MAGIC_POWER];
//             refValue += increase;
//             return (int) Math.Floor(refValue / 50.0) + 1;
//         }
//     
//         if (ItemLevel >= 198)
//         {
//             return starforce switch
//             {
//                 16 => 13,
//                 17 => 13,
//                 18 => 14,
//                 19 => 14,
//                 20 => 15,
//                 21 => 16,
//                 22 => 17,
//                 23 => 34,
//                 24 => 35,
//                 25 => 36,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 158)
//         {
//             return starforce switch
//             {
//                 16 => 9,
//                 17 => 9,
//                 18 => 10,
//                 19 => 11,
//                 20 => 12,
//                 21 => 13,
//                 22 => 14,
//                 23 => 32,
//                 24 => 33,
//                 25 => 34,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 148)
//         {
//             return starforce switch
//             {
//                 16 => 8,
//                 17 => 9,
//                 18 => 9,
//                 19 => 10,
//                 20 => 11,
//                 21 => 12,
//                 22 => 13,
//                 23 => 31,
//                 24 => 32,
//                 25 => 33,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 138)
//         {
//             return starforce switch
//             {
//                 16 => 7,
//                 17 => 8,
//                 18 => 8,
//                 19 => 9,
//                 20 => 10,
//                 21 => 11,
//                 22 => 12,
//                 23 => 30,
//                 24 => 31,
//                 25 => 32,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 128)
//         {
//             return starforce switch
//             {
//                 16 => 6,
//                 17 => 7,
//                 18 => 7,
//                 19 => 8,
//                 20 => 9,
//                 _ => 0
//             };
//         }
//         
//         return 0;
//     }
//     
//     private int GetArmorAttackIncreaseByStarforce(int starforce)
//     {
//         if (starforce <= 15)
//         {
//             if (MapleEquipType.EquipType != MapleEquipType.EquipType.GLOVE) return 0;
//             return starforce is 5 or 7 or 9 or 11 or 13 or 14 or 15 ? 1 : 0;
//         }
//     
//         if (ItemLevel >= 248)
//         {
//             return starforce switch
//             {
//                 16 => 14,
//                 17 => 15,
//                 18 => 16,
//                 19 => 17,
//                 20 => 18,
//                 21 => 19,
//                 22 => 21,
//                 23 => 23,
//                 24 => 25,
//                 25 => 27,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 198)
//         {
//             return starforce switch
//             {
//                 16 => 12,
//                 17 => 13,
//                 18 => 14,
//                 19 => 15,
//                 20 => 16,
//                 21 => 17,
//                 22 => 19,
//                 23 => 21,
//                 24 => 23,
//                 25 => 25,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 158)
//         {
//             return starforce switch
//             {
//                 16 => 10,
//                 17 => 11,
//                 18 => 12,
//                 19 => 13,
//                 20 => 14,
//                 21 => 15,
//                 22 => 17,
//                 23 => 19,
//                 24 => 21,
//                 25 => 23,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 148)
//         {
//             return starforce switch
//             {
//                 16 => 9,
//                 17 => 10,
//                 18 => 11,
//                 19 => 12,
//                 20 => 13,
//                 21 => 14,
//                 22 => 16,
//                 23 => 18,
//                 24 => 20,
//                 25 => 22,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 138)
//         {
//             return starforce switch
//             {
//                 16 => 8,
//                 17 => 9,
//                 18 => 10,
//                 19 => 11,
//                 20 => 12,
//                 21 => 13,
//                 22 => 15,
//                 23 => 17,
//                 24 => 19,
//                 25 => 21,
//                 _ => 0
//             };
//         }
//         
//         if (ItemLevel >= 128)
//         {
//             return starforce switch
//             {
//                 16 => 7,
//                 17 => 8,
//                 18 => 9,
//                 19 => 10,
//                 20 => 11,
//                 _ => 0
//             };
//         }
//         return 0;
//     }
//     
//     public MapleStatContainer ParseStarforceOption()
//     {
//         if (CachedStarforce.TryGetValue(sfv, out var opt)) return opt;
//         MapleStatContainer option = new MapleStatContainer();
//         if (sfv == 0) return option;
//     
//         for (int idx = 1; idx <= sfv; idx++)
//         {
//             option[MapleStatus.StatusType.ALL_STAT] += GetStatIncreaseByStarforce(idx);
//             int apmp = MapleEquipType.EquipType == MapleEquipType.EquipType.WEAPON
//                 ? GetWeaponAttackIncreaseByStarforce(idx, (int) option[MapleStatus.StatusType.ATTACK_AND_MAGIC])
//                 : GetArmorAttackIncreaseByStarforce(idx);
//             option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += apmp;
//         }
//         option.Flush();
//         return option;
//     }
//     #endregion
// }