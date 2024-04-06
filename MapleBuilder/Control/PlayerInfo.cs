using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control;

public class PlayerInfo
{
    public class StatInfo
    {
        public MaplePotentialOptionType StatType { get; }
        public MaplePotentialOptionType StatRateType { get; }
        public int BaseValue { get; protected internal set; } // 기본 수치
        public int RateValue { get; protected internal set; } // % 수치
        public int FlatValue { get; protected internal set; } // % 미적용 수치

        protected internal StatInfo(MaplePotentialOptionType statType)
        {
            StatType = statType;
            StatRateType = statType switch
            {
                MaplePotentialOptionType.STR => MaplePotentialOptionType.STR_RATE,
                MaplePotentialOptionType.DEX => MaplePotentialOptionType.DEX_RATE,
                MaplePotentialOptionType.INT => MaplePotentialOptionType.INT_RATE,
                MaplePotentialOptionType.LUK => MaplePotentialOptionType.LUK_RATE,
                MaplePotentialOptionType.MAX_HP => MaplePotentialOptionType.MAX_HP_RATE,
                _ => MaplePotentialOptionType.OTHER
            };
            BaseValue = 4;
            RateValue = 0;
            FlatValue = 0;
        }
    }
    
    public StatInfo MainStat { get; private set; }
    public StatInfo SubStat { get; private set; }
    public StatInfo SubStat2 { get; private set; }
    public MaplePotentialOptionType AttackType { get; private set; }
    public MaplePotentialOptionType AttackRateType { get; private set; }
    public int AttackValue { get; private set; }
    public int AttackRate { get; private set; }
    public int Damage { get; private set; }
    public int BossDamage { get; private set; }
    public int CommonDamage { get; private set; }
    public int DebuffDamage { get; private set; }
    public int ItemDropIncrease { get; private set; }
    public int MesoDropIncrease { get; private set; }
    public int CooldownDecreaseValue { get; private set; }
    public int CooldownDecreaseRate { get; private set; }
    public int CooldownIgnoreRate { get; private set; }
    public int BuffDurationIncrease { get; private set; }
    public int SummonDurationIncrease { get; private set; }
    public int Immune { get; private set; }
    public double FinalDamage { get; private set; }
    public double IgnoreArmor { get; private set; }
    public double CriticalChance { get; private set; }
    public double CriticalDamage { get; private set; }
    public double IgnoreImmune { get; private set; }

    public PlayerInfo(MaplePotentialOptionType mainStat, MaplePotentialOptionType subStat, MaplePotentialOptionType subStat2 = MaplePotentialOptionType.OTHER)
    {
        MainStat = new StatInfo(mainStat);
        SubStat = new StatInfo(subStat);
        SubStat2 = new StatInfo(subStat2);

        AttackType = MainStat.StatType == MaplePotentialOptionType.INT
            ? MaplePotentialOptionType.MAGIC
            : MaplePotentialOptionType.ATTACK;
        AttackRateType = MainStat.StatType == MaplePotentialOptionType.INT
            ? MaplePotentialOptionType.MAGIC_RATE
            : MaplePotentialOptionType.ATTACK_RATE;
        AttackValue = 0;
        AttackRate = 0;

        Damage = 0;
        BossDamage = 0;
        CommonDamage = 0;
        DebuffDamage = 0;
        ItemDropIncrease = 0;
        MesoDropIncrease = 0;
        CooldownDecreaseValue = 0;
        CooldownDecreaseRate = 0;
        CooldownIgnoreRate = 0;
        BuffDurationIncrease = 0;
        SummonDurationIncrease = 0;
        Immune = 0;
        FinalDamage = 0;
        IgnoreArmor = 0;
        CriticalChance = 0;
        CriticalDamage = 0;
        IgnoreImmune = 0;
    }

    private MapleOption GetOption(MapleItem item)
    {
        MapleOption option = new MapleOption();
        if (item.BaseOption != null) option += item.BaseOption;
        if (item.AddOption != null) option += item.AddOption;
        if (item.EtcOption != null) option += item.EtcOption;
        if (item.ExceptionalOption != null) option += item.ExceptionalOption;
        return option;
    }

    private int GetStatFromOption(MapleOption option, MaplePotentialOptionType statType)
    {
        return statType switch
        {
            MaplePotentialOptionType.STR => option.Str,
            MaplePotentialOptionType.DEX => option.Dex,
            MaplePotentialOptionType.INT => option.Int,
            MaplePotentialOptionType.LUK => option.Luk,
            MaplePotentialOptionType.MAX_HP => option.MaxHp,
            _ => 0
        };
    }

    private double CalcIgnoreArmor(double baseValue, double additional, bool isAdd)
    {
        double cvtBase = (100 - baseValue) / 100.0;
        double cvtAdd = (100 - additional) / 100.0;
        return isAdd ? (1 - cvtBase * cvtAdd) * 100 : (1 - cvtBase / cvtAdd) * 100;
    }

    private void ApplyPotential(KeyValuePair<MaplePotentialOptionType, int>[] potential, bool isAdd = true)
    {
        int sign = isAdd ? 1 : -1;

        foreach (var pair in potential)
        {
            int value = pair.Value * sign;
            if      (pair.Key == MainStat.StatType)     MainStat.BaseValue += value;
            else if (pair.Key == MainStat.StatRateType) MainStat.RateValue += value;
            else if (pair.Key == SubStat.StatType)      SubStat.BaseValue += value;
            else if (pair.Key == SubStat.StatRateType)  SubStat.RateValue += value;
            else if (pair.Key == SubStat2.StatType)     SubStat2.BaseValue += value;
            else if (pair.Key == SubStat2.StatRateType) SubStat2.RateValue += value;
            else if (pair.Key == AttackType)            AttackValue += value;
            else if (pair.Key == AttackRateType)        AttackRate += value;
            else switch (pair.Key)
            {
                case MaplePotentialOptionType.ALL_STAT:
                    MainStat.BaseValue += value;
                    SubStat.BaseValue += value;
                    SubStat2.BaseValue += value;
                    break;
                case MaplePotentialOptionType.ALL_STAT_RATE:
                    MainStat.RateValue += value;
                    SubStat.RateValue += value;
                    SubStat2.RateValue += value;
                    break;
                case MaplePotentialOptionType.DAMAGE:
                    Damage += value;
                    break;
                case MaplePotentialOptionType.BOSS_DAMAGE:
                    BossDamage += value;
                    break;
                case MaplePotentialOptionType.CRITICAL_DAMAGE:
                    CriticalDamage += value;
                    break;
                case MaplePotentialOptionType.IGNORE_ARMOR:
                    IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, value, isAdd);
                    break;
                case MaplePotentialOptionType.ITEM_DROP:
                    ItemDropIncrease += value;
                    break;
                case MaplePotentialOptionType.MESO_DROP:
                    MesoDropIncrease += value;
                    break;
                case MaplePotentialOptionType.COOLDOWN_DECREASE:
                    CooldownDecreaseValue += value;
                    break;
            }
            
        }
        
    }

    public void AddItem(MapleItem item)
    {
        MapleOption itemOption = GetOption(item);
        MainStat.BaseValue += GetStatFromOption(itemOption, MainStat.StatType);
        SubStat.BaseValue += GetStatFromOption(itemOption, SubStat.StatType);
        SubStat2.BaseValue += GetStatFromOption(itemOption, SubStat2.StatType);

        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials);
            ApplyPotential(item.Potential.Additionals);
        }
        ApplyPotential(item.Specials.ToArray());

    }

    public void SubtractItem(MapleItem item)
    {
        MapleOption itemOption = GetOption(item);
        MainStat.BaseValue -= GetStatFromOption(itemOption, MainStat.StatType);
        SubStat.BaseValue -= GetStatFromOption(itemOption, SubStat.StatType);
        SubStat2.BaseValue -= GetStatFromOption(itemOption, SubStat2.StatType);
        
        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials, false);
            ApplyPotential(item.Potential.Additionals, false);
        }
    }
    
    
    
    
}