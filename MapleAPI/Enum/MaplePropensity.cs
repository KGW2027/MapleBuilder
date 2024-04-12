namespace MapleAPI.Enum;

public class MaplePropensity
{
    public enum PropensityType
    {
        CHARISMA,       // 카리스마
        SENSIBILITY,    // 감성
        INSIGHT,        // 통찰력
        WILLINGNESS,    // 의지
        HANDICRAFT,     // 손재주
        CHARM           // 매력
    }

    public static MapleStatus.StatusType[] GetStatusByPropensity(PropensityType type)
    {
        return type switch
        {
            PropensityType.CHARISMA => new[]{MapleStatus.StatusType.IGNORE_DEF},
            PropensityType.SENSIBILITY => new[]{MapleStatus.StatusType.BUFF_DURATION},
            PropensityType.INSIGHT => new[]{MapleStatus.StatusType.IGNORE_IMMUNE},
            PropensityType.WILLINGNESS => new[]{MapleStatus.StatusType.HP, MapleStatus.StatusType.ABN_STATUS_RESIS},
            _ => new[]{MapleStatus.StatusType.OTHER}
        };
    }

    public static double[] GetStatusValueByPropensity(PropensityType type)
    {
        return type switch
        {
            PropensityType.CHARISMA => new[]{5.0, 0.5},
            PropensityType.SENSIBILITY => new[]{10.0, 1.0},
            PropensityType.INSIGHT => new[]{10.0, 0.5},
            PropensityType.WILLINGNESS => new[]{5.0, 100.0, 1.0},
            _ => new[]{0.0, 0.0}
        };
    }

    public static PropensityType GetPropensityType(string name)
    {
        return name switch
        {
            "카리스마" => PropensityType.CHARISMA,
            "감성"    => PropensityType.SENSIBILITY,
            "통찰력"   => PropensityType.INSIGHT,
            "의지"    => PropensityType.WILLINGNESS,
            "손재주"   => PropensityType.HANDICRAFT,
            "매력"    => PropensityType.CHARM,
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        };
    }
    
    
}