using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;

namespace MapleBuilder.MapleData;

public class SetEffect
{
    public enum SetType
    {
        NECRO, // 120제 네크로
        MUSPELL, // 130제 쟈이힌, 무스펠
        ROYAL, // 130제 반레온
        PENSALIR, // 140제 펜살리르
        MAISTER, // 140제 마이스터
        CYGNUS, // 140제 여제
        ROOTABYSS, // 150제 루타
        ABSOLABSE, // 160제 앱솔
        ARCANESHADE, // 200제 아케인셰이드
        ETERNEL, // 250제 에테르넬
        MONSTERPARK, // 칠요세트
        BOSSACCESSORY, // 보스 장신구
        DAWN, // 여명
        BLACK, // 칠흑
        NONE
    }

    private Dictionary<SetType, int> Sets { get; }
    private List<SetType> LuckySets { get; set; }

    private readonly Dictionary<SetType, List<string>> setMap = new()
    {
        {SetType.NECRO, new List<string> {"네크로"}},
        {SetType.MUSPELL, new List<string> {"쟈이힌", "무스펠"}},
        {SetType.ROYAL, new List<string> {"로얄 반 레온"}},
        {SetType.PENSALIR, new List<string> {"우트가르드", "펜살리르"}},
        {SetType.MAISTER, new List<string> {"마이스터"}},
        {SetType.CYGNUS, new List<string> {"라이온하트", "드래곤테일", "팔콘윙", "레이븐혼", "샤크투스"}},
        {SetType.ROOTABYSS, new List<string> {"하이네스", "이글아이", "트릭스터", "파프니르"}},
        {SetType.ABSOLABSE, new List<string> {"앱솔랩스"}},
        {SetType.ARCANESHADE, new List<string> {"아케인셰이드"}},
        {SetType.ETERNEL, new List<string> {"에테르넬"}},
        {SetType.MONSTERPARK, new List<string> {"칠요의"}},
        {
            SetType.BOSSACCESSORY, new List<string>
            {
                "응축된 힘", "아쿠아틱", "블랙빈 마", "파풀", "지옥", "데아",
                "실버블라", "고귀한 이", "가디언 엔젤 링", "혼테일", "카오스 혼테일", "매커", "도미", "골든 클",
                "분노한 자쿰의 벨트", "로얄 블", "핑크빛", "영생의 돌", "크리스탈 웬"
            }
        },
        {SetType.DAWN, new List<string> {"트와일라이", "에스텔", "여명의 가", "데이브"}},
        {
            SetType.BLACK, new List<string>
            {
                "루즈 컨", "마력이", "블랙 하", "몽환의 벨", "고통", "창세", "커맨더 포", "거대한 공",
                "저주받은 적", "저주받은 녹", "저주받은 청", "저주받은 황", "미트라의 분노"
            }
        }
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
                        option.AllStatFlatInc = 5;
                        goto case 3;
                    case 3:
                        option.BossDamage += 5;
                        goto case 2;
                    case 2:
                        option.AllStatFlatInc = 3;
                        break;
                }

                break;
            case SetType.MUSPELL:
                switch (Math.Min(level, 5))
                {
                    case 5:
                        option.BossDamage += 6;
                        option.ApplyIgnoreArmor(10);
                        option.AllStatFlatInc = 10;
                        goto case 4;
                    case 4:
                        option.AttackPower += 8;
                        option.MagicPower += 8;
                        option.AllStatFlatInc = 8; // 주스텟만 이지만 임의로..
                        option.CommonDamage += 4;
                        goto case 3;
                    case 3:
                        option.MaxHpRate += 8;
                        option.MaxMpRate += 8;
                        option.CommonDamage += 2;
                        goto case 2;
                    case 2:
                        break;
                }

                break;
            case SetType.ROYAL:
                switch (Math.Min(level, 6))
                {
                    case 6:
                        option.AllStatFlatInc = 25;
                        option.MaxHpRate += 15;
                        option.MaxMpRate += 15;
                        option.AttackPower += 20;
                        option.MagicPower += 20;
                        option.BossDamage += 10;
                        goto case 5;
                    case 5:
                        option.AttackPower += 10;
                        option.MagicPower += 10;
                        option.AllStatFlatInc = 9;
                        goto case 4;
                    case 4:
                        option.AllStatFlatInc = 6;
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
                        option.CommonDamage += 8;
                        goto case 5;
                    case 5:
                        option.AllStatFlatInc = 15;
                        option.ApplyIgnoreArmor(10);
                        option.CommonDamage += 6;
                        goto case 4;
                    case 4:
                        option.AttackPower += 9;
                        option.MagicPower += 9;
                        option.AllStatFlatInc = 9;
                        option.CommonDamage += 4;
                        goto case 3;
                    case 3:
                        option.MaxHpRate += 9;
                        option.MaxMpRate += 9;
                        option.CommonDamage += 2;
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
                        option.AllStatFlatInc = 20;
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
                        option.AllStatFlatInc = 20;
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
                        option.AllStatFlatInc = 30;
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
                        option.AllStatFlatInc = 50;
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
                        option.AllStatFlatInc = 50;
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
                        option.AllStatFlatInc = 50;
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
                        option.AllStatFlatInc = 15;
                        goto case 7;
                    case 8:
                    case 7:
                        option.AttackPower += 10;
                        option.MagicPower += 10;
                        option.AllStatFlatInc = 10;
                        option.ApplyIgnoreArmor(10);
                        goto case 5;
                    case 6:
                    case 5:
                        option.AllStatFlatInc = 10;
                        option.MaxHpRate += 5;
                        option.MaxMpRate += 5;
                        option.AttackPower += 5;
                        option.MagicPower += 5;
                        goto case 3;
                    case 4:
                    case 3:
                        option.AllStatFlatInc = 10;
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
                        option.AllStatFlatInc = 10;
                        option.MaxHp += 250;
                        goto case 3;
                    case 3:
                        option.AttackPower += 10;
                        option.MagicPower += 10;
                        option.AllStatFlatInc = 10;
                        option.MaxHp += 250;
                        goto case 2;
                    case 2:
                        option.BossDamage += 10;
                        option.AttackPower += 10;
                        option.MagicPower += 10;
                        option.AllStatFlatInc = 10;
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
                        option.AllStatFlatInc = 15;
                        option.MaxHp += 375;
                        option.CriticalDamage += 5;
                        goto case 8;
                    case 8:
                        option.AttackPower += 15;
                        option.MagicPower += 15;
                        option.AllStatFlatInc = 15;
                        option.MaxHp += 375;
                        option.BossDamage += 10;
                        goto case 7;
                    case 7:
                        option.AttackPower += 15;
                        option.MagicPower += 15;
                        option.AllStatFlatInc = 15;
                        option.MaxHp += 375;
                        option.CriticalDamage += 5;
                        goto case 6;
                    case 6:
                        option.AttackPower += 15;
                        option.MagicPower += 15;
                        option.AllStatFlatInc = 15;
                        option.MaxHp += 375;
                        option.ApplyIgnoreArmor(10);
                        goto case 5;
                    case 5:
                        option.AttackPower += 15;
                        option.MagicPower += 15;
                        option.AllStatFlatInc = 15;
                        option.MaxHp += 375;
                        option.BossDamage += 10;
                        goto case 4;
                    case 4:
                        option.AttackPower += 15;
                        option.MagicPower += 15;
                        option.AllStatFlatInc = 15;
                        option.MaxHp += 375;
                        option.CriticalDamage += 5;
                        goto case 3;
                    case 3:
                        option.AttackPower += 10;
                        option.MagicPower += 10;
                        option.AllStatFlatInc = 10;
                        option.MaxHp += 250;
                        option.ApplyIgnoreArmor(10);
                        goto case 2;
                    case 2:
                        option.AttackPower += 10;
                        option.MagicPower += 10;
                        option.AllStatFlatInc = 10;
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
        return Sets.Aggregate("",
            (current, pair) =>
                current + $"{pair.Key} {pair.Value + (LuckySets.Contains(pair.Key) && pair.Value >= 3 ? 1 : 0)} ");
    }

    public void Add(MapleItem item)
    {
        SetType type = GetSetType(item);
        if (type == SetType.NONE)
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
            if (luckySets.Count > 0) LuckySets.Clear();
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