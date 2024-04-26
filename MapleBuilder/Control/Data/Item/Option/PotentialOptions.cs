using System;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item.Option;

public class PotentialOptions
{
    public class RangeValue
    {
        public Dictionary<int, int[]> Values;
        internal RangeValue()
        {
            Values = new Dictionary<int, int[]>();
        }

        internal RangeValue Push(int threshold, params int[] values)
        {
            Values.Add(threshold, values);
            return this;
        }
    }
    
    public class Details
    {
        public string Description;
        public Dictionary<MaplePotentialGrade.GradeType, RangeValue> Values;
        public Dictionary<MaplePotentialGrade.GradeType, RangeValue> AdditionalValues;
        public MapleEquipType.EquipType[] AvailableTypes;
        public MapleStatus.StatusType StatusType;

        private static readonly List<MapleEquipType.EquipType> WEAPONS = new()
        {
            MapleEquipType.EquipType.WEAPON, MapleEquipType.EquipType.SUB_WEAPON, MapleEquipType.EquipType.EMBLEM
        };

        private static readonly List<MapleEquipType.EquipType> ARMOR_ACCESSORY = new()
        {
            MapleEquipType.EquipType.HELMET, MapleEquipType.EquipType.TOP, MapleEquipType.EquipType.BOTTOM,
            MapleEquipType.EquipType.TOP_BOTTOM, MapleEquipType.EquipType.GLOVE, MapleEquipType.EquipType.BOOT,
            MapleEquipType.EquipType.CAPE, MapleEquipType.EquipType.SHOULDER, MapleEquipType.EquipType.RING, 
            MapleEquipType.EquipType.PENDANT, MapleEquipType.EquipType.BELT, MapleEquipType.EquipType.EYE,
            MapleEquipType.EquipType.FACE, MapleEquipType.EquipType.EARRING, MapleEquipType.EquipType.HEART
        };
        
        internal Details(string desc, MapleStatus.StatusType statusType)
        {
            Description = desc;
            Values = new Dictionary<MaplePotentialGrade.GradeType, RangeValue>();
            AdditionalValues = new Dictionary<MaplePotentialGrade.GradeType, RangeValue>();
            AvailableTypes = Array.Empty<MapleEquipType.EquipType>();
            StatusType = statusType;

        }

        internal Details Potential(MaplePotentialGrade.GradeType grade, RangeValue options)
        {
            Values.Add(grade, options);
            return this;
        }

        internal Details Additional(MaplePotentialGrade.GradeType grade, RangeValue options)
        {
            AdditionalValues.Add(grade, options);
            return this;
        }

        internal Details Available(params MapleEquipType.EquipType[] types)
        {
            AvailableTypes = types;
            return this;
        }

        internal Details AvailableWeapon(params MapleEquipType.EquipType[] expects)
        {
            List<MapleEquipType.EquipType> clone = new List<MapleEquipType.EquipType>(WEAPONS);
            foreach (var expect in expects) clone.Remove(expect);
            AvailableTypes = clone.ToArray();
            return this;
        }
        
        internal Details AvailableArmorAccessory(params MapleEquipType.EquipType[] expects)
        {
            List<MapleEquipType.EquipType> clone = new List<MapleEquipType.EquipType>(ARMOR_ACCESSORY);
            foreach (var expect in expects) clone.Remove(expect);
            AvailableTypes = clone.ToArray();
            return this;
        }

        internal Details AvailableAll()
        {
            List<MapleEquipType.EquipType> clone = new List<MapleEquipType.EquipType>(ARMOR_ACCESSORY);
            foreach(var type in WEAPONS) clone.Add(type);
            AvailableTypes = clone.ToArray();
            return this;
        }
    }

    // https://namu.wiki/w/잠재능력/옵션%20목록
    public static readonly Details[] Options =
    {
        ///////////////////////////////////
        //
        // 무기류 옵션
        //
        ///////////////////////////////////
        new Details("몬스터 방어율 무시 : +%d%", MapleStatus.StatusType.IGNORE_DEF)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(50, 15))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(50, 15))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(50, 30))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(50, 35).Push(100, 40))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(50, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(50, 3))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(50, 4))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(50, 5))
            .AvailableWeapon(),
        new Details("공격력 : +%d%", MapleStatus.StatusType.ATTACK_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .AvailableWeapon(),
        new Details("마력 : +%d%", MapleStatus.StatusType.MAGIC_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .AvailableWeapon(),
        new Details("보스 몬스터 공격 시 데미지 : +%d%", MapleStatus.StatusType.BOSS_DAMAGE)
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(100, 30))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(100, 35, 40))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(50, 12))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(50, 18))
            .AvailableWeapon(MapleEquipType.EquipType.EMBLEM),
        new Details("데미지 : +%d%", MapleStatus.StatusType.DAMAGE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .AvailableWeapon(),
        new Details("크리티컬 확률 : +%d%", MapleStatus.StatusType.CRITICAL_CHANCE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 8))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .AvailableWeapon(),
        new Details("공격력 : +%d", MapleStatus.StatusType.ATTACK_POWER)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(61, 4).Push(81, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(61, 8).Push(81, 10).Push(91, 12))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 32))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(61, 4).Push(81, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(61, 8).Push(81, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 32))
            .AvailableWeapon(),
        new Details("마력 : +%d", MapleStatus.StatusType.MAGIC_POWER)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(61, 4).Push(81, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(61, 8).Push(81, 10).Push(91, 12))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 32))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(61, 4).Push(81, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(61, 8).Push(81, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 32))
            .AvailableWeapon(),
        new Details("STR : +%d", MapleStatus.StatusType.STR)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .AvailableWeapon(),
        new Details("DEX : +%d", MapleStatus.StatusType.DEX)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .AvailableWeapon(),
        new Details("INT : +%d", MapleStatus.StatusType.INT)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .AvailableWeapon(),
        new Details("LUK : +%d", MapleStatus.StatusType.LUK)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .AvailableWeapon(),
        new Details("STR : +%d%", MapleStatus.StatusType.STR_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .AvailableWeapon(),
        new Details("DEX : +%d%", MapleStatus.StatusType.DEX_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .AvailableWeapon(),
        new Details("INT : +%d%", MapleStatus.StatusType.INT_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .AvailableWeapon(),
        new Details("LUK : +%d%", MapleStatus.StatusType.LUK_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .AvailableWeapon(),
        new Details("올스탯 : +%d", MapleStatus.StatusType.ALL_STAT)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(61, 4).Push(81, 5))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(20, 2).Push(41, 3).Push(61, 4).Push(81, 5))
            .AvailableWeapon(),
        new Details("올스탯 : +%d%", MapleStatus.StatusType.ALL_STAT_RATE)
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .AvailableWeapon(),
        new Details("최대 HP : +%d", MapleStatus.StatusType.HP)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 5).Push(11, 10).Push(21, 15)
                .Push(31, 20).Push(41, 25).Push(51, 30).Push(61, 35).Push(71, 40).Push(81, 45).Push(91, 50).Push(101, 55).Push(111, 60))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 10).Push(11, 20).Push(21, 30)
                .Push(31, 40).Push(41, 50).Push(51, 60).Push(61, 80).Push(71, 80).Push(81, 90).Push(91, 100).Push(101, 110).Push(111, 120))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 5).Push(11, 10).Push(21, 15)
                .Push(31, 20).Push(41, 25).Push(51, 30).Push(61, 35).Push(71, 40).Push(81, 45).Push(91, 50).Push(101, 55).Push(111, 60))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 10).Push(21, 15).Push(51, 50).Push(91, 100))
            .AvailableWeapon(),
        new Details("최대 HP : +%d%", MapleStatus.StatusType.HP_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(250, 13))
            .AvailableWeapon(),
        
        ///////////////////////////////////
        //
        // 공통 옵션
        //
        ///////////////////////////////////
        
        new Details("공격력 : +%d", MapleStatus.StatusType.ATTACK_POWER)
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(51, 2).Push(101, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 4).Push(61, 6).Push(81, 8).Push(91, 10).Push(250, 11))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 4).Push(21, 6).Push(51, 8).Push(91, 11).Push(250, 12))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 6).Push(21, 8).Push(51, 10).Push(91, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 8).Push(21, 10).Push(51, 12).Push(91, 14).Push(250, 15))
            .AvailableArmorAccessory(),
        new Details("마력 : +%d", MapleStatus.StatusType.MAGIC_POWER)
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(51, 2).Push(101, 3).Push(250, 4))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 4).Push(61, 6).Push(81, 8).Push(91, 10).Push(250, 11))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 4).Push(21, 6).Push(51, 8).Push(91, 11).Push(250, 12))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 6).Push(21, 8).Push(51, 10).Push(91, 12).Push(250, 13))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 8).Push(21, 10).Push(51, 12).Push(91, 14).Push(250, 15))
            .AvailableArmorAccessory(),
        new Details("STR : +%d", MapleStatus.StatusType.STR)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(51, 6).Push(71, 8).Push(91, 10))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 4).Push(21, 6).Push(41, 8).Push(51, 10).Push(71, 12).Push(91, 14))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 6).Push(21, 8).Push(41, 10).Push(51, 12).Push(71, 14).Push(91, 16))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 8).Push(21, 10).Push(41, 12).Push(51, 14).Push(71, 16).Push(91, 18))
            .AvailableArmorAccessory(MapleEquipType.EquipType.GLOVE),
        new Details("DEX : +%d", MapleStatus.StatusType.DEX)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(51, 6).Push(71, 8).Push(91, 10))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 4).Push(21, 6).Push(41, 8).Push(51, 10).Push(71, 12).Push(91, 14))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 6).Push(21, 8).Push(41, 10).Push(51, 12).Push(71, 14).Push(91, 16))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 8).Push(21, 10).Push(41, 12).Push(51, 14).Push(71, 16).Push(91, 18))
            .AvailableArmorAccessory(MapleEquipType.EquipType.GLOVE),
        new Details("INT : +%d", MapleStatus.StatusType.INT)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(51, 6).Push(71, 8).Push(91, 10))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 4).Push(21, 6).Push(41, 8).Push(51, 10).Push(71, 12).Push(91, 14))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 6).Push(21, 8).Push(41, 10).Push(51, 12).Push(71, 14).Push(91, 16))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 8).Push(21, 10).Push(41, 12).Push(51, 14).Push(71, 16).Push(91, 18))
            .AvailableArmorAccessory(MapleEquipType.EquipType.GLOVE),
        new Details("LUK : +%d", MapleStatus.StatusType.LUK)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(41, 6).Push(51, 8).Push(71, 10).Push(91, 12))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(51, 4).Push(71, 5).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 2).Push(21, 4).Push(51, 6).Push(71, 8).Push(91, 10))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 4).Push(21, 6).Push(41, 8).Push(51, 10).Push(71, 12).Push(91, 14))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 6).Push(21, 8).Push(41, 10).Push(51, 12).Push(71, 14).Push(91, 16))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 8).Push(21, 10).Push(41, 12).Push(51, 14).Push(71, 16).Push(91, 18))
            .AvailableArmorAccessory(MapleEquipType.EquipType.GLOVE),
        new Details("STR : +%d%", MapleStatus.StatusType.STR_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(91, 2).Push(250, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(21, 2).Push(51, 3).Push(91, 4))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(21, 3).Push(51, 4).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(21, 4).Push(51, 5).Push(91, 8))
            .AvailableArmorAccessory(),
        new Details("DEX : +%d%", MapleStatus.StatusType.DEX_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(91, 2).Push(250, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(21, 2).Push(51, 3).Push(91, 4))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(21, 3).Push(51, 4).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(21, 4).Push(51, 5).Push(91, 8))
            .AvailableArmorAccessory(),
        new Details("INT : +%d%", MapleStatus.StatusType.INT_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(91, 2).Push(250, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(21, 2).Push(51, 3).Push(91, 4))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(21, 3).Push(51, 4).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(21, 4).Push(51, 5).Push(91, 8))
            .AvailableArmorAccessory(),
        new Details("LUK : +%d%", MapleStatus.StatusType.LUK_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(91, 2).Push(250, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(21, 2).Push(51, 3).Push(91, 4))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(21, 3).Push(51, 4).Push(91, 6))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(21, 4).Push(51, 5).Push(91, 8))
            .AvailableArmorAccessory(),
        new Details("올스탯 : +%d", MapleStatus.StatusType.ALL_STAT)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(21, 2).Push(41, 3).Push(61, 4).Push(81, 5))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(51, 2).Push(91, 3))
            .AvailableArmorAccessory(),
        new Details("올스탯 : +%d%", MapleStatus.StatusType.ALL_STAT_RATE)
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(250, 3))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 5).Push(250, 6))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(250, 7))
            .AvailableArmorAccessory(),
        new Details("최대 HP : +%d", MapleStatus.StatusType.HP)
            .Potential(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 5).Push(11, 10).Push(21, 15)
                .Push(31, 20).Push(41, 25).Push(51, 30).Push(61, 35).Push(71, 40).Push(81, 45).Push(91, 50).Push(101, 55).Push(111, 60))
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 10).Push(11, 20).Push(21, 30)
                .Push(31, 40).Push(41, 50).Push(51, 60).Push(61, 80).Push(71, 80).Push(81, 90).Push(91, 100).Push(101, 110).Push(111, 120))
            .Additional(MaplePotentialGrade.GradeType.NONE, new RangeValue().Push(0, 5).Push(11, 10).Push(21, 15).Push(31, 20).Push(41, 25)
                .Push(51, 30).Push(61, 35).Push(71, 40).Push(81, 45).Push(91, 50).Push(101, 55).Push(111, 60))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 10).Push(21, 15).Push(51, 50).Push(91, 100))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 15).Push(11, 30).Push(21, 45).Push(31, 60).Push(41, 75)
                .Push(51, 90).Push(61, 105).Push(71, 120).Push(81, 135).Push(91, 150).Push(101, 165).Push(111, 180))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 20).Push(11, 40).Push(21, 60).Push(31, 80).Push(41, 100)
                .Push(51, 120).Push(61, 140).Push(71, 160).Push(81, 180).Push(91, 200).Push(101, 220).Push(111, 240))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 25).Push(11, 50).Push(21, 75).Push(31, 100).Push(41, 125)
                .Push(51, 150).Push(61, 175).Push(71, 200).Push(81, 225).Push(91, 250).Push(101, 275).Push(111, 300))
            .AvailableArmorAccessory(),
        new Details("최대 HP : +%d%", MapleStatus.StatusType.HP_RATE)
            .Potential(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(31, 2).Push(71, 3).Push(250, 4))
            .Potential(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 2).Push(31, 4).Push(71, 6).Push(250, 7))
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 3).Push(31, 6).Push(71, 9).Push(250, 10))
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 6).Push(31, 9).Push(71, 12).Push(13))
            .Additional(MaplePotentialGrade.GradeType.RARE, new RangeValue().Push(0, 1).Push(91, 2).Push(250, 3))
            .Additional(MaplePotentialGrade.GradeType.EPIC, new RangeValue().Push(0, 1).Push(21, 2).Push(51, 3).Push(91, 5).Push(250, 6))
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 2).Push(21, 3).Push(51, 5).Push(91, 7).Push(250, 9))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 3).Push(21, 5).Push(51, 7).Push(91, 10).Push(250, 12))
            .AvailableArmorAccessory(),
        
        ///////////////////////////////////
        //
        // 특수 옵션
        //
        ///////////////////////////////////
        
        new Details("재사용 대기시간 : -%d초", MapleStatus.StatusType.COOL_DEC_SECOND)
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(120, 2))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(100, 1))
            .Available(MapleEquipType.EquipType.HELMET),
        new Details("재사용 대기시간 : -%d초", MapleStatus.StatusType.COOL_DEC_SECOND)
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(70, 1))
            .Available(MapleEquipType.EquipType.HELMET),
        new Details("STR : +%d", MapleStatus.StatusType.STR)
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(30, 32))
            .Available(MapleEquipType.EquipType.GLOVE),
        new Details("DEX : +%d", MapleStatus.StatusType.DEX)
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(30, 32))
            .Available(MapleEquipType.EquipType.GLOVE),
        new Details("INT : +%d", MapleStatus.StatusType.INT)
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(30, 32))
            .Available(MapleEquipType.EquipType.GLOVE),
        new Details("LUK : +%d", MapleStatus.StatusType.LUK)
            .Potential(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(30, 32))
            .Available(MapleEquipType.EquipType.GLOVE),
        new Details("9레벨 당 STR : +%d", MapleStatus.StatusType.STR_PER_LEVEL)
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 1))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 2))
            .AvailableAll(),
        new Details("9레벨 당 DEX : +%d", MapleStatus.StatusType.DEX_PER_LEVEL)
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 1))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 2))
            .AvailableAll(),
        new Details("9레벨 당 INT : +%d", MapleStatus.StatusType.INT_PER_LEVEL)
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 1))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 2))
            .AvailableAll(),
        new Details("9레벨 당 LUK : +%d", MapleStatus.StatusType.LUK_PER_LEVEL)
            .Additional(MaplePotentialGrade.GradeType.UNIQUE, new RangeValue().Push(0, 1))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 2))
            .AvailableAll(),
        new Details("크리티컬 데미지 : +%d%", MapleStatus.StatusType.CRITICAL_DAMAGE)
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(50, 5).Push(70, 6).Push(90, 8))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 1).Push(21, 2).Push(91, 3))
            .Available(MapleEquipType.EquipType.GLOVE),
        new Details("크리티컬 데미지 : +%d%", MapleStatus.StatusType.CRITICAL_DAMAGE)
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 1))
            .AvailableArmorAccessory(),
        new Details("아이템 드롭율 : +%d%", MapleStatus.StatusType.ITEM_DROP)
            .Potential(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 10).Push(31, 15).Push(71, 20))
            .Additional(MaplePotentialGrade.GradeType.LEGENDARY, new RangeValue().Push(0, 2).Push(30, 3).Push(70, 4).Push(100, 5))
            .Available(MapleEquipType.EquipType.EYE, MapleEquipType.EquipType.FACE, MapleEquipType.EquipType.RING, MapleEquipType.EquipType.PENDANT, MapleEquipType.EquipType.EARRING)
    };
}