using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control;

public class PlayerInfo
{
    public class StatInfo
    {
        public MaplePotentialOptionType StatType { get; }
        public MaplePotentialOptionType StatRateType { get; }
        public MaplePotentialOptionType StatLevelType { get; }
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
            StatLevelType = statType switch
            {
                MaplePotentialOptionType.STR => MaplePotentialOptionType.LEVEL_STR,
                MaplePotentialOptionType.DEX => MaplePotentialOptionType.LEVEL_DEX,
                MaplePotentialOptionType.INT => MaplePotentialOptionType.LEVEL_INT,
                MaplePotentialOptionType.LUK => MaplePotentialOptionType.LEVEL_LUK,
                _ => MaplePotentialOptionType.OTHER
            };
            BaseValue = 4;
            RateValue = 0;
            FlatValue = 0;
        }
    }

    public enum SetType
    {
        NECRO,          // 120제 네크로
        MUSPELL,        // 130제 쟈이힌, 무스펠
        ROYAL,          // 130제 반레온
        PENSALIR,       // 140제 펜살리르
        MAISTER,        // 140제 마이스터
        CYGNUS,         // 140제 여제
        ROOTABYSS,      // 150제 루타
        ABSOLABSE,      // 160제 앱솔
        ARCANESHADE,    // 200제 아케인셰이드
        ETERNEL,        // 250제 에테르넬
        MONSTERPARK,    // 칠요세트
        BOSSACCESSORY,  // 보스 장신구
        DAWN,           // 여명
        BLACK,          // 칠흑
        NONE
    }

    public class SetEffect
    {
        private Dictionary<SetType, int> Sets { get; }
        private List<SetType> LuckySets { get; set; }
        private readonly Dictionary<SetType, List<string>> setMap = new()
        {
            {SetType.NECRO, new List<string> {"네크로"}},
            {SetType.MUSPELL, new List<string> { "쟈이힌", "무스펠" }},
            {SetType.ROYAL, new List<string> { "로얄 반 레온" }},
            {SetType.PENSALIR, new List<string> { "우트가르드", "펜살리르" }},
            {SetType.MAISTER, new List<string> { "마이스터" }},
            {SetType.CYGNUS, new List<string> { "라이온하트", "드래곤테일", "팔콘윙", "레이븐혼", "샤크투스" }},
            {SetType.ROOTABYSS, new List<string> { "하이네스", "이글아이", "트릭스터", "파프니르" }},
            {SetType.ABSOLABSE, new List<string> { "앱솔랩스" }},
            {SetType.ARCANESHADE, new List<string> { "아케인셰이드" }},
            {SetType.ETERNEL, new List<string> { "에테르넬" }},
            {SetType.MONSTERPARK, new List<string> { "칠요의" }},
            {SetType.BOSSACCESSORY, new List<string> { "응축된 힘", "아쿠아틱", "블랙빈 마", "파풀", "지옥", "데아",
                "실버블라", "고귀한 이", "가디언 엔젤 링", "혼테일", "카오스 혼테일", "매커", "도미", "골든 클", 
                "분노한 자쿰의 벨트", "로얄 블", "핑크빛", "영생의 돌", "크리스탈 웬"}},
            {SetType.DAWN, new List<string> { "트와일라이", "에스텔", "여명의 가", "데이브"}},
            {SetType.BLACK, new List<string> { "루즈 컨", "마력이", "블랙 하", "몽환의 벨", "고통", "창세", "커맨더 포", "거대한 공", 
                "저주받은 적", "저주받은 녹", "저주받은 청", "저주받은 황", "미트라의 분노"}}
        };

        protected internal SetEffect()
        {
            Sets = new Dictionary<SetType, int>();
            LuckySets = new List<SetType>();
        }

        private SetType GetSetType(MapleItem item)
        {
            foreach (var pair in setMap.Where(pair => pair.Value.Any(prefix => item.Name.StartsWith(prefix))))
                return pair.Key;
            return SetType.NONE;
        }

        private List<SetType> GetLuckySets(MapleItem item)
        {
            if (item.Name.StartsWith("제네시스"))
                return new List<SetType>
                {
                    SetType.NECRO, SetType.MUSPELL, SetType.ROYAL, SetType.PENSALIR, SetType.MAISTER, SetType.CYGNUS,
                    SetType.ROOTABYSS, SetType.ABSOLABSE, SetType.ARCANESHADE, SetType.ETERNEL
                };
            if (item.Name.Equals("카오스 반반 투구") || item.Name.Equals("카오스 벨룸의 헬름") || item.Name.Equals("카오스 피에르 모자")
                || item.Name.Equals("카오스 퀸의 티아라"))
                return new List<SetType>
                {
                    SetType.NECRO, SetType.MUSPELL, SetType.ROYAL, SetType.PENSALIR, SetType.CYGNUS,
                    SetType.ROOTABYSS, SetType.ABSOLABSE, SetType.ARCANESHADE, SetType.ETERNEL
                };
            if (item.Name.StartsWith("스칼렛 링"))
                return new List<SetType>
                {
                    SetType.BOSSACCESSORY, SetType.DAWN, SetType.BLACK
                };
            return new List<SetType>();
        }
        
        private MapleOption GetSetOption(SetType setType, int level)
        {
            MapleOption option = new MapleOption();
            if (LuckySets.Contains(setType) && level >= 3) level++;
            switch (setType)
            {
                case SetType.NECRO:
                    switch (Math.Min(level, 5))
                    {
                        case 5:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.BossDamage += 10;
                            goto case 4;
                        case 4:
                            option.AllStat += 5;
                            goto case 3;
                        case 3:
                            option.BossDamage += 5;
                            goto case 2;
                        case 2:
                            option.AllStat = 3;
                            break;
                    }
                    break;
                case SetType.MUSPELL:
                    switch (Math.Min(level, 5))
                    {
                        case 5:
                            option.BossDamage += 6;
                            option.ApplyIgnoreArmor(10);
                            option.AllStat += 10;
                            goto case 4;
                        case 4:
                            option.AttackPower += 8;
                            option.MagicPower += 8;
                            option.AllStat += 8; // 주스텟만 이지만 임의로..
                            option.Damage += 4; // 일몹뎀임
                            goto case 3;
                        case 3:
                            option.MaxHpRate += 8;
                            option.MaxMpRate += 8;
                            option.Damage += 2; // 일몹뎀임
                            goto case 2;
                        case 2:
                            break;
                    }
                    break;
                case SetType.ROYAL:
                    switch (Math.Min(level, 6))
                    {
                        case 6:
                            option.AllStat += 25;
                            option.MaxHpRate += 15;
                            option.MaxMpRate += 15;
                            option.AttackPower += 20;
                            option.MagicPower += 20;
                            option.BossDamage += 10;
                            goto case 5;
                        case 5:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 9;
                            goto case 4;
                        case 4:
                            option.AllStat += 6;
                            option.BossDamage += 10;
                            break;
                    }
                    break;
                case SetType.PENSALIR:
                    switch (Math.Min(level, 6))
                    {
                        case 6:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.ApplyIgnoreArmor(10);
                            option.Damage += 8; // 일몹뎀
                            goto case 5;
                        case 5:
                            option.AllStat += 15;
                            option.ApplyIgnoreArmor(10);
                            option.Damage += 6; // 일몹뎀
                            goto case 4;
                        case 4:
                            option.AttackPower += 9;
                            option.MagicPower += 9;
                            option.AllStat += 9;
                            option.Damage += 4; // 일몹뎀
                            goto case 3;
                        case 3:
                            option.MaxHpRate += 9;
                            option.MaxMpRate += 9;
                            option.Damage += 2; // 일몹뎀
                            break;
                    }
                    break;
                case SetType.MAISTER:
                    switch (Math.Min(level, 4))
                    {
                        case 4:
                            option.BossDamage += 20;
                            goto case 3;
                        case 3:
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            goto case 2;
                        case 2:
                            option.MaxHpRate += 10;
                            option.MaxMpRate += 10;
                            break;
                    }
                    break;
                case SetType.CYGNUS:
                    switch (Math.Min(level, 7))
                    {
                        case 7:
                            option.MaxHpRate = 15;
                            option.MaxMpRate = 15;
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            goto case 6;
                        case 6:
                            option.AttackPower += 30;
                            option.MagicPower += 30;
                            option.BossDamage += 30;
                            goto case 5;
                        case 5:
                            option.AllStat += 20;
                            goto case 4;
                        case 4:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            goto case 3;
                        case 3:
                            option.MaxHpRate = 15;
                            option.MaxMpRate = 15;
                            goto case 2;
                        case 2:
                            break;
                    }
                    break;
                case SetType.ROOTABYSS:
                    switch (Math.Min(level, 4))
                    {
                        case 4:
                            option.BossDamage += 30;
                            goto case 3;
                        case 3:
                            option.MaxHpRate = 10;
                            option.MaxMpRate = 10;
                            option.AttackPower += 50;
                            option.MagicPower += 50;
                            goto case 2;
                        case 2:
                            option.AttackPower += 20;
                            option.MagicPower += 20;
                            option.MaxHp += 1000;
                            option.MaxMp += 1000;
                            break;
                    }
                    break;
                case SetType.ABSOLABSE:
                    switch (Math.Min(level, 7))
                    {
                        case 7:
                            option.ApplyIgnoreArmor(10);
                            option.AttackPower += 20;
                            option.MagicPower += 20;
                            goto case 6;
                        case 6:
                            option.MaxHpRate += 20;
                            option.MaxMpRate += 20;
                            option.AttackPower += 20;
                            option.MagicPower += 20;
                            goto case 5;
                        case 5:
                            option.BossDamage += 10;
                            option.AttackPower += 30;
                            option.MagicPower += 30;
                            goto case 4;
                        case 4:
                            option.AttackPower += 25;
                            option.MagicPower += 25;
                            option.ApplyIgnoreArmor(10);
                            goto case 3;
                        case 3:
                            option.AllStat += 30;
                            option.AttackPower += 20;
                            option.MagicPower += 20;
                            option.BossDamage += 10;
                            goto case 2;
                        case 2:
                            option.MaxHp += 1500;
                            option.MaxMp += 1500;
                            option.AttackPower += 20;
                            option.MagicPower += 20;
                            option.BossDamage += 10;
                            break;
                    }
                    break;
                case SetType.ARCANESHADE:
                    switch (Math.Min(level, 7))
                    {
                        case 7:
                            option.AttackPower += 30;
                            option.MagicPower += 30;
                            option.ApplyIgnoreArmor(10);
                            goto case 6;
                        case 6:
                            option.AttackPower += 30;
                            option.MagicPower += 30;
                            option.MaxHpRate += 30;
                            option.MaxMpRate += 30;
                            goto case 5;
                        case 5:
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            option.MaxHp += 2000;
                            option.MaxMp += 2000;
                            option.BossDamage += 10;
                            goto case 4;
                        case 4:
                            option.AttackPower += 35;
                            option.MagicPower += 35;
                            option.AllStat += 50;
                            option.BossDamage += 10;
                            goto case 3;
                        case 3:
                            option.AttackPower += 30;
                            option.MagicPower += 30;
                            option.ApplyIgnoreArmor(10);
                            goto case 2;
                        case 2:
                            option.AttackPower += 30;
                            option.MagicPower += 30;
                            option.BossDamage += 10;
                            break;
                    }
                    break;
                case SetType.ETERNEL:
                    switch (Math.Min(level, 8))
                    {
                        case 8:
                            option.BossDamage += 10;
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            goto case 7;
                        case 7:
                            option.BossDamage += 10;
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            option.AllStat += 50;
                            option.MaxHp += 2500;
                            option.MaxMp += 2500;
                            goto case 6;
                        case 6:
                            option.BossDamage += 30;
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            option.MaxHpRate += 15;
                            option.MaxMpRate += 15;
                            goto case 5;
                        case 5:
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            option.ApplyIgnoreArmor(20);
                            goto case 4;
                        case 4:
                            option.AttackPower += 45;
                            option.MagicPower += 45;
                            option.BossDamage += 10;
                            option.MaxHpRate += 15;
                            option.MaxMpRate += 15;
                            goto case 3;
                        case 3:
                            option.AllStat += 50;
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            option.BossDamage += 10;
                            goto case 2;
                        case 2:
                            option.MaxHp += 2500;
                            option.MaxMp += 2500;
                            option.AttackPower += 40;
                            option.MagicPower += 40;
                            option.BossDamage += 10;
                            break;
                    }
                    break;
                case SetType.MONSTERPARK:
                    switch (Math.Min(level, 2))
                    {
                        case 2:
                            option.ApplyIgnoreArmor(10);
                            break;
                    }
                    break;
                case SetType.BOSSACCESSORY:
                    switch (Math.Min(level, 9))
                    {
                        case 9:
                            option.BossDamage += 10;
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 15;
                            goto case 7;
                        case 7:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 10;
                            option.ApplyIgnoreArmor(10);
                            goto case 5;
                        case 5:
                            option.AllStat += 10;
                            option.MaxHpRate += 5;
                            option.MaxMpRate += 5;
                            option.AttackPower += 5;
                            option.MagicPower += 5;
                            goto case 3;
                        case 3:
                            option.AllStat += 10;
                            option.MaxHpRate += 5;
                            option.MaxMpRate += 5;
                            option.AttackPower += 5;
                            option.MagicPower += 5;
                            break;
                    }
                    break;
                case SetType.DAWN:
                    switch (Math.Min(level, 4))
                    {
                        case 4:
                            option.ApplyIgnoreArmor(10);
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 10;
                            option.MaxHp += 250;
                            goto case 3;
                        case 3:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 10;
                            option.MaxHp += 250;
                            goto case 2;
                        case 2:
                            option.BossDamage += 10;
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 10;
                            option.MaxHp += 250;
                            break;
                    }
                    break;
                case SetType.BLACK:
                    switch (Math.Min(level, 9))
                    {
                        case 9:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            option.AllStat += 15;
                            option.MaxHp += 375;
                            option.CriticalDamage += 5;
                            goto case 8;
                        case 8:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            option.AllStat += 15;
                            option.MaxHp += 375;
                            option.BossDamage += 10;
                            goto case 7;
                        case 7:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            option.AllStat += 15;
                            option.MaxHp += 375;
                            option.CriticalDamage += 5;
                            goto case 6;
                        case 6:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            option.AllStat += 15;
                            option.MaxHp += 375;
                            option.ApplyIgnoreArmor(10);
                            goto case 5;
                        case 5:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            option.AllStat += 15;
                            option.MaxHp += 375;
                            option.BossDamage += 10;
                            goto case 4;
                        case 4:
                            option.AttackPower += 15;
                            option.MagicPower += 15;
                            option.AllStat += 15;
                            option.MaxHp += 375;
                            option.CriticalDamage += 5;
                            goto case 3;
                        case 3:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 10;
                            option.MaxHp += 250;
                            option.ApplyIgnoreArmor(10);
                            goto case 2;
                        case 2:
                            option.AttackPower += 10;
                            option.MagicPower += 10;
                            option.AllStat += 10;
                            option.MaxHp += 250;
                            option.BossDamage += 10;
                            break;
                    }

                    break;
                case SetType.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(setType), setType, null);
            }

            return option;
        }

        public MapleOption GetSetOptions()
        {
            MapleOption result = new MapleOption();
            foreach (var pair in Sets)
            {
                result += GetSetOption(pair.Key, pair.Value);
            }
            return result;
        }

        public string GetSetOptionString()
        {
            return Sets.Aggregate("", (current, pair) => current + $"{pair.Key} {pair.Value + (LuckySets.Contains(pair.Key) && pair.Value >= 3 ? 1 : 0)} ");
        }

        public void Add(MapleItem item)
        {
            SetType type = GetSetType(item);
            if( type == SetType.NONE )
            {
                var luckySets = GetLuckySets(item);
                if (luckySets.Count == 0 || LuckySets.Count > 0) return;
                LuckySets = luckySets;
                return;
            }
            
            Sets.TryAdd(type, 0);
            Sets[type] += 1;
        }

        public void Sub(MapleItem item)
        {
            SetType type = GetSetType(item);
            if (type == SetType.NONE)
            {
                var luckySets = GetLuckySets(item);
                if(luckySets.Count > 0) LuckySets.Clear();
                return;
            }

            if (!Sets.ContainsKey(type)) return;
            Sets[type] -= 1;
            if (Sets[type] <= 0) Sets.Remove(type);
        }

        public static string GetSetTypeString(SetType setType)
        {
            return setType switch
            {
                SetType.NECRO => "네크로",
                SetType.MUSPELL => "무스펠",
                SetType.ROYAL => "반레온",
                SetType.PENSALIR => "펜살",
                SetType.MAISTER => "마이스터",
                SetType.CYGNUS => "여제",
                SetType.ROOTABYSS => "루타",
                SetType.ABSOLABSE => "앱솔",
                SetType.ARCANESHADE => "아케인",
                SetType.ETERNEL => "에테",
                SetType.MONSTERPARK => "칠요",
                SetType.BOSSACCESSORY => "보장",
                SetType.DAWN => "여명",
                SetType.BLACK => "칠흑",
                SetType.NONE => "",
                _ => throw new ArgumentOutOfRangeException(nameof(setType), setType, null)
            };
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
    public int LevelStat { get; private set; }
    public double FinalDamage { get; private set; }
    public double IgnoreArmor { get; private set; }
    public double CriticalChance { get; private set; }
    public double CriticalDamage { get; private set; }
    public double IgnoreImmune { get; private set; }
    public SetEffect SetEffects { get; private set; }

    public PlayerInfo(uint level, MaplePotentialOptionType mainStat, MaplePotentialOptionType subStat, MaplePotentialOptionType subStat2 = MaplePotentialOptionType.OTHER)
    {
        MainStat = new StatInfo(mainStat);
        MainStat.BaseValue += (int) (level - 1) * (mainStat == MaplePotentialOptionType.MAX_HP ? 50 : 5);
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

        LevelStat = (int) Math.Floor(level / 9.0);
        SetEffects = new SetEffect();
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
            if      (pair.Key == MainStat.StatType)      MainStat.BaseValue += value;
            else if (pair.Key == MainStat.StatRateType)  MainStat.RateValue += value;
            else if (pair.Key == MainStat.StatLevelType) MainStat.BaseValue += LevelStat * value;
            else if (pair.Key == SubStat.StatType)       SubStat.BaseValue += value;
            else if (pair.Key == SubStat.StatRateType)   SubStat.RateValue += value;
            else if (pair.Key == SubStat.StatLevelType)  SubStat.BaseValue += LevelStat * value;
            else if (pair.Key == SubStat2.StatType)      SubStat2.BaseValue += value;
            else if (pair.Key == SubStat2.StatRateType)  SubStat2.RateValue += value;
            else if (pair.Key == SubStat2.StatLevelType) SubStat2.BaseValue += LevelStat * value;
            else if (pair.Key == AttackType)             AttackValue += value;
            else if (pair.Key == AttackRateType)         AttackRate += value;
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

    private void ApplyMapleOption(MapleOption option)
    {
        MainStat.BaseValue += GetStatFromOption(option, MainStat.StatType) + option.AllStat;
        SubStat.BaseValue += GetStatFromOption(option, SubStat.StatType) + option.AllStat;
        SubStat2.BaseValue += GetStatFromOption(option, SubStat2.StatType) + option.AllStat;
        AttackValue += AttackType == MaplePotentialOptionType.ATTACK ? option.AttackPower : option.MagicPower;
        BossDamage += option.BossDamage;
        Damage += option.Damage;
        CommonDamage += option.CommonDamage;
        CriticalDamage += option.CriticalDamage;
        IgnoreArmor = CalcIgnoreArmor(IgnoreArmor, option.IgnoreArmor, option.IgnoreArmor >= 0);
        
        if (MainStat.StatType == MaplePotentialOptionType.MAX_HP)
            MainStat.RateValue += option.MaxHpRate;
    }
    
    public void AddItem(MapleItem item)
    {
        // 아이템 기본 효과 적용
        MapleOption itemOption = GetOption(item);
        MainStat.BaseValue += GetStatFromOption(itemOption, MainStat.StatType);
        SubStat.BaseValue += GetStatFromOption(itemOption, SubStat.StatType);
        SubStat2.BaseValue += GetStatFromOption(itemOption, SubStat2.StatType);
        
        // 잠재능력 적용
        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials);
            ApplyPotential(item.Potential.Additionals);
        }
        
        // 칭호 효과 적용
        ApplyPotential(item.Specials.ToArray());
        
        // 세트 효과 적용
        MapleOption setEffectPrev = SetEffects.GetSetOptions();
        SetEffects.Add(item);
        ApplyMapleOption(SetEffects.GetSetOptions() - setEffectPrev);
    }

    public void SubtractItem(MapleItem item)
    {
        // 아이템 기본 효과 적용
        MapleOption itemOption = GetOption(item);
        MainStat.BaseValue -= GetStatFromOption(itemOption, MainStat.StatType);
        SubStat.BaseValue -= GetStatFromOption(itemOption, SubStat.StatType);
        SubStat2.BaseValue -= GetStatFromOption(itemOption, SubStat2.StatType);
        
        // 잠재능력 적용
        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials, false);
            ApplyPotential(item.Potential.Additionals, false);
        }
        
        // 칭호 효과 적용
        ApplyPotential(item.Specials.ToArray(), false);
        
        // 세트 효과 적용
        MapleOption setEffectPrev = SetEffects.GetSetOptions();
        SetEffects.Sub(item);
        ApplyMapleOption(SetEffects.GetSetOptions() - setEffectPrev);
    }
    
    
    
    
}