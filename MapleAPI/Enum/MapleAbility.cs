﻿using System.Text.RegularExpressions;

namespace MapleAPI.Enum;

public class MapleAbility
{
    public enum AbilityType
    {
        STR,
        DEX,
        INT,
        LUK,
        HP,
        MP,
        ATTACK_POWER,
        MAGIC_POWER,
        CRITICAL_CHANCE,
        ALL_STAT,
        ATTACK_SPEED,
        LEVEL_ATTACK_POWER,
        LEVEL_MAGIC_POWER,
        MAX_HP_RATE,
        MAX_MP_RATE,
        BOSS_DAMAGE,
        COMMON_DAMAGE,
        DEBUFF_DAMAGE,
        COOLDOWN_IGNORE,
        PASSIVE_SKILL_LEVEL,
        MULTI_TARGET,
        BUFF_DURATION_INCREASE,
        ITEM_DROP,
        MESO_DROP,
        OTHER
    }

    private static readonly int[] ZeroToZero = {0, 0};
    private static readonly Dictionary<AbilityType, int[][]> MinMaxMap = new()
    {
        {AbilityType.STR,                    new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {AbilityType.DEX,                    new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {AbilityType.INT,                    new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {AbilityType.LUK,                    new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {AbilityType.HP,                     new[] { new[]{75, 150, 15}, new[]{225, 300, 15},    new[]{375, 450, 15},    new[]{525, 600, 15} }},
        {AbilityType.MP,                     new[] { new[]{75, 150, 15}, new[]{225, 300, 15},    new[]{375, 450, 15},    new[]{525, 600, 15} }},
        {AbilityType.ATTACK_POWER,           new[] { ZeroToZero,         new[]{6, 9, 3},         new[]{15, 21, 3},       new[]{27, 30, 3} }},
        {AbilityType.MAGIC_POWER,            new[] { ZeroToZero,         new[]{6, 9, 3},         new[]{15, 21, 3},       new[]{27, 30, 3} }},
        {AbilityType.MAX_HP_RATE,            new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10 },          new []{15, 20}}},
        {AbilityType.MAX_MP_RATE,            new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10 },          new []{15, 20}}},
        {AbilityType.BOSS_DAMAGE,            new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10 },          new []{15, 20}}},
        {AbilityType.LEVEL_ATTACK_POWER,     new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{10, 16, 2}}},
        {AbilityType.LEVEL_MAGIC_POWER,      new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{10, 16, 2}}},
        {AbilityType.PASSIVE_SKILL_LEVEL,    new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{1, 1}}},
        {AbilityType.MULTI_TARGET,           new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{1, 1}}},
        {AbilityType.ATTACK_SPEED,           new[] { ZeroToZero,         ZeroToZero,             ZeroToZero,             new []{1, 1}}},
        {AbilityType.COMMON_DAMAGE,          new[] { new[]{2, 3},        new[]{4, 5},            new[]{7, 8},            new[]{9, 10} }},
        {AbilityType.DEBUFF_DAMAGE,          new[] { new[]{2, 3},        new[]{4, 5},            new[]{7, 8},            new[]{9, 10} }},
        {AbilityType.ITEM_DROP,              new[] { new[]{3, 5},        new[]{8, 10},           new[]{13, 15},          new[]{18, 20} }},
        {AbilityType.MESO_DROP,              new[] { new[]{3, 5},        new[]{8, 10},           new[]{13, 15},          new[]{18, 20} }},
        {AbilityType.CRITICAL_CHANCE,        new[] { ZeroToZero,         new[]{5, 10},           new[]{15, 20},          new[]{25, 30} }},
        {AbilityType.ALL_STAT,               new[] { new[]{5, 10},       new[]{15, 20},          new[]{25, 30},          new[]{35, 40} }},
        {AbilityType.COOLDOWN_IGNORE,        new[] { ZeroToZero,         ZeroToZero,             new[]{5, 10},           new[]{15, 20} }},
        {AbilityType.BUFF_DURATION_INCREASE, new[] { new[]{7, 13},       new[]{19, 25},          new []{32, 38},         new []{44, 50} }},
    };
    
    private static readonly Dictionary<AbilityType, string> AbilityStringMap = new()
    {
        {AbilityType.STR,                    "STR %d 증가"},
        {AbilityType.DEX,                    "DEX %d 증가"},
        {AbilityType.INT,                    "INT %d 증가"},
        {AbilityType.LUK,                    "LUK %d 증가"},
        {AbilityType.HP,                     "최대 HP %d 증가"},
        {AbilityType.MP,                     "최대 MP %d 증가"},
        {AbilityType.ATTACK_POWER,           "공격력 %d 증가"},
        {AbilityType.MAGIC_POWER,            "마력 %d 증가"},
        {AbilityType.MAX_HP_RATE,            "최대 HP %d% 증가"},
        {AbilityType.MAX_MP_RATE,            "최대 MP %d% 증가"},
        {AbilityType.BOSS_DAMAGE,            "보스 몬스터 공격 시 데미지 %d% 증가"},
        {AbilityType.LEVEL_ATTACK_POWER,     "%d 레벨 당 공격력 1 증가"},
        {AbilityType.LEVEL_MAGIC_POWER,      "%d 레벨 당 마력 1 증가"},
        {AbilityType.PASSIVE_SKILL_LEVEL,    "패시브 스킬 레벨 %d 증가 (액티브 혼합형, 5차 스킬 적용 불가)"},
        {AbilityType.MULTI_TARGET,           "다수 공격 스킬의 공격 대상 %d 증가"},
        {AbilityType.ATTACK_SPEED,           "공격 속도 %d단계 증가"},
        {AbilityType.COMMON_DAMAGE,          "일반 몬스터 공격 시 데미지 %d% 증가"},
        {AbilityType.DEBUFF_DAMAGE,          "상태 이상에 걸린 대상 공격 시 데미지 %d% 증가"},
        {AbilityType.ITEM_DROP,              "아이템 드롭률 %d% 증가"},
        {AbilityType.MESO_DROP,              "메소 획득량 %d% 증가"},
        {AbilityType.CRITICAL_CHANCE,        "크리티컬 확률 %d% 증가"},
        {AbilityType.ALL_STAT,               "모든 능력치 %d 증가"},
        {AbilityType.COOLDOWN_IGNORE,        "스킬 사용 시 %d% 확률로 재사용 대기시간이 미적용"},
        {AbilityType.BUFF_DURATION_INCREASE, "버프 스킬의 지속 시간 %d% 증가"},
    };
    

    public static int[] GetMinMax(AbilityType type, MaplePotentialGrade.GradeType grade)
    {
        if (type == AbilityType.OTHER || grade == MaplePotentialGrade.GradeType.NONE) return ZeroToZero;
        if (MinMaxMap.TryGetValue(type, out var gradeMap)) return gradeMap[(int)grade-1];
        return ZeroToZero;
    }

    public static string GetAbilityString(AbilityType type)
    {
        return AbilityStringMap.GetValueOrDefault(type, "잡옵");
    }

    public static AbilityType TryParse(string value)
    {
        value = Regex.Replace(value, "[0-9]", "").Trim();
        foreach (AbilityType abType in System.Enum.GetValues(typeof(AbilityType)))
        {
            string testValue = Regex.Replace(GetAbilityString(abType).Replace("%d", ""), "[0-9]", "").Trim();
            if (testValue.Equals(value)) return abType;
        }

        return AbilityType.OTHER;
    }

    public static MaplePotentialGrade.GradeType DetectAbilityGrade(AbilityType type, int value)
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