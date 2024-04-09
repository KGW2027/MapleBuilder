using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.MapleData;
using MapleBuilder.View.SubFrames;
using MaplePotentialOption = MapleAPI.Enum.MaplePotentialOption;

namespace MapleBuilder.Control;

public class PlayerInfo
{
    public class StatInfo
    {
        public MapleStatus.StatusType Stat { get; }
        public MapleStatus.StatusType StatRate { get; }
        public MapleStatus.StatusType StatLevel { get; }
        public int BaseValue { get; protected internal set; } // 기본 수치
        public int RateValue { get; protected internal set; } // % 수치
        public int FlatValue { get; protected internal set; } // % 미적용 수치

        protected internal StatInfo(MapleStatus.StatusType stat)
        {
            Stat = stat;
            StatRate = Stat + 0x10;
            StatLevel = stat == MapleStatus.StatusType.HP ? MapleStatus.StatusType.OTHER : Stat + 0x20;
            BaseValue = 4;
            RateValue = 0;
            FlatValue = 0;
        }
    }
    
    public PlayerInfo(uint level, MapleStatus.StatusType mainStat, 
        MapleStatus.StatusType subStat, MapleStatus.StatusType subStat2 = 0)
    {
        MainStat = new StatInfo(mainStat)
        {
            BaseValue = (int) (mainStat == MapleStatus.StatusType.HP ? 90 * level + 545 : 5 * level + 18) // 레벨 자동 분배 수치 적용
        };
        SubStat = new StatInfo(subStat);
        SubStat2 = new StatInfo(subStat2);

        AttackType = mainStat == MapleStatus.StatusType.INT
            ? MapleStatus.StatusType.MAGIC_POWER
            : MapleStatus.StatusType.ATTACK_POWER;
        AttackRateType = mainStat == MapleStatus.StatusType.INT
            ? MapleStatus.StatusType.MAGIC_RATE
            : MapleStatus.StatusType.ATTACK_RATE;
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
        LastAbilities = new Dictionary< MapleStatus.StatusType, int>();
    }

    
    public StatInfo MainStat { get; private set; }
    public StatInfo SubStat { get; private set; }
    public StatInfo SubStat2 { get; private set; }
    public MapleStatus.StatusType AttackType { get; private set; }
    public MapleStatus.StatusType AttackRateType { get; private set; }
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
    
    public MapleStatContainer PlayerStat { get; private set; }
    public SetEffect SetEffects { get; private set; }
    public Dictionary<MapleSymbol.SymbolType, int> LastSymbols;
    public Dictionary< MapleStatus.StatusType, int> LastAbilities;
    
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
    // private MapleOption GetOption(MapleItem item)
    // {
    //     MapleOption option = new MapleOption();
    //     if (item.BaseOption != null) option += item.BaseOption;
    //     if (item.AddOption != null) option += item.AddOption;
    //     if (item.EtcOption != null) option += item.EtcOption;
    //     if (item.ExceptionalOption != null) option += item.ExceptionalOption;
    //     if (item.StarforceOption != null) option += item.StarforceOption;
    //     return option;
    // }
    
    ///<summary>
    ///    장비 정보에서 원하는 스텟에 해당하는 값을 추출합니다.
    ///</summary>
    // private int GetStatFromOption(MapleOption option,  MapleStatus.StatusType stat)
    // {
    //     return stat switch
    //     {
    //         MapleStatus.StatusType.STR => option.Str,
    //         MapleStatus.StatusType.DEX => option.Dex,
    //         MapleStatus.StatusType.INT => option.Int,
    //         MapleStatus.StatusType.LUK => option.Luk,
    //         MapleStatus.StatusType.HP => option.MaxHp,
    //         _ => 0
    //     };
    // }
    
    ///<summary>
    ///    잠재능력, 에디셔널 잠재능력을 현재 스텟에 적용합니다.
    ///</summary>
    private void ApplyPotential(KeyValuePair<MapleStatus.StatusType, int>[] potential, bool isAdd = true)
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
                case MapleStatus.StatusType.ALL_STAT:
                    MainStat.BaseValue += value;
                    SubStat.BaseValue += value;
                    SubStat2.BaseValue += value;
                    break;
                case  MapleStatus.StatusType.ALL_STAT_RATE:
                    MainStat.RateValue += value;
                    SubStat.RateValue += value;
                    SubStat2.RateValue += value;
                    break;
                case MapleStatus.StatusType.DAMAGE:
                    Damage += value;
                    break;
                case MapleStatus.StatusType.BOSS_DAMAGE:
                    BossDamage += value;
                    break;
                case MapleStatus.StatusType.CRITICAL_DAMAGE:
                    CriticalDamage += value;
                    break;
                case MapleStatus.StatusType.IGNORE_DEF:
                    IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, value, isAdd);
                    break;
                case MapleStatus.StatusType.ITEM_DROP:
                    ItemDropIncrease += value;
                    break;
                case MapleStatus.StatusType.MESO_DROP:
                    MesoDropIncrease += value;
                    break;
                case MapleStatus.StatusType.COOL_DEC_SECOND:
                    CooldownDecreaseValue += value;
                    break;
            }
        }
    }
    
    ///<summary>
    ///    MapleOption 객체의 정보를 현재 스텟에 적용하는 것으로, 세트 아이템 효과 적용에 사용합니다.
    ///</summary>
    // private void ApplyMapleOption(MapleOption option)
    // {
    //     MainStat.BaseValue += GetStatFromOption(option, MainStat.Stat);
    //     SubStat.BaseValue += GetStatFromOption(option, SubStat.Stat);
    //     SubStat2.BaseValue += GetStatFromOption(option, SubStat2.Stat);
    //     AttackValue += AttackType ==  MapleStatus.StatusType.ATTACK_POWER ? option.AttackPower : option.MagicPower;
    //     BossDamage += option.BossDamage;
    //     Damage += option.Damage;
    //     CommonDamage += option.CommonDamage;
    //     CriticalDamage += option.CriticalDamage;
    //     IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, option.IgnoreArmor, option.IgnoreArmor >= 0);
    //     
    //     if (MainStat.Stat ==  MapleStatus.StatusType.HP)
    //         MainStat.RateValue += option.MaxHpRate;
    // }
    
    ///<summary>
    ///    아이템 착용과 해제를 현재 스텟에 시뮬레이션합니다.
    ///</summary>
    private void ApplyAddSub(MapleCommonItem item, bool isAdd)
    {
        int sign = isAdd ? 1 : -1;
        
        // 아이템 기본 효과 적용
        // MapleOption itemOption = GetOption(item);
        // MainStat.BaseValue += GetStatFromOption(itemOption, MainStat.Stat) * sign;
        // SubStat.BaseValue += GetStatFromOption(itemOption, SubStat.Stat) * sign;
        // SubStat2.BaseValue += GetStatFromOption(itemOption, SubStat2.Stat) * sign;
        // MainStat.RateValue += itemOption.AllStatRate * sign;
        // SubStat.RateValue += itemOption.AllStatRate * sign;
        // SubStat2.RateValue += itemOption.AllStatRate * sign;
        // AttackValue += AttackType ==  MapleStatus.StatusType.ATTACK_POWER ? itemOption.AttackPower : itemOption.MagicPower;
        // BossDamage += itemOption.BossDamage;
        // IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, itemOption.IgnoreArmor, isAdd);
        
        if (isAdd)  PlayerStat += item.Status;
        else        PlayerStat -= item.Status;
        
        // 잠재능력 적용
        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials, isAdd);
            ApplyPotential(item.Potential.Additionals, isAdd);
        }
        
        // 칭호 효과 적용
        // ApplyPotential(item.Specials.ToArray(), isAdd);
        
        // 세트 효과 적용
        MapleStatContainer setEffectPrev = SetEffects.GetSetOptions();
        if(isAdd)
            SetEffects.Add(item);
        else
            SetEffects.Sub(item);
        PlayerStat += SetEffects.GetSetOptions() - setEffectPrev;
    }
    
    ///<summary>
    ///    아이템을 장착합니다.
    ///</summary>
    public void AddCommonItem(MapleCommonItem item)
    {
        ApplyAddSub(item, true);
    }
    
    ///<summary>
    ///    아이템을 해제합니다.
    ///</summary>
    public void SubCommonItem(MapleCommonItem item)
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
    
    private void ApplyAbilityEach(MapleStatus.StatusType type, int value)
    {
        if (type == MainStat.Stat) MainStat.FlatValue += value;
        else if (type == MainStat.StatRate) MainStat.RateValue += value;
        else if (type == SubStat.Stat) SubStat.FlatValue += value;
        else if (type == SubStat2.Stat) SubStat2.FlatValue += value;
        else if (type == AttackType)
        {
            if (type is MapleStatus.StatusType.MAG_PER_LEVEL or MapleStatus.StatusType.ATK_PER_LEVEL)
                value = (int) (BuilderDataContainer.CharacterInfo!.Level / value);
            AttackValue += value;
        }
        
        switch (type)
        {
            case MapleStatus.StatusType.CRITICAL_CHANCE:
                CriticalChance += value;
                break;
            case MapleStatus.StatusType.ALL_STAT:
                MainStat.FlatValue += value;
                SubStat.FlatValue += value;
                SubStat2.FlatValue += value;
                break;
            case MapleStatus.StatusType.BOSS_DAMAGE:
                BossDamage += value;
                break;
            case MapleStatus.StatusType.COMMON_DAMAGE:
                CommonDamage += value;
                break;
            case MapleStatus.StatusType.DEBUFF_DAMAGE:
                DebuffDamage += value;
                break;
            case MapleStatus.StatusType.COOL_IGNORE:
                CooldownIgnoreRate += value;
                break;
            case MapleStatus.StatusType.BUFF_DURATION:
                BuffDurationIncrease += value;
                break;
            case MapleStatus.StatusType.ITEM_DROP:
                ItemDropIncrease += value;
                break;
            case MapleStatus.StatusType.MESO_DROP:
                MesoDropIncrease += value;
                break;
            case MapleStatus.StatusType.ATTACK_SPEED:
            case MapleStatus.StatusType.PASSIVE_LEVEL:
            case MapleStatus.StatusType.TARGET_COUNT:
            case MapleStatus.StatusType.OTHER:
            default:
                break;
        }
    }

    public void ApplyAbility(Dictionary<MapleStatus.StatusType, int> abilities)
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
    
    public void ApplyHyperStat(MapleStatus.StatusType type, double delta)
    {
        if (type == MainStat.Stat) MainStat.FlatValue += (int) delta;
        else if (type == MainStat.StatRate) MainStat.RateValue += (int) delta;
        else if (type == SubStat.Stat) SubStat.FlatValue += (int) delta;
        else if (type == SubStat2.Stat) SubStat2.FlatValue += (int) delta;
        
        switch (type)
        {
            case MapleStatus.StatusType.CRITICAL_CHANCE:
                CriticalChance += delta;
                break;
            case MapleStatus.StatusType.CRITICAL_DAMAGE:
                CriticalDamage += delta;
                break;
            case MapleStatus.StatusType.IGNORE_DEF:
                IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, delta, delta >= 0);
                break;
            case MapleStatus.StatusType.DAMAGE:
                Damage += (int) delta;
                break;
            case MapleStatus.StatusType.BOSS_DAMAGE:
                BossDamage += (int) delta;
                break;
            case MapleStatus.StatusType.COMMON_DAMAGE:
                CommonDamage += (int) delta;
                break;
            case MapleStatus.StatusType.ABN_STATUS_RESIS:
                Immune += (int) delta;
                break;
            case MapleStatus.StatusType.ATTACK_POWER:
                AttackValue += (int) delta;
                break;
            case MapleStatus.StatusType.MP:
            case MapleStatus.StatusType.DF_TT_PP:
            case MapleStatus.StatusType.EXP_INCREASE:
            case MapleStatus.StatusType.ARCANE_FORCE:
                break;
        }
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 유니온 공격대 효과 적용
    
    public void ApplyUnionRaider(MapleStatus.StatusType type, int delta)
    {
        if (type == MainStat.Stat) MainStat.FlatValue += delta;
        else if (type == MainStat.StatRate) MainStat.RateValue += delta;
        else if (type == SubStat.Stat) SubStat.FlatValue += delta;
        else if (type == SubStat2.Stat) SubStat2.FlatValue += delta;

        switch (type)
        {
            case MapleStatus.StatusType.BOSS_DAMAGE:
                BossDamage += delta;
                break;
            case MapleStatus.StatusType.IGNORE_DEF:
                IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, delta, delta >= 0);
                break;
            case MapleStatus.StatusType.ABN_STATUS_RESIS:
                Immune += delta;
                break;
            case MapleStatus.StatusType.STR_DEX_LUK:
                MapleStatus.StatusType[] options =
                {
                    MapleStatus.StatusType.STR, MapleStatus.StatusType.DEX,
                    MapleStatus.StatusType.LUK
                };
                if (options.Contains(MainStat.Stat)) MainStat.FlatValue += delta;
                if (options.Contains(SubStat.Stat)) SubStat.FlatValue += delta;
                if (options.Contains(SubStat2.Stat)) SubStat2.FlatValue += delta;
                break;
            case MapleStatus.StatusType.ATTACK_AND_MAGIC:
                AttackValue += delta;
                break;
            case MapleStatus.StatusType.MESO_DROP:
                MesoDropIncrease += delta;
                break;
            case MapleStatus.StatusType.CRITICAL_CHANCE:
                CriticalChance += delta;
                break;
            case MapleStatus.StatusType.BUFF_DURATION:
                BuffDurationIncrease += delta;
                break;
            case MapleStatus.StatusType.SUMMON_DURATION:
                SummonDurationIncrease += delta;
                break;
            case MapleStatus.StatusType.CRITICAL_DAMAGE:
                CriticalDamage += delta;
                break;
            case MapleStatus.StatusType.COOL_DEC_RATE:
                CooldownDecreaseRate += delta;
                break;
            case MapleStatus.StatusType.OTHER:
            case MapleStatus.StatusType.INT:
            case MapleStatus.StatusType.STR:
            case MapleStatus.StatusType.LUK:
            case MapleStatus.StatusType.DEX:
            case MapleStatus.StatusType.HP:
            case MapleStatus.StatusType.HP_RATE:
            case MapleStatus.StatusType.MP_RATE:
            case MapleStatus.StatusType.EXP_INCREASE:
            case MapleStatus.StatusType.WILD_HUNTER_DMG:
            default:
                break;

        }

        Summarize.DispatchSummary();
    }
    
    #endregion
    
    #region

    public void ApplyPetItem(List<MaplePetItem> petItems)
    {
        foreach (MaplePetItem petItem in petItems)
        {
            Console.WriteLine($"[{petItem.Name}] 공 : {petItem.Status[MapleStatus.StatusType.ATTACK_POWER]} / 마 : {petItem.Status[MapleStatus.StatusType.MAGIC_POWER]}");
            AttackValue += AttackType == MapleStatus.StatusType.ATTACK_POWER 
                ? (int) petItem.Status[MapleStatus.StatusType.ATTACK_POWER] 
                : (int) petItem.Status[MapleStatus.StatusType.MAGIC_POWER];
        }
    }
    
    #endregion
    
    // TODO : 스텟 적용 테스트 - 기본수치 4422, 실제수치 4477, 차이 125 - 아르카나세트 올스텟 15
}