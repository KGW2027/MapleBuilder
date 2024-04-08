namespace MapleAPI.Enum;

public class MapleClass
{
    public enum ClassType
    {
        HERO,
        PALADIN,
        DARK_KNIGHT,
        SOUL_MASTER,
        MIKHAIL,
        BLASTER,
        DEMON_SLAYER,
        DEMON_AVENGER,
        ARAN,
        KAISER,
        ADEL,
        ZERO,
        ARCMAGE_FP,
        ARCMAGE_TC,
        BISHOP,
        FLAME_WIZARD,
        BATTLE_MAGE,
        EVAN,
        LUMINUS,
        ILLIUM,
        LALA,
        KINESIS,
        BOW_MASTER,
        SINGOONG,
        PATH_FINDER,
        WIND_BREAKER,
        WILD_HUNTER,
        MERCEDES,
        KAIN,
        NIGHTLOAD,
        SHADOWER,
        DUALBLADE,
        NIGHTWALKER,
        XENON,
        PHANTOM,
        KADENA,
        KALI,
        HOYOUNG,
        VAIPER,
        CAPTAIN,
        CANONSHOOTER,
        STRIKER,
        MECHANIC,
        EUNWALL,
        ANGELICBUSTER,
        ARC,
        NONE
    }
    
    private static readonly MaplePotentialOption.OptionType[] STR_DEX =
        {MaplePotentialOption.OptionType.STR, MaplePotentialOption.OptionType.DEX, MaplePotentialOption.OptionType.OTHER};
    private static readonly MaplePotentialOption.OptionType[] DEX_STR =
        {MaplePotentialOption.OptionType.DEX, MaplePotentialOption.OptionType.STR, MaplePotentialOption.OptionType.OTHER};
    private static readonly MaplePotentialOption.OptionType[] INT_LUK =
        {MaplePotentialOption.OptionType.INT, MaplePotentialOption.OptionType.LUK, MaplePotentialOption.OptionType.OTHER};
    private static readonly MaplePotentialOption.OptionType[] LUK_DEX =
        {MaplePotentialOption.OptionType.LUK, MaplePotentialOption.OptionType.DEX, MaplePotentialOption.OptionType.OTHER};
    private static readonly MaplePotentialOption.OptionType[] LUK_DEX_STR =
        {MaplePotentialOption.OptionType.LUK, MaplePotentialOption.OptionType.DEX, MaplePotentialOption.OptionType.STR};
    private static readonly MaplePotentialOption.OptionType[] HP =
        {MaplePotentialOption.OptionType.MAX_HP, MaplePotentialOption.OptionType.OTHER, MaplePotentialOption.OptionType.OTHER};
    private static readonly MaplePotentialOption.OptionType[] NONE =
        {MaplePotentialOption.OptionType.OTHER, MaplePotentialOption.OptionType.OTHER, MaplePotentialOption.OptionType.OTHER};

    public static string GetMapleClassString(ClassType classType)
    {
        return classType switch
        {
            ClassType.HERO => "히어로",
            ClassType.PALADIN => "팔라딘",
            ClassType.DARK_KNIGHT => "다크나이트",
            ClassType.SOUL_MASTER => "소울마스터",
            ClassType.MIKHAIL => "미하일",
            ClassType.BLASTER => "블래스터",
            ClassType.DEMON_SLAYER => "데몬슬레이어",
            ClassType.DEMON_AVENGER => "데몬어벤저",
            ClassType.ARAN => "아란",
            ClassType.KAISER => "카이저",
            ClassType.ADEL => "아델",
            ClassType.ZERO => "제로",
            ClassType.ARCMAGE_FP => "아크메이지(불,독)",
            ClassType.ARCMAGE_TC => "아크메이지(썬,콜)",
            ClassType.BISHOP => "비숍",
            ClassType.FLAME_WIZARD => "플레임위자드",
            ClassType.BATTLE_MAGE => "배틀메이지",
            ClassType.EVAN => "에반",
            ClassType.LUMINUS => "루미너스",
            ClassType.ILLIUM => "일리움",
            ClassType.LALA => "라라",
            ClassType.KINESIS => "키네시스",
            ClassType.BOW_MASTER => "보우마스터",
            ClassType.SINGOONG => "신궁",
            ClassType.PATH_FINDER => "패스파인더",
            ClassType.WIND_BREAKER => "윈드브레이커",
            ClassType.WILD_HUNTER => "와일드헌터",
            ClassType.MERCEDES => "메르세데스",
            ClassType.KAIN => "카인",
            ClassType.NIGHTLOAD => "나이트로드",
            ClassType.SHADOWER => "섀도어",
            ClassType.DUALBLADE => "듀얼블레이더",
            ClassType.NIGHTWALKER => "나이트워커",
            ClassType.XENON => "제논",
            ClassType.PHANTOM => "팬텀",
            ClassType.KADENA => "카데나",
            ClassType.KALI => "칼리",
            ClassType.HOYOUNG => "호영",
            ClassType.VAIPER => "바이퍼",
            ClassType.CAPTAIN => "캡틴",
            ClassType.CANONSHOOTER => "캐논슈터",
            ClassType.STRIKER => "스트라이커",
            ClassType.MECHANIC => "메카닉",
            ClassType.EUNWALL => "은월",
            ClassType.ANGELICBUSTER => "엔젤릭버스터",
            ClassType.ARC => "아크",
            ClassType.NONE => "메이플스토리 M",
            _ => "알 수 없음"
        };
    }
    
    public static ClassType GetMapleClass(string className)
    {
        return className.Replace(" ", "") switch
        {
            "히어로" => ClassType.HERO,
            "팔라딘" => ClassType.PALADIN,
            "다크나이트" => ClassType.DARK_KNIGHT,
            "소울마스터" => ClassType.SOUL_MASTER,
            "미하일" => ClassType.MIKHAIL,
            "블래스터" => ClassType.BLASTER,
            "데몬슬레이어" => ClassType.DEMON_SLAYER,
            "데몬어벤져" => ClassType.DEMON_AVENGER,
            "아란" => ClassType.ARAN,
            "카이저" => ClassType.KAISER,
            "아델" => ClassType.ADEL,
            "제로" => ClassType.ZERO,
            "아크메이지(불,독)" => ClassType.ARCMAGE_FP,
            "아크메이지(썬,콜)" => ClassType.ARCMAGE_TC,
            "비숍" => ClassType.BISHOP,
            "플레임위자드" => ClassType.FLAME_WIZARD,
            "배틀메이지" => ClassType.BATTLE_MAGE,
            "에반" => ClassType.EVAN,
            "루미너스" => ClassType.LUMINUS,
            "일리움" => ClassType.ILLIUM,
            "라라" => ClassType.LALA,
            "키네시스" => ClassType.KINESIS,
            "보우마스터" => ClassType.BOW_MASTER,
            "신궁" => ClassType.SINGOONG,
            "패스파인더" => ClassType.PATH_FINDER,
            "윈드브레이커" => ClassType.WIND_BREAKER,
            "와일드헌터" => ClassType.WILD_HUNTER,
            "메르세데스" => ClassType.MERCEDES,
            "카인" => ClassType.KAIN,
            "나이트로드" => ClassType.NIGHTLOAD,
            "섀도어" => ClassType.SHADOWER,
            "듀얼블레이더" => ClassType.DUALBLADE,
            "나이트워커" => ClassType.NIGHTWALKER,
            "제논" => ClassType.XENON,
            "팬텀" => ClassType.PHANTOM,
            "카데나" => ClassType.KADENA,
            "칼리" => ClassType.KALI,
            "호영" => ClassType.HOYOUNG,
            "바이퍼" => ClassType.VAIPER,
            "캡틴" => ClassType.CAPTAIN,
            "캐논마스터" => ClassType.CANONSHOOTER,
            "스트라이커" => ClassType.STRIKER,
            "메카닉" => ClassType.MECHANIC,
            "은월" => ClassType.EUNWALL,
            "엔젤릭버스터" => ClassType.ANGELICBUSTER,
            "아크" => ClassType.ARC,
            _ => ClassType.NONE
        };
    }
    
    public static MaplePotentialOption.OptionType[] GetClassStatType(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.HERO: // 전사
            case ClassType.PALADIN:
            case ClassType.DARK_KNIGHT:
            case ClassType.SOUL_MASTER:
            case ClassType.MIKHAIL:
            case ClassType.BLASTER:
            case ClassType.DEMON_SLAYER:
            case ClassType.ARAN:
            case ClassType.KAISER:
            case ClassType.ADEL:
            case ClassType.ZERO:
            case ClassType.VAIPER: // 힘해적
            case ClassType.CANONSHOOTER:
            case ClassType.STRIKER:
            case ClassType.EUNWALL:
            case ClassType.ARC:
                return STR_DEX;
            case ClassType.DEMON_AVENGER: // 데벤저
                return HP;
            case ClassType.ARCMAGE_FP: // 법사
            case ClassType.ARCMAGE_TC:
            case ClassType.BISHOP:
            case ClassType.FLAME_WIZARD:
            case ClassType.BATTLE_MAGE:
            case ClassType.EVAN:
            case ClassType.LUMINUS:
            case ClassType.ILLIUM:
            case ClassType.LALA:
            case ClassType.KINESIS:
                return INT_LUK;
            case ClassType.BOW_MASTER: // 궁수
            case ClassType.SINGOONG:
            case ClassType.PATH_FINDER:
            case ClassType.WIND_BREAKER:
            case ClassType.WILD_HUNTER:
            case ClassType.MERCEDES:
            case ClassType.KAIN:
            case ClassType.CAPTAIN: // 덱해적
            case ClassType.ANGELICBUSTER:
            case ClassType.MECHANIC:
                return DEX_STR;
            case ClassType.NIGHTLOAD: // 도적
            case ClassType.NIGHTWALKER:
            case ClassType.PHANTOM:
            case ClassType.KALI:
            case ClassType.HOYOUNG:
                return LUK_DEX;
            case ClassType.SHADOWER: // 2부스텟 도적 + 제논
            case ClassType.KADENA:
            case ClassType.DUALBLADE:
            case ClassType.XENON:
                return LUK_DEX_STR;
        }

        return NONE;
    }

}