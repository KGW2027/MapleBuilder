using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MaplePotentialOption
{
    public KeyValuePair<MaplePotentialOptionType, int>[] Potentials { get; private set; }
    public KeyValuePair<MaplePotentialOptionType, int>[] Additionals { get; private set; }
    public MaplePotentialOptionGrade TopGrade { get; private set; }
    public MaplePotentialOptionGrade BottomGrade { get; private set; }
    private int level;
    private MapleEquipType equipType;
    
    public MaplePotentialOption(int itemLevel, MapleEquipType equipType, MaplePotentialOptionGrade potentialGrade = MaplePotentialOptionGrade.NONE, MaplePotentialOptionGrade additionalGrade = MaplePotentialOptionGrade.NONE)
    {
        TopGrade = potentialGrade;
        BottomGrade = potentialGrade;
        level = itemLevel;
        this.equipType = equipType;
        Potentials = new[]
        {
            KeyValuePair.Create(MaplePotentialOptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOptionType.OTHER, -1)
        };
        Additionals = new[]
        {
            KeyValuePair.Create(MaplePotentialOptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOptionType.OTHER, -1)
        };
    }

    public void SetPotential(int slot, KeyValuePair<MaplePotentialOptionType, int> value)
    {
        if (slot is < 0 or >= 3) return;
        Potentials[slot] = value;
    }
    
    public void SetAdditional(int slot, KeyValuePair<MaplePotentialOptionType, int> value)
    {
        if (slot is < 0 or >= 3) return;
        Additionals[slot] = value;
    }


    public static KeyValuePair<MaplePotentialOptionType, int> ParseOption(string value)
    {
        MaplePotentialOptionType optionType = MaplePotentialOptionType.OTHER;
        int optionValue = int.TryParse(value.Substring(value.IndexOf('+') + 1).Replace("%", ""), out int val)
            ? val
            : -1;
        bool bIsEndsPercent = value.EndsWith("%");

        if (value.StartsWith("STR"))                   optionType = bIsEndsPercent ? MaplePotentialOptionType.STR_RATE : MaplePotentialOptionType.STR;
        else if (value.StartsWith("DEX"))              optionType = bIsEndsPercent ? MaplePotentialOptionType.DEX_RATE : MaplePotentialOptionType.DEX;
        else if (value.StartsWith("INT"))              optionType = bIsEndsPercent ? MaplePotentialOptionType.INT_RATE : MaplePotentialOptionType.INT;
        else if (value.StartsWith("LUK"))              optionType = bIsEndsPercent ? MaplePotentialOptionType.LUK_RATE : MaplePotentialOptionType.LUK;
        else if (value.StartsWith("올스탯"))            optionType = bIsEndsPercent ? MaplePotentialOptionType.ALL_STAT_RATE : MaplePotentialOptionType.ALL_STAT;
        else if (value.StartsWith("최대 HP") || value.StartsWith("HP"))           optionType = bIsEndsPercent ? MaplePotentialOptionType.MAX_HP_RATE : MaplePotentialOptionType.MAX_HP;
        else if (value.StartsWith("최대 MP") || value.StartsWith("MP"))           optionType = bIsEndsPercent ? MaplePotentialOptionType.MAX_MP_RATE : MaplePotentialOptionType.MAX_MP;
        else if (value.StartsWith("공격력"))            optionType = bIsEndsPercent ? MaplePotentialOptionType.ATTACK_RATE : MaplePotentialOptionType.ATTACK;
        else if (value.StartsWith("마력"))              optionType = bIsEndsPercent ? MaplePotentialOptionType.MAGIC_RATE : MaplePotentialOptionType.MAGIC;
        else if (value.StartsWith("데미지"))            optionType = MaplePotentialOptionType.DAMAGE;
        else if (value.StartsWith("보스 몬스터"))         optionType = MaplePotentialOptionType.BOSS_DAMAGE;
        else if (value.StartsWith("크리티컬 데미지"))    optionType = MaplePotentialOptionType.CRITICAL_DAMAGE;
        else if (value.StartsWith("몬스터 방어율"))      optionType = MaplePotentialOptionType.IGNORE_ARMOR;
        else if (value.StartsWith("아이템"))            optionType = MaplePotentialOptionType.ITEM_DROP;
        else if (value.StartsWith("메소"))              optionType = MaplePotentialOptionType.MESO_DROP;
        else if (value.StartsWith("캐릭터 기준"))
        {
            int colonIndex = value.IndexOf(':');
            string statName = value.Substring(colonIndex - 4, 3);
            optionType = statName switch
            {
                "STR" => MaplePotentialOptionType.LEVEL_STR,
                "DEX" => MaplePotentialOptionType.LEVEL_DEX,
                "INT" => MaplePotentialOptionType.LEVEL_INT,
                "LUK" => MaplePotentialOptionType.LEVEL_LUK,
                _ => MaplePotentialOptionType.OTHER
            };
            optionValue = int.Parse(value.Substring(value.IndexOf('+') + 1, 1));
        }
        else if (value.StartsWith("모든 스킬의 재사용"))
        {
            optionType = MaplePotentialOptionType.COOLDOWN_DECREASE;
            optionValue = int.Parse(value.Substring(value.IndexOf('-')+1, 1));
        }
        
        return KeyValuePair.Create(optionType, optionValue);
    }
    
    
}