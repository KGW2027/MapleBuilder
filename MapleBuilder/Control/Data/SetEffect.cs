using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data;

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

    private SetType GetSetType(MapleCommonItem item)
    {
        foreach (var pair in setMap.Where(pair => pair.Value.Any(prefix => item.Name.StartsWith(prefix))))
            return pair.Key;
        return SetType.NONE;
    }

    private List<SetType> GetLuckySets(MapleCommonItem item)
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

    private MapleStatContainer GetSetOption(SetType setType, int level)
    {
        MapleStatContainer option = new MapleStatContainer();
        if (LuckySets.Contains(setType) && level >= 3) level++;
        switch (setType)
        {
            case SetType.NECRO:
                switch (Math.Min(level, 5))
                {
                    case 5:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ALL_STAT] += 5;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 5;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.ALL_STAT] += 3;
                        break;
                }

                break;
            case SetType.MUSPELL:
                switch (Math.Min(level, 5))
                {
                    case 5:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 6;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 8;
                        option[MapleStatus.StatusType.MAIN_STAT] += 8;
                        option[MapleStatus.StatusType.COMMON_DAMAGE] += 4;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 8;
                        option[MapleStatus.StatusType.COMMON_DAMAGE] += 2;
                        goto case 2;
                    case 2:
                        break;
                }

                break;
            case SetType.ROYAL:
                switch (Math.Min(level, 6))
                {
                    case 6:
                        option[MapleStatus.StatusType.ALL_STAT] += 25;
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 15;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 20;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 9;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ALL_STAT] += 6;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        break;
                }

                break;
            case SetType.PENSALIR:
                switch (Math.Min(level, 6))
                {
                    case 6:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        option[MapleStatus.StatusType.COMMON_DAMAGE] += 8;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        option[MapleStatus.StatusType.COMMON_DAMAGE] += 6;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 9;
                        option[MapleStatus.StatusType.ALL_STAT] += 9;
                        option[MapleStatus.StatusType.COMMON_DAMAGE] += 4;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 9;
                        option[MapleStatus.StatusType.COMMON_DAMAGE] += 2;
                        break;
                }

                break;
            case SetType.MAISTER:
                switch (Math.Min(level, 4))
                {
                    case 4:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 20;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 10;
                        break;
                }

                break;
            case SetType.CYGNUS:
                switch (Math.Min(level, 7))
                {
                    case 7:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 15;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        goto case 6;
                    case 6:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 30;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 30;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.ALL_STAT] += 20;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 15;
                        goto case 2;
                    case 2:
                        break;
                }

                break;
            case SetType.ROOTABYSS:
                switch (Math.Min(level, 4))
                {
                    case 4:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 30;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 50;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.ALL_STAT] += 20;
                        option[MapleStatus.StatusType.HP_AND_MP] += 1000;
                        break;
                }

                break;
            case SetType.ABSOLABSE:
                switch (Math.Min(level, 7))
                {
                    case 7:
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 20;
                        goto case 6;
                    case 6:
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 20;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 20;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 30;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 25;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.ALL_STAT] += 30;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 20;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.HP_AND_MP] += 1500;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 20;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        break;
                }

                break;
            case SetType.ARCANESHADE:
                switch (Math.Min(level, 7))
                {
                    case 7:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 30;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        goto case 6;
                    case 6:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 30;
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 30;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        option[MapleStatus.StatusType.HP_AND_MP] += 2000;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 35;
                        option[MapleStatus.StatusType.ALL_STAT] += 50;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 30;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 30;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        break;
                }

                break;
            case SetType.ETERNEL:
                switch (Math.Min(level, 8))
                {
                    case 8:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        goto case 7;
                    case 7:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        option[MapleStatus.StatusType.ALL_STAT] += 50;
                        option[MapleStatus.StatusType.HP_AND_MP] += 2500;
                        goto case 6;
                    case 6:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 30;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 15;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 20;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 45;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 15;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.ALL_STAT] += 50;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 40;
                        option[MapleStatus.StatusType.BOSS_DAMAGE]+= 10;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.HP_AND_MP] += 2500;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC]  += 40;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        break;
                }

                break;
            case SetType.MONSTERPARK:
                switch (Math.Min(level, 2))
                {
                    case 2:
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        break;
                }

                break;
            case SetType.BOSSACCESSORY:
                switch (Math.Min(level, 9))
                {
                    case 9:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        goto case 7;
                    case 8:
                    case 7:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        goto case 5;
                    case 6:
                    case 5:
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 5;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 5;
                        goto case 3;
                    case 4:
                    case 3:
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP_AND_MP_RATE] += 5;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 5;
                        break;
                }

                break;
            case SetType.DAWN:
                switch (Math.Min(level, 4))
                {
                    case 4:
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP] += 250;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP] += 250;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP] += 250;
                        break;
                }

                break;
            case SetType.BLACK:
                switch (Math.Min(level, 9))
                {
                    case 9:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.HP] += 375;
                        option[MapleStatus.StatusType.CRITICAL_DAMAGE] += 5;
                        goto case 8;
                    case 8:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.HP] += 375;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 7;
                    case 7:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.HP] += 375;
                        option[MapleStatus.StatusType.CRITICAL_DAMAGE] += 5;
                        goto case 6;
                    case 6:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.HP] += 375;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        goto case 5;
                    case 5:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.HP] += 375;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                        goto case 4;
                    case 4:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 15;
                        option[MapleStatus.StatusType.ALL_STAT] += 15;
                        option[MapleStatus.StatusType.HP] += 375;
                        option[MapleStatus.StatusType.CRITICAL_DAMAGE] += 5;
                        goto case 3;
                    case 3:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP] += 250;
                        option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                        goto case 2;
                    case 2:
                        option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                        option[MapleStatus.StatusType.ALL_STAT] += 10;
                        option[MapleStatus.StatusType.HP] += 250;
                        option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
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

    public MapleStatContainer GetSetOptions()
    {
        MapleStatContainer result = new MapleStatContainer();
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

    public void Add(MapleCommonItem item)
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

    public void Sub(MapleCommonItem item)
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