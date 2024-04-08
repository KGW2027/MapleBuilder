using MapleAPI.DataType;

namespace MapleAPI.Enum;

public class MapleUnion
{
    public struct UnionBlock
    {
        public MapleClass.ClassType classType;
        public RaiderRank raiderRank;
        public sbyte[][] blockPositions;

        public UnionBlock()
        {
            classType = MapleClass.ClassType.NONE;
            raiderRank = RaiderRank.NONE;
            blockPositions = Array.Empty<sbyte[]>();
        }
    }
    
    public enum RaiderEffectType
    {
        INT,
        STR,
        LUK,
        DEX,
        MAX_HP,
        BOSS_DAMAGE,
        MAX_HP_RATE,
        IGNORE_ARMOR,
        IMMUNE,
        STR_DEX_LUK,
        MAX_MP_RATE,
        ATTACK_MAGIC_POWER,
        MESO_DROP,
        EXP_UP,
        CRIT_CHANCE,
        BUFF_DURATION,
        SUMMON_DURATION,
        CRIT_DAMAGE,
        CHANCE_DAMAGE,
        COOLDOWN_DECREASE_RATE,
        OTHER
    }

    public static string GetRaiderEffectString(RaiderEffectType effectType)
    {
        return effectType switch
        {
            RaiderEffectType.INT => "INT %d 증가",
            RaiderEffectType.STR => "STR %d 증가",
            RaiderEffectType.LUK => "LUK %d 증가",
            RaiderEffectType.DEX => "DEX %d 증가",
            RaiderEffectType.MAX_HP => "최대 HP %d 증가",
            RaiderEffectType.BOSS_DAMAGE => "보스 공격 시 데미지 %d 증가",
            RaiderEffectType.MAX_HP_RATE => "최대 HP %d% 증가",
            RaiderEffectType.IGNORE_ARMOR => "방어율 무시 %d% 증가",
            RaiderEffectType.IMMUNE => "상태 이상 내성 %d 증가",
            RaiderEffectType.STR_DEX_LUK => "STR, DEX, LUK %d 증가",
            RaiderEffectType.MAX_MP_RATE => "최대 MP %d% 증가",
            RaiderEffectType.ATTACK_MAGIC_POWER => "공격력/마력 %d 증가",
            RaiderEffectType.MESO_DROP => "메소 획득량 %d 증가",
            RaiderEffectType.EXP_UP => "경험치 획득량 %d 증가",
            RaiderEffectType.CRIT_CHANCE => "크리티컬 확률 %d% 증가",
            RaiderEffectType.BUFF_DURATION => "버프 지속 시간 %d% 증가",
            RaiderEffectType.SUMMON_DURATION => "소환수 지속 시간 %d% 증가",
            RaiderEffectType.CRIT_DAMAGE => "크리티컬 데미지 %d% 증가",
            RaiderEffectType.CHANCE_DAMAGE => "공격 시 20% 확률로 데미지 %d% 증가",
            RaiderEffectType.COOLDOWN_DECREASE_RATE => "스킬 재사용 대기시간 %d%감소 (1초 미만으로 줄어들지 않음)",
            RaiderEffectType.OTHER => "효과 없음",
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };
    }

    public static int GetRaiderEffectValue(RaiderEffectType effectType, RaiderRank rank)
    {
        switch (effectType)
        {
            case RaiderEffectType.INT:
            case RaiderEffectType.STR:
            case RaiderEffectType.LUK:
            case RaiderEffectType.DEX:
                return rank switch
                {
                    RaiderRank.SSS => 100,
                    RaiderRank.SS => 80,
                    RaiderRank.S => 40,
                    RaiderRank.A => 20,
                    RaiderRank.B => 10,
                    _ => 0
                };
            case RaiderEffectType.STR_DEX_LUK:
                return rank switch
                {
                    RaiderRank.SSS => 50,
                    RaiderRank.SS => 40,
                    RaiderRank.S => 20,
                    RaiderRank.A => 10,
                    RaiderRank.B => 5,
                    _ => 0
                };
            case RaiderEffectType.MAX_HP:
                return rank switch
                {
                    RaiderRank.SSS => 2500,
                    RaiderRank.SS => 2000,
                    RaiderRank.S => 1000,
                    RaiderRank.A => 500,
                    RaiderRank.B => 250,
                    _ => 0
                };
            case RaiderEffectType.MAX_HP_RATE:
            case RaiderEffectType.MAX_MP_RATE:
            case RaiderEffectType.COOLDOWN_DECREASE_RATE:
                return rank switch
                {
                    RaiderRank.SSS => 6,
                    RaiderRank.SS => 5,
                    RaiderRank.S => 4,
                    RaiderRank.A => 3,
                    RaiderRank.B => 2,
                    _ => 0
                };
            case RaiderEffectType.IGNORE_ARMOR:
            case RaiderEffectType.BOSS_DAMAGE:
            case RaiderEffectType.CRIT_DAMAGE:
                return rank switch
                {
                    RaiderRank.SSS => 6,
                    RaiderRank.SS => 5,
                    RaiderRank.S => 3,
                    RaiderRank.A => 2,
                    RaiderRank.B => 1,
                    _ => 0
                };
            case RaiderEffectType.IMMUNE:
            case RaiderEffectType.CRIT_CHANCE:
            case RaiderEffectType.MESO_DROP:
                return rank switch
                {
                    RaiderRank.SSS => 5,
                    RaiderRank.SS => 4,
                    RaiderRank.S => 3,
                    RaiderRank.A => 2,
                    RaiderRank.B => 1,
                    _ => 0
                };
            case RaiderEffectType.EXP_UP:
            case RaiderEffectType.SUMMON_DURATION:
                return rank switch
                {
                    RaiderRank.SSS => 12,
                    RaiderRank.SS => 10,
                    RaiderRank.S => 8,
                    RaiderRank.A => 6,
                    RaiderRank.B => 4,
                    _ => 0
                };
            case RaiderEffectType.ATTACK_MAGIC_POWER:
                if (rank == RaiderRank.SSS) rank = RaiderRank.SS;
                goto case RaiderEffectType.BUFF_DURATION;
            case RaiderEffectType.BUFF_DURATION:
                return rank switch
                {
                    RaiderRank.SSS => 25,
                    RaiderRank.SS => 20,
                    RaiderRank.S => 15,
                    RaiderRank.A => 10,
                    RaiderRank.B => 5,
                    _ => 0
                };
            case RaiderEffectType.CHANCE_DAMAGE:
                return rank switch
                {
                    RaiderRank.SSS => 20,
                    RaiderRank.SS => 16,
                    RaiderRank.S => 12,
                    RaiderRank.A => 8,
                    RaiderRank.B => 4,
                    _ => 0
                };
            case RaiderEffectType.OTHER:
            default:
                return 0;
        }
    }

    public static RaiderEffectType GetRaiderEffectByClass(MapleClass.ClassType classType)
    {
        switch (classType)
        {
            case MapleClass.ClassType.HERO:
            case MapleClass.ClassType.PALADIN:
            case MapleClass.ClassType.VAIPER:
            case MapleClass.ClassType.CANONSHOOTER:
            case MapleClass.ClassType.STRIKER:
            case MapleClass.ClassType.KAISER:
            case MapleClass.ClassType.ARC:
            case MapleClass.ClassType.ADEL:
                return RaiderEffectType.STR; // 10 20 40 80 100
            
            case MapleClass.ClassType.BOW_MASTER:
            case MapleClass.ClassType.PATH_FINDER:
            case MapleClass.ClassType.WIND_BREAKER:
            case MapleClass.ClassType.ANGELICBUSTER:
            case MapleClass.ClassType.KAIN:
                return RaiderEffectType.DEX;
            
            case MapleClass.ClassType.ARCMAGE_TC:
            case MapleClass.ClassType.BISHOP:
            case MapleClass.ClassType.FLAME_WIZARD:
            case MapleClass.ClassType.BATTLE_MAGE:
            case MapleClass.ClassType.LUMINUS:
            case MapleClass.ClassType.ILLIUM:
            case MapleClass.ClassType.LALA:
            case MapleClass.ClassType.KINESIS:
                return RaiderEffectType.INT;
            
            case MapleClass.ClassType.SHADOWER:
            case MapleClass.ClassType.DUALBLADE:
            case MapleClass.ClassType.NIGHTWALKER:
            case MapleClass.ClassType.KADENA:
            case MapleClass.ClassType.KALI:
            case MapleClass.ClassType.HOYOUNG:
                return RaiderEffectType.LUK;
            
            case MapleClass.ClassType.XENON:
                return RaiderEffectType.STR_DEX_LUK; // 5 10 20 40 50
            
            case MapleClass.ClassType.SOUL_MASTER:
            case MapleClass.ClassType.MIKHAIL:
                return RaiderEffectType.MAX_HP; // 250 500 1000 2000 2500
            
            case MapleClass.ClassType.DARK_KNIGHT:
                return RaiderEffectType.MAX_HP_RATE; // 2 3 4 5 6
            
            case MapleClass.ClassType.BLASTER:
                return RaiderEffectType.IGNORE_ARMOR; // 1 2 3 5 6
            
            case MapleClass.ClassType.DEMON_SLAYER:
                return RaiderEffectType.IMMUNE; // 1 2 3 4 5
            
            case MapleClass.ClassType.DEMON_AVENGER:
                return RaiderEffectType.BOSS_DAMAGE; // 1 2 3 5 6
            
            case MapleClass.ClassType.ZERO:
                return RaiderEffectType.EXP_UP; // 4 6 8 10 12
            
            case MapleClass.ClassType.SINGOONG:
            case MapleClass.ClassType.NIGHTLOAD:
                return RaiderEffectType.CRIT_CHANCE; // 1 2 3 4 5
            
            case MapleClass.ClassType.MERCEDES:
                return RaiderEffectType.COOLDOWN_DECREASE_RATE; // 2 3 4 5 6
            
            case MapleClass.ClassType.ARCMAGE_FP:
                return RaiderEffectType.MAX_MP_RATE; // 2 3 4 5 6
            
            case MapleClass.ClassType.WILD_HUNTER:
                return RaiderEffectType.CHANCE_DAMAGE; // 4 8 12 16 20
            
            case MapleClass.ClassType.MECHANIC:
                return RaiderEffectType.BUFF_DURATION; // 5 10 15 20 25
            
            case MapleClass.ClassType.CAPTAIN:
                return RaiderEffectType.SUMMON_DURATION; // 4 6 8 10 12
            
            case MapleClass.ClassType.PHANTOM:
                return RaiderEffectType.MESO_DROP; // 1 2 3 4 5
            
            case MapleClass.ClassType.EUNWALL:
                return RaiderEffectType.CRIT_DAMAGE; // 1 2 3 5 6
            
            case MapleClass.ClassType.NONE: // 메이플 M
                return RaiderEffectType.ATTACK_MAGIC_POWER; // 5 10 15 20
            
            case MapleClass.ClassType.EVAN:
            case MapleClass.ClassType.ARAN:
            default:
                return RaiderEffectType.OTHER;
        }
    }

    public enum RaiderRank
    {
        NONE = 0,
        B = 1,
        A = 2,
        S = 3,
        SS = 4,
        SSS = 5
    }

    public static RaiderRank GetRaiderRank(int level, MapleClass.ClassType classType = MapleClass.ClassType.NONE)
    {
        if (classType == MapleClass.ClassType.NONE) // 메이플스토리 M
        {
            return level switch
            {
                >= 120 => RaiderRank.SS,
                >= 70 => RaiderRank.S,
                >= 50 => RaiderRank.A,
                >= 30 => RaiderRank.B,
                _ => RaiderRank.NONE
            };
        }
        
        if (classType == MapleClass.ClassType.ZERO)
        {
            return level switch
            {
                >= 250 => RaiderRank.SSS,
                >= 200 => RaiderRank.SS,
                >= 180 => RaiderRank.S,
                >= 160 => RaiderRank.A,
                >= 130 => RaiderRank.B,
                _ => RaiderRank.NONE
            };
        }
        return level switch
        {
            >= 250 => RaiderRank.SSS,
            >= 200 => RaiderRank.SS,
            >= 140 => RaiderRank.S,
            >= 100 => RaiderRank.A,
            >= 60 => RaiderRank.B,
            _ => RaiderRank.NONE
        };
    }

    public enum ClaimType : byte
    {
        INNER_0 = 1,
        INNER_1 = 2,
        INNER_2 = 3,
        INNER_3 = 4,
        INNER_4 = 5,
        INNER_5 = 6,
        INNER_6 = 7,
        INNER_7 = 8,
        
        IMMUNE = 11,
        EXP_UP = 12,
        CRIT_CHANCE = 13,
        BOSS_DAMAGE = 14,
        COMMON_DAMAGE = 15,
        BUFF_DURATION = 16,
        IGNORE_ARMOR = 17,
        CRIT_DAMAGE = 18
    }

    private static byte[][] UnionField = {
            //    X=-11                                                                               X=10
        new byte[]{18, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13}, // Y = 10
        new byte[]{18, 18, 11, 11, 11, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13, 13},
        new byte[]{18, 18, 18, 11, 11, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 11, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 13, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 18, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 13, 13, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 18,  8,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  3, 13, 13, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 18,  8,  8,  1,  1,  1,  1,  2,  2,  2,  3,  3,  3, 13, 13, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 18,  8,  8,  8,  1,  1,  1,  2,  2,  3,  3,  3,  3, 13, 13, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 18,  8,  8,  8,  8,  1,  1,  2,  2,  3,  3,  3,  3, 13, 13, 13, 13, 13},
        new byte[]{18, 18, 18, 18, 18,  8,  8,  8,  8,  8,  1,  2,  3,  3,  3,  3,  3, 13, 13, 13, 13, 13},
        new byte[]{17, 17, 17, 17, 17,  7,  7,  7,  7,  7,  6,  5,  4,  4,  4,  4,  4, 14, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 17, 17,  7,  7,  7,  7,  6,  6,  5,  5,  4,  4,  4,  4, 14, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 17, 17,  7,  7,  7,  6,  6,  6,  5,  5,  5,  4,  4,  4, 14, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 17, 17,  7,  7,  6,  6,  6,  6,  5,  5,  5,  5,  4,  4, 14, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 17, 17,  7,  6,  6,  6,  6,  6,  5,  5,  5,  5,  5,  4, 14, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 17, 17, 16, 16, 16, 16, 16, 16, 15, 15, 15, 15, 15, 15, 14, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 17, 16, 16, 16, 16, 16, 16, 16, 15, 15, 15, 15, 15, 15, 15, 14, 14, 14, 14},
        new byte[]{17, 17, 17, 16, 16, 16, 16, 16, 16, 16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 14, 14, 14},
        new byte[]{17, 17, 16, 16, 16, 16, 16, 16, 16, 16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 14, 14},
        new byte[]{17, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 14}, // Y = -9
    };

    public static ClaimType GetOptionBySlot(sbyte x, sbyte y)
    {
        int rx = x + 11, ry = Math.Abs(y - 10);
        return (ClaimType) UnionField[ry][rx];
    }

    public static Dictionary<ClaimType, int> GetUnionOption(sbyte[][] claims)
    {
        Dictionary<ClaimType, int> claimCount = new Dictionary<ClaimType, int>();
        foreach (sbyte[] claim in claims)
        {
            ClaimType ct = GetOptionBySlot(claim[0], claim[1]);
            int c = claimCount.GetValueOrDefault(ct, 0) + 1;
            if (!claimCount.TryAdd(ct, c)) claimCount[ct] = c;
        }
        return claimCount;
    }
}