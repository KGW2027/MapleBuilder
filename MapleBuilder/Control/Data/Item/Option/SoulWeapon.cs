namespace MapleBuilder.Control.Data.Item.Option;

public static class SoulWeapon
{
    public enum SoulType
    {
        NONE            = 0x00,
        
        // Tier 1
        KALING          = 0x01,
        KALOS           = 0x02,
        DJUNKEL         = 0x03,
        VERUS_HILLA     = 0x04,
        WILL            = 0x05,
        LUCID           = 0x06,
        DAMIEN          = 0x07,
        LOTUS           = 0x08, // 스우
        MAGNUS          = 0x09,
        CYGNUS          = 0x0A,
        BLOODY_QUEEN    = 0x0B,
        VELLUM          = 0x0C,
        MURMUR          = 0x0D, // 무르무르
        
        // Tier 2
        PINKBEAN        = 0x11,
        VONBON          = 0x12,
        PIERRE          = 0x13,
        URUS            = 0x14,
        
        // Tier 3
        AKARUM          = 0x21,
        MOGADIN         = 0x22,
        KHALIAIN        = 0x23,
        JURAI           = 0x24,
        CQ57            = 0x25,
        FREYD           = 0x26,
        
        // Tier 4
        VON_LEON        = 0x31,
        HILLA           = 0x32,
        
        // Tier 5
        PAPULATUS       = 0x41,
        ZAKUM           = 0x42,
        
        EPHENIA         = 0x51, // 에피네아
    }

    public static SoulType GetSoulType(string name)
    {
        return name switch
        {
            "카링" => SoulType.KALING,
            "칼로스" => SoulType.KALOS,
            "듄켈" => SoulType.DJUNKEL,
            "진 힐라" => SoulType.VERUS_HILLA,
            "윌" => SoulType.WILL,
            "루시드" => SoulType.LUCID,
            "데미안" => SoulType.DAMIEN,
            "스우" => SoulType.LOTUS,
            "전투병기 스우" => SoulType.LOTUS,
            "매그너스" => SoulType.MAGNUS,
            "시그너스" => SoulType.CYGNUS,
            "블러디퀸" => SoulType.BLOODY_QUEEN,
            "벨룸" => SoulType.VELLUM,
            "무르무르" => SoulType.MURMUR,
            "핑크빈" => SoulType.PINKBEAN,
            "반반" => SoulType.VONBON,
            "피에르" => SoulType.PIERRE,
            "우르스" => SoulType.URUS,
            "아카이럼" => SoulType.AKARUM,
            "모카딘" => SoulType.MOGADIN,
            "카리아인" => SoulType.KHALIAIN,
            "줄라이" => SoulType.JURAI,
            "CQ57" => SoulType.CQ57,
            "플레드" => SoulType.FREYD,
            "반레온" => SoulType.VON_LEON,
            "힐라" => SoulType.HILLA,
            "파풀라투스" => SoulType.PAPULATUS,
            "자쿰" => SoulType.ZAKUM,
            "에피네아" => SoulType.EPHENIA,
            _ => SoulType.NONE
        };
    }
    
}