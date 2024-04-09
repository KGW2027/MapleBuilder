namespace MapleAPI.Enum;

public class MapleHyperStat
{
    // public enum StatType
    // {
    //     STR,
    //     DEX,
    //     INT,
    //     LUK,
    //     HP,
    //     MP,
    //     DF_TT_PP,
    //     CRITCIAL_CHANCE,
    //     CRITICAL_DAMAGE,
    //     IGNORE_ARMOR,
    //     DAMAGE,
    //     BOSS_DAMAGE,
    //     COMMON_DAMAGE,
    //     IMMUNE,
    //     ATTACK_POWER,
    //     EXP_UP,
    //     ARCANE_FORCE,
    //     UNKNOWN
    // }

    public static MapleStatus.StatusType GetStatType(string name)
    {
        return name switch
        {
            "STR" => MapleStatus.StatusType.STR_FLAT,
            "DEX" => MapleStatus.StatusType.DEX_FLAT,
            "INT" => MapleStatus.StatusType.INT_FLAT,
            "LUK" => MapleStatus.StatusType.LUK_FLAT,
            "HP" => MapleStatus.StatusType.HP,
            "MP" => MapleStatus.StatusType.MP,
            "DF/TT/PP" => MapleStatus.StatusType.DF_TT_PP,
            "크리티컬 확률" => MapleStatus.StatusType.CRITICAL_CHANCE,
            "크리티컬 데미지" => MapleStatus.StatusType.CRITICAL_DAMAGE,
            "방어율 무시" => MapleStatus.StatusType.IGNORE_ARMOR,
            "데미지" => MapleStatus.StatusType.DAMAGE,
            "보스 몬스터 공격 시 데미지 증가" => MapleStatus.StatusType.BOSS_DAMAGE,
            "상태 이상 내성" => MapleStatus.StatusType.TOLERANCE,
            "공격력/마력" => MapleStatus.StatusType.ATTACK_POWER,
            "획득 경험치" => MapleStatus.StatusType.EXP_INCREASE,
            "아케인포스" => MapleStatus.StatusType.ARCANE_FORCE,
            _ => MapleStatus.StatusType.OTHER
        };
    }
}