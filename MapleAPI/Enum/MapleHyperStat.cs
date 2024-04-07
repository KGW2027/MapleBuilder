namespace MapleAPI.Enum;

public class MapleHyperStat
{
    public enum StatType
    {
        STR,
        DEX,
        INT,
        LUK,
        HP,
        MP,
        DF_TT_PP,
        CRITCIAL_CHANCE,
        CRITICAL_DAMAGE,
        IGNORE_ARMOR,
        DAMAGE,
        BOSS_DAMAGE,
        COMMON_DAMAGE,
        IMMUNE,
        ATTACK_POWER,
        EXP_UP,
        ARCANE_FORCE,
        UNKNOWN
    }

    public static StatType GetStatType(string name)
    {
        return name switch
        {
            "STR" => StatType.STR,
            "DEX" => StatType.DEX,
            "INT" => StatType.INT,
            "LUK" => StatType.LUK,
            "HP" => StatType.HP,
            "MP" => StatType.MP,
            "DF/TT/PP" => StatType.DF_TT_PP,
            "크리티컬 확률" => StatType.CRITCIAL_CHANCE,
            "크리티컬 데미지" => StatType.CRITICAL_DAMAGE,
            "방어율 무시" => StatType.IGNORE_ARMOR,
            "데미지" => StatType.DAMAGE,
            "보스 몬스터 공격 시 데미지 증가" => StatType.BOSS_DAMAGE,
            "상태 이상 내성" => StatType.IMMUNE,
            "공격력/마력" => StatType.ATTACK_POWER,
            "획득 경험치" => StatType.EXP_UP,
            "아케인포스" => StatType.ARCANE_FORCE,
            _ => StatType.UNKNOWN
        };
    }
}