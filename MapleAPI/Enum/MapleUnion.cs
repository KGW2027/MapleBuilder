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

    public static string GetRaiderEffectString(MapleStatus.StatusType effectType)
    {
        return effectType switch
        {
            MapleStatus.StatusType.INT_FLAT => "INT %d 증가",
            MapleStatus.StatusType.STR_FLAT => "STR %d 증가",
            MapleStatus.StatusType.LUK_FLAT => "LUK %d 증가",
            MapleStatus.StatusType.DEX_FLAT => "DEX %d 증가",
            MapleStatus.StatusType.HP => "최대 HP %d 증가",
            MapleStatus.StatusType.HP_RATE => "최대 HP %d% 증가",
            MapleStatus.StatusType.BOSS_DAMAGE => "보스 공격 시 데미지 %d% 증가",
            MapleStatus.StatusType.IGNORE_DEF => "방어율 무시 %d% 증가",
            MapleStatus.StatusType.ABN_STATUS_RESIS => "상태 이상 내성 %d 증가",
            MapleStatus.StatusType.STR_DEX_LUK => "STR, DEX, LUK %d 증가",
            MapleStatus.StatusType.MP_RATE => "최대 MP %d% 증가",
            MapleStatus.StatusType.ATTACK_AND_MAGIC => "공격력/마력 %d 증가",
            MapleStatus.StatusType.MESO_DROP => "메소 획득량 %d 증가",
            MapleStatus.StatusType.EXP_INCREASE => "경험치 획득량 %d 증가",
            MapleStatus.StatusType.CRITICAL_CHANCE => "크리티컬 확률 %d% 증가",
            MapleStatus.StatusType.BUFF_DURATION => "버프 지속 시간 %d% 증가",
            MapleStatus.StatusType.SUMMON_DURATION => "소환수 지속 시간 %d% 증가",
            MapleStatus.StatusType.CRITICAL_DAMAGE => "크리티컬 데미지 %d% 증가",
            MapleStatus.StatusType.WILD_HUNTER_DMG => "공격 시 20% 확률로 데미지 %d% 증가",
            MapleStatus.StatusType.COOL_DEC_RATE => "스킬 재사용 대기시간 %d%감소 (1초 미만으로 줄어들지 않음)",
            MapleStatus.StatusType.OTHER => "효과 없음",
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };
    }

    public static int GetRaiderEffectValue(MapleStatus.StatusType effectType, RaiderRank rank)
    {
        switch (effectType)
        {
            case MapleStatus.StatusType.INT_FLAT:
            case MapleStatus.StatusType.STR_FLAT:
            case MapleStatus.StatusType.LUK_FLAT:
            case MapleStatus.StatusType.DEX_FLAT:
                return rank switch
                {
                    RaiderRank.SSS => 100,
                    RaiderRank.SS => 80,
                    RaiderRank.S => 40,
                    RaiderRank.A => 20,
                    RaiderRank.B => 10,
                    _ => 0
                };
            case MapleStatus.StatusType.STR_DEX_LUK:
                return rank switch
                {
                    RaiderRank.SSS => 50,
                    RaiderRank.SS => 40,
                    RaiderRank.S => 20,
                    RaiderRank.A => 10,
                    RaiderRank.B => 5,
                    _ => 0
                };
            case MapleStatus.StatusType.HP:
                return rank switch
                {
                    RaiderRank.SSS => 2500,
                    RaiderRank.SS => 2000,
                    RaiderRank.S => 1000,
                    RaiderRank.A => 500,
                    RaiderRank.B => 250,
                    _ => 0
                };
            case MapleStatus.StatusType.HP_RATE:
            case MapleStatus.StatusType.MP_RATE:
            case MapleStatus.StatusType.COOL_DEC_RATE:
                return rank switch
                {
                    RaiderRank.SSS => 6,
                    RaiderRank.SS => 5,
                    RaiderRank.S => 4,
                    RaiderRank.A => 3,
                    RaiderRank.B => 2,
                    _ => 0
                };
            case MapleStatus.StatusType.IGNORE_DEF:
            case MapleStatus.StatusType.BOSS_DAMAGE:
            case MapleStatus.StatusType.CRITICAL_DAMAGE:
                return rank switch
                {
                    RaiderRank.SSS => 6,
                    RaiderRank.SS => 5,
                    RaiderRank.S => 3,
                    RaiderRank.A => 2,
                    RaiderRank.B => 1,
                    _ => 0
                };
            case MapleStatus.StatusType.ABN_STATUS_RESIS:
            case MapleStatus.StatusType.CRITICAL_CHANCE:
            case MapleStatus.StatusType.MESO_DROP:
                return rank switch
                {
                    RaiderRank.SSS => 5,
                    RaiderRank.SS => 4,
                    RaiderRank.S => 3,
                    RaiderRank.A => 2,
                    RaiderRank.B => 1,
                    _ => 0
                };
            case MapleStatus.StatusType.EXP_INCREASE:
            case MapleStatus.StatusType.SUMMON_DURATION:
                return rank switch
                {
                    RaiderRank.SSS => 12,
                    RaiderRank.SS => 10,
                    RaiderRank.S => 8,
                    RaiderRank.A => 6,
                    RaiderRank.B => 4,
                    _ => 0
                };
            case MapleStatus.StatusType.ATTACK_AND_MAGIC:
                if (rank == RaiderRank.SSS) rank = RaiderRank.SS;
                goto case MapleStatus.StatusType.BUFF_DURATION;
            case MapleStatus.StatusType.BUFF_DURATION:
                return rank switch
                {
                    RaiderRank.SSS => 25,
                    RaiderRank.SS => 20,
                    RaiderRank.S => 15,
                    RaiderRank.A => 10,
                    RaiderRank.B => 5,
                    _ => 0
                };
            case MapleStatus.StatusType.WILD_HUNTER_DMG:
                return rank switch
                {
                    RaiderRank.SSS => 20,
                    RaiderRank.SS => 16,
                    RaiderRank.S => 12,
                    RaiderRank.A => 8,
                    RaiderRank.B => 4,
                    _ => 0
                };
            case MapleStatus.StatusType.OTHER:
            default:
                return 0;
        }
    }

    public static MapleStatus.StatusType GetRaiderEffectByClass(MapleClass.ClassType classType)
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
                return MapleStatus.StatusType.STR_FLAT; // 10 20 40 80 100
            
            case MapleClass.ClassType.BOW_MASTER:
            case MapleClass.ClassType.PATH_FINDER:
            case MapleClass.ClassType.WIND_BREAKER:
            case MapleClass.ClassType.ANGELICBUSTER:
            case MapleClass.ClassType.KAIN:
                return MapleStatus.StatusType.DEX_FLAT;
            
            case MapleClass.ClassType.ARCMAGE_TC:
            case MapleClass.ClassType.BISHOP:
            case MapleClass.ClassType.FLAME_WIZARD:
            case MapleClass.ClassType.BATTLE_MAGE:
            case MapleClass.ClassType.LUMINUS:
            case MapleClass.ClassType.ILLIUM:
            case MapleClass.ClassType.LALA:
            case MapleClass.ClassType.KINESIS:
                return MapleStatus.StatusType.INT_FLAT;
            
            case MapleClass.ClassType.SHADOWER:
            case MapleClass.ClassType.DUALBLADE:
            case MapleClass.ClassType.NIGHTWALKER:
            case MapleClass.ClassType.KADENA:
            case MapleClass.ClassType.KALI:
            case MapleClass.ClassType.HOYOUNG:
                return MapleStatus.StatusType.LUK_FLAT;
            
            case MapleClass.ClassType.XENON:
                return MapleStatus.StatusType.STR_DEX_LUK; // 5 10 20 40 50
            
            case MapleClass.ClassType.SOUL_MASTER:
            case MapleClass.ClassType.MIKHAIL:
                return MapleStatus.StatusType.HP; // 250 500 1000 2000 2500
            
            case MapleClass.ClassType.DARK_KNIGHT:
                return MapleStatus.StatusType.HP_RATE; // 2 3 4 5 6
            
            case MapleClass.ClassType.BLASTER:
                return MapleStatus.StatusType.IGNORE_DEF; // 1 2 3 5 6
            
            case MapleClass.ClassType.DEMON_SLAYER:
                return MapleStatus.StatusType.ABN_STATUS_RESIS; // 1 2 3 4 5
            
            case MapleClass.ClassType.DEMON_AVENGER:
                return MapleStatus.StatusType.BOSS_DAMAGE; // 1 2 3 5 6
            
            case MapleClass.ClassType.ZERO:
                return MapleStatus.StatusType.EXP_INCREASE; // 4 6 8 10 12
            
            case MapleClass.ClassType.SINGOONG:
            case MapleClass.ClassType.NIGHTLOAD:
                return MapleStatus.StatusType.CRITICAL_CHANCE; // 1 2 3 4 5
            
            case MapleClass.ClassType.MERCEDES:
                return MapleStatus.StatusType.COOL_DEC_RATE; // 2 3 4 5 6
            
            case MapleClass.ClassType.ARCMAGE_FP:
                return MapleStatus.StatusType.MP_RATE; // 2 3 4 5 6
            
            case MapleClass.ClassType.WILD_HUNTER:
                return MapleStatus.StatusType.WILD_HUNTER_DMG; // 4 8 12 16 20
            
            case MapleClass.ClassType.MECHANIC:
                return MapleStatus.StatusType.BUFF_DURATION; // 5 10 15 20 25
            
            case MapleClass.ClassType.CAPTAIN:
                return MapleStatus.StatusType.SUMMON_DURATION; // 4 6 8 10 12
            
            case MapleClass.ClassType.PHANTOM:
                return MapleStatus.StatusType.MESO_DROP; // 1 2 3 4 5
            
            case MapleClass.ClassType.EUNWALL:
                return MapleStatus.StatusType.CRITICAL_DAMAGE; // 1 2 3 5 6
            
            case MapleClass.ClassType.NONE: // 메이플 M
                return MapleStatus.StatusType.ATTACK_AND_MAGIC; // 5 10 15 20
            
            case MapleClass.ClassType.EVAN:
            case MapleClass.ClassType.ARAN:
            default:
                return MapleStatus.StatusType.OTHER;
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

    public static MapleStatus.StatusType GetStatusTypeByUnionField(string field)
    {
        return field switch
        {
            "유니온 STR" => MapleStatus.StatusType.STR,
            "유니온 DEX" => MapleStatus.StatusType.DEX,
            "유니온 INT" => MapleStatus.StatusType.INT,
            "유니온 LUK" => MapleStatus.StatusType.LUK,
            "유니온 최대 HP" => MapleStatus.StatusType.HP,
            "유니온 최대 MP" => MapleStatus.StatusType.MP,
            "유니온 공격력" => MapleStatus.StatusType.ATTACK_POWER,
            "유니온 마력" => MapleStatus.StatusType.MAGIC_POWER,
            _ => MapleStatus.StatusType.OTHER
        };
    }

    private static MapleStatus.StatusType GetStatusTypeByClaimType(ClaimType claimType, List<MapleStatus.StatusType> innerTypes)
    {
        return claimType switch
        {
            ClaimType.INNER_0 => innerTypes[0],
            ClaimType.INNER_1 => innerTypes[1],
            ClaimType.INNER_2 => innerTypes[2],
            ClaimType.INNER_3 => innerTypes[3],
            ClaimType.INNER_4 => innerTypes[4],
            ClaimType.INNER_5 => innerTypes[5],
            ClaimType.INNER_6 => innerTypes[6],
            ClaimType.INNER_7 => innerTypes[7],
            ClaimType.IMMUNE => MapleStatus.StatusType.ABN_STATUS_RESIS,
            ClaimType.EXP_UP => MapleStatus.StatusType.EXP_INCREASE,
            ClaimType.CRIT_CHANCE => MapleStatus.StatusType.CRITICAL_CHANCE,
            ClaimType.BOSS_DAMAGE => MapleStatus.StatusType.BOSS_DAMAGE,
            ClaimType.COMMON_DAMAGE => MapleStatus.StatusType.COMMON_DAMAGE,
            ClaimType.BUFF_DURATION => MapleStatus.StatusType.BUFF_DURATION,
            ClaimType.IGNORE_ARMOR => MapleStatus.StatusType.IGNORE_DEF,
            ClaimType.CRIT_DAMAGE => MapleStatus.StatusType.CRITICAL_DAMAGE,
            _ => throw new ArgumentOutOfRangeException(nameof(claimType), claimType, null)
        };
    }

    public static Dictionary<MapleStatus.StatusType, int> GetUnionOption(sbyte[][] claims, List<MapleStatus.StatusType> innerTypes)
    {
        Dictionary<MapleStatus.StatusType, int> claimCount = new Dictionary<MapleStatus.StatusType, int>();
        foreach (sbyte[] claim in claims)
        {
            ClaimType ct = GetOptionBySlot(claim[0], claim[1]);
            MapleStatus.StatusType st = GetStatusTypeByClaimType(ct, innerTypes);
            int c = claimCount.GetValueOrDefault(st, 0) + 1;
            if (!claimCount.TryAdd(st, c)) claimCount[st] = c;
        }
        return claimCount;
    }
}