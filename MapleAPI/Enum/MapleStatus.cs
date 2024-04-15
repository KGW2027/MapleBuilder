namespace MapleAPI.Enum;

public class MapleStatus
{
    public enum StatusType
    {
        OTHER           = 0,
        
        STR             = 0x01,
        DEX             = 0x02,
        INT             = 0x03,
        LUK             = 0x04,
        ALL_STAT        = 0x05,
        ATTACK_POWER    = 0x06,
        MAGIC_POWER     = 0x07,
        HP              = 0x08,
        MP              = 0x09,
        DF_TT_PP        = 0x0A,
        
        STAT_RATE       = 0x10,
        STR_RATE        = 0x11,
        DEX_RATE        = 0x12,
        INT_RATE        = 0x13,
        LUK_RATE        = 0x14,
        ALL_STAT_RATE   = 0x15,
        ATTACK_RATE     = 0x16,
        MAGIC_RATE      = 0x17,
        HP_RATE         = 0x18,
        MP_RATE         = 0x19,
        
        STAT_FLAT       = 0x20,
        STR_FLAT        = 0x21,
        DEX_FLAT        = 0x22,
        INT_FLAT        = 0x23,
        LUK_FLAT        = 0x24,
        
        STR_PER_LEVEL   = 0x31,
        DEX_PER_LEVEL   = 0x32,
        INT_PER_LEVEL   = 0x33,
        LUK_PER_LEVEL   = 0x34,
        ATK_PER_LEVEL   = 0x36,
        MAG_PER_LEVEL   = 0x37,
            
        CRITICAL_CHANCE = 0x41,
        CRITICAL_DAMAGE = 0x42,
        DAMAGE          = 0x43,
        BOSS_DAMAGE     = 0x44,
        COMMON_DAMAGE   = 0x45,
        DEBUFF_DAMAGE   = 0x46,
        IGNORE_DEF      = 0x47,
        WILD_HUNTER_DMG = 0x48,
        
        ABN_STATUS_RESIS= 0x51, // Abnormal Status Resistance : 상태 이상 내성
        IGNORE_IMMUNE   = 0x52,
        COOL_DEC_SECOND = 0x53,
        COOL_DEC_RATE   = 0x54,
        COOL_IGNORE     = 0x55,
        BUFF_DURATION   = 0x56,
        SUMMON_DURATION = 0x57,
        FINAL_ATTACK    = 0x58,
        ATTACK_SPEED    = 0x59,
        PASSIVE_LEVEL   = 0x5A,
        TARGET_COUNT    = 0x5B,
        
        ITEM_DROP       = 0x61,
        MESO_DROP       = 0x62,
        EXP_INCREASE    = 0x63,
        ARCANE_FORCE    = 0x64,
        AUTHENTIC_FORCE = 0x65,
        
        HP_AND_MP       = 0x71,
        HP_AND_MP_RATE  = 0x72,
        ATTACK_AND_MAGIC= 0x73,
        STR_DEX_LUK     = 0x74,
        MAIN_STAT       = 0x75,
        MAIN_STAT_FLAT  = 0x76,
        SUB_STAT        = 0x77,
        FINAL_DAMAGE    = 0x78,
        FINAL_DAMAGE_SUM= 0x79,
        MAIN_ATTACK     = 0x7A,
        
        
    }
}