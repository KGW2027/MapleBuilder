namespace MapleAPI.Enum;

public class MaplePotentialOption
{
    public enum OptionType
    {
        IGNORE_ARMOR,
        ATTACK_RATE,
        MAGIC_RATE,
        BOSS_DAMAGE,
        DAMAGE,
        CRITICAL_CHANCE,
        ATTACK,
        MAGIC,
        STR_RATE,
        DEX_RATE,
        INT_RATE,
        LUK_RATE,
        ALL_STAT_RATE,
        MAX_HP_RATE,
        MAX_MP_RATE,
        STR,
        DEX,
        INT,
        LUK,
        LEVEL_STR,
        LEVEL_DEX,
        LEVEL_INT,
        LEVEL_LUK,
        ALL_STAT,
        MAX_HP,
        MAX_MP,
        COOLDOWN_DECREASE,
        CRITICAL_DAMAGE,
        ITEM_DROP,
        MESO_DROP,
        OTHER
    }
    


    public static KeyValuePair<OptionType, int> ParseOptionFromPotential(string value)
    {
        OptionType option = OptionType.OTHER;
        int optionValue = int.TryParse(value.Substring(value.IndexOf('+') + 1).Replace("%", ""), out int val)
            ? val
            : -1;
        bool bIsEndsPercent = value.EndsWith("%");

        if (value.StartsWith("STR"))                   option = bIsEndsPercent ? OptionType.STR_RATE : OptionType.STR;
        else if (value.StartsWith("DEX"))              option = bIsEndsPercent ? OptionType.DEX_RATE : OptionType.DEX;
        else if (value.StartsWith("INT"))              option = bIsEndsPercent ? OptionType.INT_RATE : OptionType.INT;
        else if (value.StartsWith("LUK"))              option = bIsEndsPercent ? OptionType.LUK_RATE : OptionType.LUK;
        else if (value.StartsWith("올스탯"))            option = bIsEndsPercent ? OptionType.ALL_STAT_RATE : OptionType.ALL_STAT;
        else if (value.StartsWith("최대 HP") || value.StartsWith("HP"))           option = bIsEndsPercent ? OptionType.MAX_HP_RATE : OptionType.MAX_HP;
        else if (value.StartsWith("최대 MP") || value.StartsWith("MP"))           option = bIsEndsPercent ? OptionType.MAX_MP_RATE : OptionType.MAX_MP;
        else if (value.StartsWith("공격력"))            option = bIsEndsPercent ? OptionType.ATTACK_RATE : OptionType.ATTACK;
        else if (value.StartsWith("마력"))              option = bIsEndsPercent ? OptionType.MAGIC_RATE : OptionType.MAGIC;
        else if (value.StartsWith("데미지"))            option = OptionType.DAMAGE;
        else if (value.StartsWith("보스 몬스터"))         option = OptionType.BOSS_DAMAGE;
        else if (value.StartsWith("크리티컬 데미지"))    option = OptionType.CRITICAL_DAMAGE;
        else if (value.StartsWith("몬스터 방어율"))      option = OptionType.IGNORE_ARMOR;
        else if (value.StartsWith("아이템"))            option = OptionType.ITEM_DROP;
        else if (value.StartsWith("메소"))              option = OptionType.MESO_DROP;
        else if (value.StartsWith("캐릭터 기준"))
        {
            int colonIndex = value.IndexOf(':');
            string statName = value.Substring(colonIndex - 4, 3);
            option = statName switch
            {
                "STR" => OptionType.LEVEL_STR,
                "DEX" => OptionType.LEVEL_DEX,
                "INT" => OptionType.LEVEL_INT,
                "LUK" => OptionType.LEVEL_LUK,
                _ => OptionType.OTHER
            };
            optionValue = int.Parse(value.Substring(value.IndexOf('+') + 1, 1));
        }
        else if (value.StartsWith("모든 스킬의 재사용"))
        {
            option = OptionType.COOLDOWN_DECREASE;
            optionValue = int.Parse(value.Substring(value.IndexOf('-')+1, 1));
        }
        
        return KeyValuePair.Create(option, optionValue);
    }
}