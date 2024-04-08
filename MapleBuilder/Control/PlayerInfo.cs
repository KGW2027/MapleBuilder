using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.MapleData;
using MapleBuilder.View.SubFrames;
using MaplePotentialOption = MapleAPI.Enum.MaplePotentialOption;

namespace MapleBuilder.Control;

public class PlayerInfo
{
    public class StatInfo
    {
        public MaplePotentialOption.OptionType Stat { get; }
        public MaplePotentialOption.OptionType StatRate { get; }
        public MaplePotentialOption.OptionType StatLevel { get; }
        public int BaseValue { get; protected internal set; } // 기본 수치
        public int RateValue { get; protected internal set; } // % 수치
        public int FlatValue { get; protected internal set; } // % 미적용 수치

        protected internal StatInfo(MaplePotentialOption.OptionType stat)
        {
            Stat = stat;
            StatRate = stat switch
            {
                MaplePotentialOption.OptionType.STR => MaplePotentialOption.OptionType.STR_RATE,
                MaplePotentialOption.OptionType.DEX => MaplePotentialOption.OptionType.DEX_RATE,
                MaplePotentialOption.OptionType.INT => MaplePotentialOption.OptionType.INT_RATE,
                MaplePotentialOption.OptionType.LUK => MaplePotentialOption.OptionType.LUK_RATE,
                MaplePotentialOption.OptionType.MAX_HP => MaplePotentialOption.OptionType.MAX_HP_RATE,
                _ => MaplePotentialOption.OptionType.OTHER
            };
            StatLevel = stat switch
            {
                MaplePotentialOption.OptionType.STR => MaplePotentialOption.OptionType.LEVEL_STR,
                MaplePotentialOption.OptionType.DEX => MaplePotentialOption.OptionType.LEVEL_DEX,
                MaplePotentialOption.OptionType.INT => MaplePotentialOption.OptionType.LEVEL_INT,
                MaplePotentialOption.OptionType.LUK => MaplePotentialOption.OptionType.LEVEL_LUK,
                _ => MaplePotentialOption.OptionType.OTHER
            };
            BaseValue = 4;
            RateValue = 0;
            FlatValue = 0;
        }
    }
    
    public PlayerInfo(uint level, MaplePotentialOption.OptionType mainStat, 
        MaplePotentialOption.OptionType subStat, MaplePotentialOption.OptionType subStat2 = MaplePotentialOption.OptionType.OTHER)
    {
        MainStat = new StatInfo(mainStat)
        {
            BaseValue = (int) (mainStat == MaplePotentialOption.OptionType.MAX_HP ? 90 * level + 545 : 5 * level + 18)
        };
        SubStat = new StatInfo(subStat);
        SubStat2 = new StatInfo(subStat2);

        AttackType = MainStat.Stat == MaplePotentialOption.OptionType.INT
            ? MaplePotentialOption.OptionType.MAGIC
            : MaplePotentialOption.OptionType.ATTACK;
        AttackRateType = MainStat.Stat == MaplePotentialOption.OptionType.INT
            ? MaplePotentialOption.OptionType.MAGIC_RATE
            : MaplePotentialOption.OptionType.ATTACK_RATE;
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
        CriticalChance = 5;
        CriticalDamage = 0;
        IgnoreImmune = 0;

        LevelStat = (int) Math.Floor(level / 9.0);
        SetEffects = new SetEffect();
        LastSymbols = new Dictionary<MapleSymbol.SymbolType, int>();
        LastAbilities = new Dictionary<MapleAbility.AbilityType, int>();
    }

    
    public StatInfo MainStat { get; private set; }
    public StatInfo SubStat { get; private set; }
    public StatInfo SubStat2 { get; private set; }
    public MaplePotentialOption.OptionType AttackType { get; private set; }
    public MaplePotentialOption.OptionType AttackRateType { get; private set; }
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
    public int LevelStat { get; private set; }
    public double FinalDamage { get; private set; }
    public double IgnoreArmor { get; private set; }
    public double CriticalChance { get; private set; }
    public double CriticalDamage { get; private set; }
    public double IgnoreImmune { get; private set; }
    
    public SetEffect SetEffects { get; private set; }
    public Dictionary<MapleSymbol.SymbolType, int> LastSymbols;
    public Dictionary<MapleAbility.AbilityType, int> LastAbilities;
    
    ///<summary>
    ///    방어력 무시의 곱연산을 계산합니다.
    ///</summary>
    private double CalcIgnoreArmor(double baseValue, double additional, bool isAdd)
    {
        double cvtBase = (100 - Math.Abs(baseValue)) / 100.0;
        double cvtAdd = (100 -  Math.Abs(additional)) / 100.0;
        return isAdd ? (1 - cvtBase * cvtAdd) * 100 : (1 - cvtBase / cvtAdd) * 100;
    }
    
    #region 장비 아이템 효과 적용
    ///<summary>
    ///    장비 아이템의 기본 스텟, 작, 추옵, 익셉셔널 정보를 가져옵니다.
    ///</summary>
    private MapleOption GetOption(MapleItem item)
    {
        MapleOption option = new MapleOption();
        if (item.BaseOption != null) option += item.BaseOption;
        if (item.AddOption != null) option += item.AddOption;
        if (item.EtcOption != null) option += item.EtcOption;
        if (item.ExceptionalOption != null) option += item.ExceptionalOption;
        if (item.StarforceOption != null) option += item.StarforceOption;
        return option;
    }
    
    ///<summary>
    ///    장비 정보에서 원하는 스텟에 해당하는 값을 추출합니다.
    ///</summary>
    private int GetStatFromOption(MapleOption option, MaplePotentialOption.OptionType stat)
    {
        return stat switch
        {
            MaplePotentialOption.OptionType.STR => option.Str,
            MaplePotentialOption.OptionType.DEX => option.Dex,
            MaplePotentialOption.OptionType.INT => option.Int,
            MaplePotentialOption.OptionType.LUK => option.Luk,
            MaplePotentialOption.OptionType.MAX_HP => option.MaxHp,
            _ => 0
        };
    }
    
    ///<summary>
    ///    잠재능력, 에디셔널 잠재능력을 현재 스텟에 적용합니다.
    ///</summary>
    private void ApplyPotential(KeyValuePair<MaplePotentialOption.OptionType, int>[] potential, bool isAdd = true)
    {
        int sign = isAdd ? 1 : -1;

        foreach (var pair in potential)
        {
            int value = pair.Value * sign;
            if      (pair.Key == MainStat.Stat)      MainStat.BaseValue += value;
            else if (pair.Key == MainStat.StatRate)  MainStat.RateValue += value;
            else if (pair.Key == MainStat.StatLevel) MainStat.BaseValue += LevelStat * value;
            else if (pair.Key == SubStat.Stat)       SubStat.BaseValue += value;
            else if (pair.Key == SubStat.StatRate)   SubStat.RateValue += value;
            else if (pair.Key == SubStat.StatLevel)  SubStat.BaseValue += LevelStat * value;
            else if (pair.Key == SubStat2.Stat)      SubStat2.BaseValue += value;
            else if (pair.Key == SubStat2.StatRate)  SubStat2.RateValue += value;
            else if (pair.Key == SubStat2.StatLevel) SubStat2.BaseValue += LevelStat * value;
            else if (pair.Key == AttackType)             AttackValue += value;
            else if (pair.Key == AttackRateType)         AttackRate += value;
            else switch (pair.Key)
            {
                case MaplePotentialOption.OptionType.ALL_STAT:
                    MainStat.BaseValue += value;
                    SubStat.BaseValue += value;
                    SubStat2.BaseValue += value;
                    break;
                case MaplePotentialOption.OptionType.ALL_STAT_RATE:
                    MainStat.RateValue += value;
                    SubStat.RateValue += value;
                    SubStat2.RateValue += value;
                    break;
                case MaplePotentialOption.OptionType.DAMAGE:
                    Damage += value;
                    break;
                case MaplePotentialOption.OptionType.BOSS_DAMAGE:
                    BossDamage += value;
                    break;
                case MaplePotentialOption.OptionType.CRITICAL_DAMAGE:
                    CriticalDamage += value;
                    break;
                case MaplePotentialOption.OptionType.IGNORE_ARMOR:
                    IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, value, isAdd);
                    break;
                case MaplePotentialOption.OptionType.ITEM_DROP:
                    ItemDropIncrease += value;
                    break;
                case MaplePotentialOption.OptionType.MESO_DROP:
                    MesoDropIncrease += value;
                    break;
                case MaplePotentialOption.OptionType.COOLDOWN_DECREASE:
                    CooldownDecreaseValue += value;
                    break;
            }
        }
    }
    
    ///<summary>
    ///    MapleOption 객체의 정보를 현재 스텟에 적용하는 것으로, 세트 아이템 효과 적용에 사용합니다.
    ///</summary>
    private void ApplyMapleOption(MapleOption option)
    {
        MainStat.BaseValue += GetStatFromOption(option, MainStat.Stat);
        SubStat.BaseValue += GetStatFromOption(option, SubStat.Stat);
        SubStat2.BaseValue += GetStatFromOption(option, SubStat2.Stat);
        AttackValue += AttackType == MaplePotentialOption.OptionType.ATTACK ? option.AttackPower : option.MagicPower;
        BossDamage += option.BossDamage;
        Damage += option.Damage;
        CommonDamage += option.CommonDamage;
        CriticalDamage += option.CriticalDamage;
        IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, option.IgnoreArmor, option.IgnoreArmor >= 0);
        
        if (MainStat.Stat == MaplePotentialOption.OptionType.MAX_HP)
            MainStat.RateValue += option.MaxHpRate;
    }
    
    ///<summary>
    ///    아이템 착용과 해제를 현재 스텟에 시뮬레이션합니다.
    ///</summary>
    private void ApplyAddSub(MapleItem item, bool isAdd)
    {
        int sign = isAdd ? 1 : -1;
        
        // 아이템 기본 효과 적용
        MapleOption itemOption = GetOption(item);
        MainStat.BaseValue += GetStatFromOption(itemOption, MainStat.Stat) * sign;
        SubStat.BaseValue += GetStatFromOption(itemOption, SubStat.Stat) * sign;
        SubStat2.BaseValue += GetStatFromOption(itemOption, SubStat2.Stat) * sign;
        MainStat.RateValue += itemOption.AllStatRate * sign;
        SubStat.RateValue += itemOption.AllStatRate * sign;
        SubStat2.RateValue += itemOption.AllStatRate * sign;

        BossDamage += itemOption.BossDamage;
        IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, itemOption.IgnoreArmor, isAdd);
        
        // 잠재능력 적용
        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials, isAdd);
            ApplyPotential(item.Potential.Additionals, isAdd);
        }
        
        // 칭호 효과 적용
        ApplyPotential(item.Specials.ToArray(), isAdd);
        
        // 세트 효과 적용
        MapleOption setEffectPrev = SetEffects.GetSetOptions();
        if(isAdd)
            SetEffects.Add(item);
        else
            SetEffects.Sub(item);
        ApplyMapleOption(SetEffects.GetSetOptions() - setEffectPrev);
    }
    
    ///<summary>
    ///    아이템을 장착합니다.
    ///</summary>
    public void AddItem(MapleItem item)
    {
        ApplyAddSub(item, true);
    }
    
    ///<summary>
    ///    아이템을 해제합니다.
    ///</summary>
    public void SubtractItem(MapleItem item)
    {
        ApplyAddSub(item, false);
    }
    #endregion
    
    #region 심볼 효과 적용

    private int GetSymbolStats(Dictionary<MapleSymbol.SymbolType, int> symbolData)
    {
        int stat = 0;
        int baseValue = BuilderDataContainer.CharacterInfo!.Class switch
            {
              MapleClass.ClassType.XENON => 48,
              MapleClass.ClassType.DEMON_AVENGER => 2100,
              _ => 100
            };
        foreach (var pair in symbolData)
        {
            switch (pair.Key)
            {
                case MapleSymbol.SymbolType.YEORO:
                case MapleSymbol.SymbolType.CHUCHU:
                case MapleSymbol.SymbolType.LACHELEIN:
                case MapleSymbol.SymbolType.ARCANA:
                case MapleSymbol.SymbolType.MORAS:
                case MapleSymbol.SymbolType.ESFERA:
                    stat += baseValue * (pair.Value + 2);
                    break;
                case MapleSymbol.SymbolType.CERNIUM:
                case MapleSymbol.SymbolType.ARCS:
                case MapleSymbol.SymbolType.ODIUM:
                case MapleSymbol.SymbolType.DOWONKYUNG:
                case MapleSymbol.SymbolType.ARTERIA:
                case MapleSymbol.SymbolType.CARCION:
                    stat += baseValue * (2 * pair.Value + 3);
                    break;
                case MapleSymbol.SymbolType.UNKNOWN:
                default:
                    break;
            }
        }
        return stat;
    }
    
    public void ApplySymbolData(Dictionary<MapleSymbol.SymbolType, int> symbolData)
    {
        int delta = GetSymbolStats(symbolData) - GetSymbolStats(LastSymbols);
        MainStat.FlatValue += delta;
        if (BuilderDataContainer.CharacterInfo!.Class == MapleClass.ClassType.XENON)
        {
            SubStat.FlatValue += delta;
            SubStat2.FlatValue += delta;
        }

        LastSymbols = symbolData;
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 어빌리티 효과 적용

    private MaplePotentialOption.OptionType GetOptionTypeByAbilityType(MapleAbility.AbilityType type)
    {
        return type switch
        {
            MapleAbility.AbilityType.STR => MaplePotentialOption.OptionType.STR,
            MapleAbility.AbilityType.DEX => MaplePotentialOption.OptionType.DEX,
            MapleAbility.AbilityType.INT => MaplePotentialOption.OptionType.INT,
            MapleAbility.AbilityType.LUK => MaplePotentialOption.OptionType.LUK,
            MapleAbility.AbilityType.HP => MaplePotentialOption.OptionType.MAX_HP,
            MapleAbility.AbilityType.MAX_HP_RATE => MaplePotentialOption.OptionType.MAX_HP_RATE,
            MapleAbility.AbilityType.ATTACK_POWER => MaplePotentialOption.OptionType.ATTACK,
            MapleAbility.AbilityType.LEVEL_ATTACK_POWER => MaplePotentialOption.OptionType.ATTACK,
            MapleAbility.AbilityType.MAGIC_POWER => MaplePotentialOption.OptionType.MAGIC,
            MapleAbility.AbilityType.LEVEL_MAGIC_POWER => MaplePotentialOption.OptionType.MAGIC,
            _ => MaplePotentialOption.OptionType.OTHER
        };
    }
    
    private void ApplyAbilityEach(MapleAbility.AbilityType type, int value)
    {
        MaplePotentialOption.OptionType optType = GetOptionTypeByAbilityType(type);
        if (optType == MainStat.Stat) MainStat.FlatValue += value;
        else if (optType == MainStat.StatRate) MainStat.RateValue += value;
        else if (optType == SubStat.Stat) SubStat.FlatValue += value;
        else if (optType == SubStat2.Stat) SubStat2.FlatValue += value;
        else if (optType == AttackType)
        {
            if (type is MapleAbility.AbilityType.LEVEL_MAGIC_POWER or MapleAbility.AbilityType.LEVEL_ATTACK_POWER)
                value = (int) (BuilderDataContainer.CharacterInfo!.Level / value);
            AttackValue += value;
        }
        
        switch (type)
        {
            case MapleAbility.AbilityType.CRITICAL_CHANCE:
                CriticalChance += value;
                break;
            case MapleAbility.AbilityType.ALL_STAT:
                MainStat.FlatValue += value;
                SubStat.FlatValue += value;
                SubStat2.FlatValue += value;
                break;
            case MapleAbility.AbilityType.BOSS_DAMAGE:
                BossDamage += value;
                break;
            case MapleAbility.AbilityType.COMMON_DAMAGE:
                CommonDamage += value;
                break;
            case MapleAbility.AbilityType.DEBUFF_DAMAGE:
                DebuffDamage += value;
                break;
            case MapleAbility.AbilityType.COOLDOWN_IGNORE:
                CooldownIgnoreRate += value;
                break;
            case MapleAbility.AbilityType.BUFF_DURATION_INCREASE:
                BuffDurationIncrease += value;
                break;
            case MapleAbility.AbilityType.ITEM_DROP:
                ItemDropIncrease += value;
                break;
            case MapleAbility.AbilityType.MESO_DROP:
                MesoDropIncrease += value;
                break;
            case MapleAbility.AbilityType.ATTACK_SPEED:
            case MapleAbility.AbilityType.PASSIVE_SKILL_LEVEL:
            case MapleAbility.AbilityType.MULTI_TARGET:
            case MapleAbility.AbilityType.OTHER:
            default:
                break;
        }
    }

    public void ApplyAbility(Dictionary<MapleAbility.AbilityType, int> abilities)
    {
        foreach (var pair in LastAbilities)
            ApplyAbilityEach(pair.Key, pair.Value * -1);
        foreach (var pair in abilities)
            ApplyAbilityEach(pair.Key, pair.Value);
        LastAbilities = abilities;
        
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 하이퍼 스탯 효과 적용
    
    private MaplePotentialOption.OptionType GetOptionTypeByHyperStatType(MapleHyperStat.StatType type)
    {
        return type switch
        {
            MapleHyperStat.StatType.STR => MaplePotentialOption.OptionType.STR,
            MapleHyperStat.StatType.DEX => MaplePotentialOption.OptionType.DEX,
            MapleHyperStat.StatType.INT => MaplePotentialOption.OptionType.INT,
            MapleHyperStat.StatType.LUK => MaplePotentialOption.OptionType.LUK,
            MapleHyperStat.StatType.HP => MaplePotentialOption.OptionType.MAX_HP_RATE,
            _ => MaplePotentialOption.OptionType.OTHER
        };
    }
    public void ApplyHyperStat(MapleHyperStat.StatType statType, double delta)
    {
        MaplePotentialOption.OptionType optType = GetOptionTypeByHyperStatType(statType);
        if (optType == MainStat.Stat) MainStat.FlatValue += (int) delta;
        else if (optType == MainStat.StatRate) MainStat.RateValue += (int) delta;
        else if (optType == SubStat.Stat) SubStat.FlatValue += (int) delta;
        else if (optType == SubStat2.Stat) SubStat2.FlatValue += (int) delta;
        
        switch (statType)
        {
            case MapleHyperStat.StatType.CRITCIAL_CHANCE:
                CriticalChance += delta;
                break;
            case MapleHyperStat.StatType.CRITICAL_DAMAGE:
                CriticalDamage += delta;
                break;
            case MapleHyperStat.StatType.IGNORE_ARMOR:
                IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, delta, delta >= 0);
                break;
            case MapleHyperStat.StatType.DAMAGE:
                Damage += (int) delta;
                break;
            case MapleHyperStat.StatType.BOSS_DAMAGE:
                BossDamage += (int) delta;
                break;
            case MapleHyperStat.StatType.COMMON_DAMAGE:
                CommonDamage += (int) delta;
                break;
            case MapleHyperStat.StatType.IMMUNE:
                Immune += (int) delta;
                break;
            case MapleHyperStat.StatType.ATTACK_POWER:
                AttackValue += (int) delta;
                break;
            case MapleHyperStat.StatType.MP:
            case MapleHyperStat.StatType.DF_TT_PP:
            case MapleHyperStat.StatType.EXP_UP:
            case MapleHyperStat.StatType.ARCANE_FORCE:
                break;
        }
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 유니온 공격대 효과 적용

    private MaplePotentialOption.OptionType GetOptionTypeByRaiderEffectType(MapleUnion.RaiderEffectType effectType)
    {
        return effectType switch
        {
            MapleUnion.RaiderEffectType.INT => MaplePotentialOption.OptionType.INT,
            MapleUnion.RaiderEffectType.STR => MaplePotentialOption.OptionType.STR,
            MapleUnion.RaiderEffectType.LUK => MaplePotentialOption.OptionType.LUK,
            MapleUnion.RaiderEffectType.DEX => MaplePotentialOption.OptionType.DEX,
            MapleUnion.RaiderEffectType.MAX_HP => MaplePotentialOption.OptionType.MAX_HP,
            MapleUnion.RaiderEffectType.MAX_HP_RATE => MaplePotentialOption.OptionType.MAX_HP_RATE,
            _ => MaplePotentialOption.OptionType.OTHER
        };
    }
    
    public void ApplyUnionRaider(MapleUnion.RaiderEffectType effectType, int delta)
    {
        MaplePotentialOption.OptionType statChecker = GetOptionTypeByRaiderEffectType(effectType);
        if (statChecker == MainStat.Stat) MainStat.FlatValue += delta;
        else if (statChecker == MainStat.StatRate) MainStat.RateValue += delta;
        else if (statChecker == SubStat.Stat) SubStat.FlatValue += delta;
        else if (statChecker == SubStat2.Stat) SubStat2.FlatValue += delta;
        
        if(statChecker == MaplePotentialOption.OptionType.OTHER)
        {
            switch (effectType)
            {
                case MapleUnion.RaiderEffectType.BOSS_DAMAGE:
                    BossDamage += delta;
                    break;
                case MapleUnion.RaiderEffectType.IGNORE_ARMOR:
                    IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, delta, delta >= 0);
                    break;
                case MapleUnion.RaiderEffectType.IMMUNE:
                    Immune += delta;
                    break;
                case MapleUnion.RaiderEffectType.STR_DEX_LUK:
                    MaplePotentialOption.OptionType[] options =
                    {
                        MaplePotentialOption.OptionType.STR, MaplePotentialOption.OptionType.DEX,
                        MaplePotentialOption.OptionType.LUK
                    };
                    if (options.Contains(MainStat.Stat)) MainStat.FlatValue += delta;
                    if (options.Contains(SubStat.Stat)) SubStat.FlatValue += delta;
                    if (options.Contains(SubStat2.Stat)) SubStat2.FlatValue += delta;
                    break;
                case MapleUnion.RaiderEffectType.ATTACK_MAGIC_POWER:
                    AttackValue += delta;
                    break;
                case MapleUnion.RaiderEffectType.MESO_DROP:
                    MesoDropIncrease += delta;
                    break;
                case MapleUnion.RaiderEffectType.CRIT_CHANCE:
                    CriticalChance += delta;
                    break;
                case MapleUnion.RaiderEffectType.BUFF_DURATION:
                    BuffDurationIncrease += delta;
                    break;
                case MapleUnion.RaiderEffectType.SUMMON_DURATION:
                    SummonDurationIncrease += delta;
                    break;
                case MapleUnion.RaiderEffectType.CRIT_DAMAGE:
                    CriticalDamage += delta;
                    break;
                case MapleUnion.RaiderEffectType.COOLDOWN_DECREASE_RATE:
                    CooldownDecreaseRate += delta;
                    break;
                case MapleUnion.RaiderEffectType.OTHER:
                case MapleUnion.RaiderEffectType.INT:
                case MapleUnion.RaiderEffectType.STR:
                case MapleUnion.RaiderEffectType.LUK:
                case MapleUnion.RaiderEffectType.DEX:
                case MapleUnion.RaiderEffectType.MAX_HP:
                case MapleUnion.RaiderEffectType.MAX_HP_RATE:
                case MapleUnion.RaiderEffectType.MAX_MP_RATE:
                case MapleUnion.RaiderEffectType.EXP_UP:
                case MapleUnion.RaiderEffectType.CHANCE_DAMAGE:
                default:
                    break;
            }
        }
        Summarize.DispatchSummary();
    }
    
    #endregion
    
    // TODO : 스텟 적용 테스트 - 기본수치 4422, 실제수치 4477, 차이 125 - 아르카나세트 올스텟 15
}