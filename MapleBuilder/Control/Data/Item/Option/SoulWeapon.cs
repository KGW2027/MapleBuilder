using System;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item.Option;

public static class SoulWeapon
{
    public static Dictionary<MapleStatus.StatusType, string> SoulDescriptions = new()
    {
        {MapleStatus.StatusType.STR, "STR : +%d"},
        {MapleStatus.StatusType.DEX, "DEX : +%d"},
        {MapleStatus.StatusType.INT, "INT : +%d"},
        {MapleStatus.StatusType.LUK, "LUK : +%d"},
        {MapleStatus.StatusType.ALL_STAT, "올스탯 : +%d"},
        {MapleStatus.StatusType.ALL_STAT_RATE, "올스탯 : +%d%"},
        {MapleStatus.StatusType.ATTACK_POWER, "공격력 : +%d"},
        {MapleStatus.StatusType.ATTACK_RATE, "공격력 : +%d%"},
        {MapleStatus.StatusType.MAGIC_POWER, "마력 : +%d"},
        {MapleStatus.StatusType.MAGIC_RATE, "마력 : +%d%"},
        {MapleStatus.StatusType.HP, "최대 HP : +%d"},
        {MapleStatus.StatusType.MP, "최대 MP : +%d"},
        {MapleStatus.StatusType.BOSS_DAMAGE, "보스 공격시 데미지 : +%d%"},
        {MapleStatus.StatusType.IGNORE_DEF, "몬스터 방어율 무시 : +%d%"},
        {MapleStatus.StatusType.CRITICAL_CHANCE, "크리티컬 확률 : +%d%"},
    };
    
    public enum SoulPrefix
    {
        BEEFY           = 0x01, // 기운찬 : STR
        SWIFT           = 0x02, // 날렵한 : DEX
        CLEVER          = 0x03, // 총명한 : INT
        FORTUITOUS      = 0x04, // 놀라운 : LUK
        FLASHY          = 0x05, // 화려한 : ALL_STAT
        POWERFUL        = 0x06, // 강력한 : ATTACK_POWER
        RADIANT         = 0x07, // 빛나는 : MAGIC_POWER
        HEARTY          = 0x08, // 강인한 : HP
        AMPLE           = 0x09, // 풍부한 : MP
        MAGNIFICENT     = 0x10, // 위대한 : ATTACK_POWER / MAGIC_POWER / ALL_STAT[_RATE (1 Tier)] / HP / CRIT_CHANCE / IGNORE_DEF / BOSS_DAMAGE
        UNKNOWN         = 0x00,
    }

    public static string GetSoulPrefixString(SoulPrefix prefix)
    {
        return prefix switch
        {
            SoulPrefix.BEEFY => "기운찬",
            SoulPrefix.SWIFT => "날렵한",
            SoulPrefix.CLEVER => "총명한",
            SoulPrefix.FORTUITOUS => "놀라운",
            SoulPrefix.FLASHY => "화려한",
            SoulPrefix.POWERFUL => "강력한",
            SoulPrefix.RADIANT => "빛나는",
            SoulPrefix.HEARTY => "강인한",
            SoulPrefix.AMPLE => "풍부한",
            SoulPrefix.MAGNIFICENT => "위대한",
            SoulPrefix.UNKNOWN => "알 수 없음",
            _ => throw new ArgumentOutOfRangeException(nameof(prefix), prefix, null)
        };
    }

    public static SoulPrefix[] GetSoulPrefixes(SoulType type)
    {
        switch (type)
        {
            case SoulType.NONE:
                return Array.Empty<SoulPrefix>();
            case SoulType.KALING:
            case SoulType.KALOS:
            case SoulType.DJUNKEL:
            case SoulType.VERUS_HILLA:
            case SoulType.WILL:
            case SoulType.LUCID:
            case SoulType.DAMIEN:
            case SoulType.LOTUS:
            case SoulType.MAGNUS:
            case SoulType.CYGNUS:
            case SoulType.BLOODY_QUEEN:
            case SoulType.VELLUM:
            case SoulType.MURMUR:
            case SoulType.PINKBEAN:
            case SoulType.VONBON:
            case SoulType.PIERRE:
            case SoulType.URUS:
            case SoulType.AKARUM:
            case SoulType.MOGADIN:
            case SoulType.KHALIAIN:
            case SoulType.JURAI:
            case SoulType.CQ57:
            case SoulType.FREYD:
            case SoulType.VON_LEON:
            case SoulType.HILLA:
            case SoulType.PAPULATUS:
            case SoulType.ZAKUM:
                return new[] // 풍부한 제외 9종
                {
                    SoulPrefix.BEEFY, SoulPrefix.SWIFT, SoulPrefix.CLEVER, SoulPrefix.FORTUITOUS, SoulPrefix.FLASHY,
                    SoulPrefix.POWERFUL, SoulPrefix.RADIANT, SoulPrefix.HEARTY, SoulPrefix.MAGNIFICENT
                };
            case SoulType.MUGONG:
            case SoulType.EPHENIA:
                return new[] // 풍부한, 강력한, 빛나는 제외 7종
                {
                    SoulPrefix.BEEFY, SoulPrefix.SWIFT, SoulPrefix.CLEVER, SoulPrefix.FORTUITOUS, SoulPrefix.FLASHY,
                    SoulPrefix.HEARTY, SoulPrefix.MAGNIFICENT
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static KeyValuePair<MapleStatus.StatusType, int>[] GetSoulOption(SoulType type, SoulPrefix prefix)
    {
        if (prefix == SoulPrefix.UNKNOWN) return Array.Empty<KeyValuePair<MapleStatus.StatusType, int>>();
        var tier = (((int)type & 0xF0) >> 4)+1;
        switch (prefix)
        {
            case SoulPrefix.BEEFY:
                if (type == SoulType.MOGADIN) tier--;
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.STR,
                      tier == 1 ? 24
                    : tier == 2 ? 20
                    : tier == 3 ? 18
                    : tier == 4 ? 15
                    : tier == 5 ? 12
                    : tier == 7 ? 7
                    : tier == 9 ? 4 : 0)};
            case SoulPrefix.SWIFT:
                if (type == SoulType.JURAI) tier--;
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.DEX,
                     tier == 1 ? 24
                    : tier == 2 ? 20
                    : tier == 3 ? 18
                    : tier == 4 ? 15
                    : tier == 5 ? 12
                    : tier == 7 ? 7
                    : tier == 9 ? 4 : 0)};
            case SoulPrefix.CLEVER:
                if (type == SoulType.KHALIAIN) tier--;
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.INT,
                     tier == 1 ? 24
                    : tier == 2 ? 20
                    : tier == 3 ? 18
                    : tier == 4 ? 15
                    : tier == 5 ? 12
                    : tier == 7 ? 7
                    : tier == 9 ? 4 : 0)};
            case SoulPrefix.FORTUITOUS:
                if (type == SoulType.CQ57) tier--;
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.LUK,
                     tier == 1 ? 24
                    : tier == 2 ? 20
                    : tier == 3 ? 18
                    : tier == 4 ? 15
                    : tier == 5 ? 12
                    : tier == 7 ? 7
                    : tier == 9 ? 4 : 0)};
            case SoulPrefix.FLASHY:
                if (type == SoulType.FREYD) tier--;
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.ALL_STAT,
                      tier == 1 ? 15
                    : tier == 2 ? 12
                    : tier == 3 ? 10
                    : tier == 4 ? 8
                    : tier == 5 ? 8
                    : tier == 7 ? 5
                    : tier == 9 ? 2 : 0)};
            case SoulPrefix.POWERFUL:
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.ATTACK_POWER,
                      tier == 1 ? 6
                    : tier == 2 ? 5
                    : tier == 3 ? 4
                    : tier == 4 ? 3
                    : tier == 5 ? 3 : 0)};
            case SoulPrefix.RADIANT:
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.MAGIC_POWER,
                      tier == 1 ? 6
                    : tier == 2 ? 5
                    : tier == 3 ? 4
                    : tier == 4 ? 3
                    : tier == 5 ? 3 : 0)};
            case SoulPrefix.HEARTY:
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.HP,
                      tier == 1 ? 960
                    : tier == 2 ? 800
                    : tier == 3 ? 700
                    : tier == 4 ? 600
                    : tier == 5 ? 500
                    : tier == 7 ? 300
                    : tier == 9 ? 180 : 0)};
            case SoulPrefix.AMPLE:
                return new[] {KeyValuePair.Create(MapleStatus.StatusType.MP,
                    tier == 1 ? 960
                    : tier == 2 ? 800
                    : tier == 3 ? 700
                    : tier == 4 ? 600
                    : tier == 5 ? 500
                    : tier == 7 ? 300
                    : tier == 9 ? 180 : 0)};
            case SoulPrefix.MAGNIFICENT:
                if (tier == 1)
                {
                    return new[]
                    {
                        KeyValuePair.Create(MapleStatus.StatusType.ATTACK_RATE, 3),
                        KeyValuePair.Create(MapleStatus.StatusType.MAGIC_RATE, 3),
                        KeyValuePair.Create(MapleStatus.StatusType.ALL_STAT_RATE, 5),
                        KeyValuePair.Create(MapleStatus.StatusType.HP, 2000),
                        KeyValuePair.Create(MapleStatus.StatusType.CRITICAL_CHANCE, 12),
                        KeyValuePair.Create(MapleStatus.StatusType.IGNORE_DEF, 7),
                        KeyValuePair.Create(MapleStatus.StatusType.BOSS_DAMAGE, 7),
                    };
                }
                return new[]
                {
                    KeyValuePair.Create(MapleStatus.StatusType.ATTACK_POWER, 
                          tier == 2 ? 10 :
                          type == SoulType.VON_LEON ? 7 :
                          tier == 3 || tier == 4 || type == SoulType.MOGADIN ? 8 :
                          tier == 5 ? 6 : 5),
                    KeyValuePair.Create(MapleStatus.StatusType.MAGIC_POWER, 
                        tier == 2 ? 10 :
                        type == SoulType.VON_LEON ? 7 :
                        tier == 3 || tier == 4 || type == SoulType.KHALIAIN ? 8 :
                        tier == 5 ? 6 : 5),
                    KeyValuePair.Create(MapleStatus.StatusType.ALL_STAT, 
                        tier == 2 ? 20:
                        type == SoulType.VON_LEON ? 15 :
                        tier == 3 || tier == 4 || type == SoulType.FREYD ? 17 :
                        tier == 5 ? 12 : 10),
                    KeyValuePair.Create(MapleStatus.StatusType.HP,
                        tier == 2 ? 1500 :
                        type == SoulType.VON_LEON ? 1200 :
                        tier == 3 || tier == 4 || type == SoulType.JURAI ? 1300 :
                        tier == 5 ? 1100 : 1000),
                    KeyValuePair.Create(MapleStatus.StatusType.CRITICAL_CHANCE, 
                        tier == 2 ? 10 :
                        type == SoulType.VON_LEON ? 7 :
                        tier == 3 || tier == 4 || type == SoulType.KHALIAIN ? 8 :
                        tier == 5 ? 6 : 5),
                    KeyValuePair.Create(MapleStatus.StatusType.IGNORE_DEF,
                        tier == 2 ? 7 :
                        tier == 3 || tier == 4 || type == SoulType.KHALIAIN ? 4 : 3),
                    KeyValuePair.Create(MapleStatus.StatusType.BOSS_DAMAGE,
                        tier == 2 ? 7 :
                        tier == 3 || tier == 4 || type == SoulType.KHALIAIN ? 4 : 3)
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(prefix), prefix, null);
        }
    }
    
    public enum SoulType
    {
        NONE            = 0x00,
        
        // Tier 1 - 24 / 15 / 6 / 960
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
        
        // Tier 2 - 20 / 12 / 5 / 800 
        PINKBEAN        = 0x11,
        VONBON          = 0x12,
        PIERRE          = 0x13,
        URUS            = 0x14,
        
        // Tier 3 - 18 / 10 / 4 / 700
        AKARUM          = 0x21,
        MOGADIN         = 0x22,
        KHALIAIN        = 0x23,
        JURAI           = 0x24,
        CQ57            = 0x25,
        FREYD           = 0x26,
        
        // Tier 4 - 15 / 8 / 3 / 600 
        VON_LEON        = 0x31,
        HILLA           = 0x32,
        
        // Tier 5 - 12 / 8 / 3 / 500 
        PAPULATUS       = 0x41,
        ZAKUM           = 0x42,
        
        MUGONG          = 0x61, // 무공 - 7 / 5 / X / 300
        EPHENIA         = 0x81, // 에피네아 - 4 / 2 / X / 180
    }

    public static string GetSoulTypeString(SoulType soulType)
    {
        return soulType switch
        {
            SoulType.NONE => "없음",
            SoulType.KALING => "카링",
            SoulType.KALOS => "칼로스",
            SoulType.DJUNKEL => "듄켈",
            SoulType.VERUS_HILLA => "진 힐라",
            SoulType.WILL => "윌",
            SoulType.LUCID => "루시드",
            SoulType.DAMIEN => "데미안",
            SoulType.LOTUS => "전투병기 스우",
            SoulType.MAGNUS => "매그너스",
            SoulType.CYGNUS => "시그너스",
            SoulType.BLOODY_QUEEN => "블러디 퀸",
            SoulType.VELLUM => "벨룸",
            SoulType.MURMUR => "무르무르",
            SoulType.PINKBEAN => "핑크빈",
            SoulType.VONBON => "반반",
            SoulType.PIERRE => "피에르",
            SoulType.URUS => "우르스",
            SoulType.AKARUM => "아카이럼",
            SoulType.MOGADIN => "모카딘",
            SoulType.KHALIAIN => "카리아인",
            SoulType.JURAI => "줄라이",
            SoulType.CQ57 => "CQ57",
            SoulType.FREYD => "플레드",
            SoulType.VON_LEON => "반 레온",
            SoulType.HILLA => "힐라",
            SoulType.PAPULATUS => "파풀라투스",
            SoulType.ZAKUM => "자쿰",
            SoulType.EPHENIA => "에피네아",
            SoulType.MUGONG => "무공",
            _ => throw new ArgumentOutOfRangeException(nameof(soulType), soulType, null)
        };
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
            "블러디 퀸" => SoulType.BLOODY_QUEEN,
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
            "무공" => SoulType.MUGONG,
            _ => SoulType.NONE
        };
    }

    public static SoulPrefix GetSoulPrefix(string overrideString)
    {
        return overrideString switch
        {
            "기운찬" => SoulPrefix.BEEFY,
            "날렵한" => SoulPrefix.SWIFT,
            "총명한" => SoulPrefix.CLEVER,
            "놀라운" => SoulPrefix.FORTUITOUS,
            "화려한" => SoulPrefix.FLASHY,
            "강력한" => SoulPrefix.POWERFUL,
            "빛나는" => SoulPrefix.RADIANT,
            "강인한" => SoulPrefix.HEARTY,
            "풍부한" => SoulPrefix.AMPLE,
            "위대한" => SoulPrefix.MAGNIFICENT,
            _ => SoulPrefix.UNKNOWN
        };
    }
}