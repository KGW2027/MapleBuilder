namespace MapleAPI.Enum;

public class MaplePotentialOption
{
    // public enum OptionType
    // {
    //     IGNORE_ARMOR,
    //     ATTACK_RATE,
    //     MAGIC_RATE,
    //     BOSS_DAMAGE,
    //     DAMAGE,
    //     CRITICAL_CHANCE,
    //     ATTACK,
    //     MAGIC,
    //     STR_RATE,
    //     DEX_RATE,
    //     INT_RATE,
    //     LUK_RATE,
    //     ALL_STAT_RATE,
    //     MAX_HP_RATE,
    //     MAX_MP_RATE,
    //     STR,
    //     DEX,
    //     INT,
    //     LUK,
    //     LEVEL_STR,
    //     LEVEL_DEX,
    //     LEVEL_INT,
    //     LEVEL_LUK,
    //     ALL_STAT,
    //     MAX_HP,
    //     MAX_MP,
    //     COOLDOWN_DECREASE,
    //     CRITICAL_DAMAGE,
    //     ITEM_DROP,
    //     MESO_DROP,
    //     OTHER
    // }

    public static KeyValuePair<MapleStatus.StatusType, int> ParseOptionFromPotential(string value)
    {
        MapleStatus.StatusType option = MapleStatus.StatusType.OTHER;
        int optionValue = int.TryParse(value.Substring(value.IndexOf('+') + 1).Replace("%", ""), out int val)
            ? val
            : -1;
        bool bIsEndsPercent = value.EndsWith("%");

        if (value.StartsWith("STR"))                   option = bIsEndsPercent ? MapleStatus.StatusType.STR_RATE : MapleStatus.StatusType.STR;
        else if (value.StartsWith("DEX"))              option = bIsEndsPercent ? MapleStatus.StatusType.DEX_RATE : MapleStatus.StatusType.DEX;
        else if (value.StartsWith("INT"))              option = bIsEndsPercent ? MapleStatus.StatusType.INT_RATE : MapleStatus.StatusType.INT;
        else if (value.StartsWith("LUK"))              option = bIsEndsPercent ? MapleStatus.StatusType.LUK_RATE : MapleStatus.StatusType.LUK;
        else if (value.StartsWith("올스탯"))            option = bIsEndsPercent ? MapleStatus.StatusType.ALL_STAT_RATE : MapleStatus.StatusType.ALL_STAT;
        else if (value.StartsWith("최대 HP") || value.StartsWith("HP"))           option = bIsEndsPercent ? MapleStatus.StatusType.HP_RATE : MapleStatus.StatusType.HP;
        else if (value.StartsWith("최대 MP") || value.StartsWith("MP"))           option = bIsEndsPercent ? MapleStatus.StatusType.MP_RATE : MapleStatus.StatusType.MP;
        else if (value.StartsWith("공격력"))            option = bIsEndsPercent ? MapleStatus.StatusType.ATTACK_RATE : MapleStatus.StatusType.ATTACK_POWER;
        else if (value.StartsWith("마력"))              option = bIsEndsPercent ? MapleStatus.StatusType.MAGIC_RATE : MapleStatus.StatusType.MAGIC_POWER;
        else if (value.StartsWith("데미지"))            option = MapleStatus.StatusType.DAMAGE;
        else if (value.StartsWith("보스 몬스터"))         option = MapleStatus.StatusType.BOSS_DAMAGE;
        else if (value.StartsWith("크리티컬 데미지"))    option = MapleStatus.StatusType.CRITICAL_DAMAGE;
        else if (value.StartsWith("몬스터 방어율"))      option = MapleStatus.StatusType.IGNORE_DEF;
        else if (value.StartsWith("아이템"))            option = MapleStatus.StatusType.ITEM_DROP;
        else if (value.StartsWith("메소"))              option = MapleStatus.StatusType.MESO_DROP;
        else if (value.StartsWith("캐릭터 기준"))
        {
            int colonIndex = value.IndexOf(':');
            string statName = value.Substring(colonIndex - 4, 3).Trim();
            option = statName switch
            {
                "STR" => MapleStatus.StatusType.STR_PER_LEVEL,
                "DEX" => MapleStatus.StatusType.DEX_PER_LEVEL,
                "INT" => MapleStatus.StatusType.INT_PER_LEVEL,
                "LUK" => MapleStatus.StatusType.LUK_PER_LEVEL,
                "공격력" => MapleStatus.StatusType.ATK_PER_LEVEL,
                "마력" => MapleStatus.StatusType.MAG_PER_LEVEL,
                _ => MapleStatus.StatusType.OTHER
            };
            optionValue = int.Parse(value.Substring(value.IndexOf('+') + 1, 1));
        }
        else if (value.StartsWith("모든 스킬의 재사용"))
        {
            option = MapleStatus.StatusType.COOL_DEC_SECOND;
            optionValue = int.Parse(value.Substring(value.IndexOf('-')+1, 1));
        }
        
        return KeyValuePair.Create(option, optionValue);
    }
}