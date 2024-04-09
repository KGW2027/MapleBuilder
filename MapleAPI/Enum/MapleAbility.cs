using System.Text.RegularExpressions;

namespace MapleAPI.Enum;

public class MapleAbility
{
    private static readonly int[] ZeroToZero = {0, 0};
    private static readonly Dictionary<MapleStatus.StatusType, int[][]> MinMaxMap = new()
    {
        {MapleStatus.StatusType.STR_FLAT,               new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {MapleStatus.StatusType.DEX_FLAT,               new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {MapleStatus.StatusType.INT_FLAT,               new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {MapleStatus.StatusType.LUK_FLAT,               new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {MapleStatus.StatusType.HP,                     new[] { new[]{75, 150, 15}, new[]{225, 300, 15},    new[]{375, 450, 15},    new[]{525, 600, 15} }},
        {MapleStatus.StatusType.MP,                     new[] { new[]{75, 150, 15}, new[]{225, 300, 15},    new[]{375, 450, 15},    new[]{525, 600, 15} }},
        {MapleStatus.StatusType.ATTACK_POWER,           new[] { ZeroToZero,         new[]{6, 9, 3},         new[]{15, 21, 3},       new[]{27, 30, 3} }},
        {MapleStatus.StatusType.MAGIC_POWER,            new[] { ZeroToZero,         new[]{6, 9, 3},         new[]{15, 21, 3},       new[]{27, 30, 3} }},
        {MapleStatus.StatusType.HP_RATE,                new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10 },          new []{15, 20}}},
        {MapleStatus.StatusType.MP_RATE,                new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10 },          new []{15, 20}}},
        {MapleStatus.StatusType.BOSS_DAMAGE,            new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10 },          new []{15, 20}}},
        {MapleStatus.StatusType.ATK_PER_LEVEL,          new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{10, 16, 2}}},
        {MapleStatus.StatusType.MAG_PER_LEVEL,          new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{10, 16, 2}}},
        {MapleStatus.StatusType.PASSIVE_LEVEL,          new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{1, 1}}},
        {MapleStatus.StatusType.TARGET_COUNT,           new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{1, 1}}},
        {MapleStatus.StatusType.ATTACK_SPEED,           new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{1, 1}}},
        {MapleStatus.StatusType.COMMON_DAMAGE,          new[] { new[]{2, 3},        new[]{4, 5},            new[]{7, 8},            new[]{9, 10} }},
        {MapleStatus.StatusType.DEBUFF_DAMAGE,          new[] { new[]{2, 3},        new[]{4, 5},            new[]{7, 8},            new[]{9, 10} }},
        {MapleStatus.StatusType.ITEM_DROP,              new[] { new[]{3, 5},        new[]{8, 10},           new[]{13, 15},          new[]{18, 20} }},
        {MapleStatus.StatusType.MESO_DROP,              new[] { new[]{3, 5},        new[]{8, 10},           new[]{13, 15},          new[]{18, 20} }},
        {MapleStatus.StatusType.CRITICAL_CHANCE,        new[] { ZeroToZero,         new[]{5, 10},           new[]{15, 20},          new[]{25, 30} }},
        {MapleStatus.StatusType.ALL_STAT,               new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {MapleStatus.StatusType.COOL_IGNORE,            new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10},           new[]{15, 20} }},
        {MapleStatus.StatusType.BUFF_DURATION,          new[] { new[]{7, 13},       new[]{19, 25},          new []{32, 38},         new []{44, 50} }},
    };
    
    private static readonly Dictionary<MapleStatus.StatusType, string> AbilityStringMap = new()
    {
        {MapleStatus.StatusType.STR_FLAT,           "STR %d 증가"},
        {MapleStatus.StatusType.DEX_FLAT,           "DEX %d 증가"},
        {MapleStatus.StatusType.INT_FLAT,           "INT %d 증가"},
        {MapleStatus.StatusType.LUK_FLAT,           "LUK %d 증가"},
        {MapleStatus.StatusType.HP,                 "최대 HP %d 증가"},
        {MapleStatus.StatusType.MP,                 "최대 MP %d 증가"},
        {MapleStatus.StatusType.ATTACK_POWER,       "공격력 %d 증가"},
        {MapleStatus.StatusType.MAGIC_POWER,        "마력 %d 증가"},
        {MapleStatus.StatusType.HP_RATE,            "최대 HP %d% 증가"},
        {MapleStatus.StatusType.MP_RATE,            "최대 MP %d% 증가"},
        {MapleStatus.StatusType.BOSS_DAMAGE,        "보스 몬스터 공격 시 데미지 %d% 증가"},
        {MapleStatus.StatusType.ATK_PER_LEVEL,      "%d 레벨 당 공격력 1 증가"},
        {MapleStatus.StatusType.MAG_PER_LEVEL,      "%d 레벨 당 마력 1 증가"},
        {MapleStatus.StatusType.PASSIVE_LEVEL,      "패시브 스킬 레벨 %d 증가 (액티브 혼합형, 5차 스킬 적용 불가)"},
        {MapleStatus.StatusType.TARGET_COUNT,       "다수 공격 스킬의 공격 대상 %d 증가"},
        {MapleStatus.StatusType.ATTACK_SPEED,       "공격 속도 %d단계 증가"},
        {MapleStatus.StatusType.COMMON_DAMAGE,      "일반 몬스터 공격 시 데미지 %d% 증가"},
        {MapleStatus.StatusType.DEBUFF_DAMAGE,      "상태 이상에 걸린 대상 공격 시 데미지 %d% 증가"},
        {MapleStatus.StatusType.ITEM_DROP,          "아이템 드롭률 %d% 증가"},
        {MapleStatus.StatusType.MESO_DROP,          "메소 획득량 %d% 증가"},
        {MapleStatus.StatusType.CRITICAL_CHANCE,    "크리티컬 확률 %d% 증가"},
        {MapleStatus.StatusType.ALL_STAT,           "모든 능력치 %d 증가"},
        {MapleStatus.StatusType.COOL_IGNORE,        "스킬 사용 시 %d% 확률로 재사용 대기시간이 미적용"},
        {MapleStatus.StatusType.BUFF_DURATION,      "버프 스킬의 지속 시간 %d% 증가"},
    };
    

    public static int[] GetMinMax(MapleStatus.StatusType type, MaplePotentialGrade.GradeType grade)
    {
        if (type == MapleStatus.StatusType.OTHER || grade == MaplePotentialGrade.GradeType.NONE) return ZeroToZero;
        if (MinMaxMap.TryGetValue(type, out var gradeMap)) return gradeMap[(int)grade-1];
        return ZeroToZero;
    }

    public static string GetAbilityString(MapleStatus.StatusType type)
    {
        return AbilityStringMap.GetValueOrDefault(type, "잡옵");
    }

    public static MapleStatus.StatusType TryParse(string value)
    {
        value = Regex.Replace(value, "[0-9]", "").Trim();
        foreach (MapleStatus.StatusType abType in System.Enum.GetValues(typeof(MapleStatus.StatusType)))
        {
            string testValue = Regex.Replace(GetAbilityString(abType).Replace("%d", ""), "[0-9]", "").Trim();
            if (testValue.Equals(value)) return abType;
        }

        return MapleStatus.StatusType.OTHER;
    }

    public static MaplePotentialGrade.GradeType DetectAbilityGrade(MapleStatus.StatusType type, int value)
    {
        foreach (MaplePotentialGrade.GradeType grade in System.Enum.GetValues(typeof(MaplePotentialGrade.GradeType)))
        {
            if (!MinMaxMap.ContainsKey(type) || grade == MaplePotentialGrade.GradeType.NONE) continue;
            int[] range = MinMaxMap[type][(int) grade - 1];
            if (range[0] <= value && value <= range[1])
                return grade;
        }

        return MaplePotentialGrade.GradeType.NONE;
    }
    
}