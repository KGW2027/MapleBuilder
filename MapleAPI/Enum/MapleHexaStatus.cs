namespace MapleAPI.Enum;

public class MapleHexaStatus
{
    public struct HexaStatus
    {
        public KeyValuePair<MapleStatus.StatusType, int> MainStat;
        public KeyValuePair<MapleStatus.StatusType, int> SubStat1;
        public KeyValuePair<MapleStatus.StatusType, int> SubStat2;

        public HexaStatus()
        {
            MainStat = KeyValuePair.Create(MapleStatus.StatusType.OTHER, 0);
            SubStat1 = KeyValuePair.Create(MapleStatus.StatusType.OTHER, 0);
            SubStat2 = KeyValuePair.Create(MapleStatus.StatusType.OTHER, 0);
        }
    }
    
    public static MapleStatus.StatusType GetStatusTypeFromHexaStatus(string name)
    {
        name = name.Replace("증가", "").Trim();
        return name switch
        {
            "크리티컬 데미지" => MapleStatus.StatusType.CRITICAL_DAMAGE,
            "보스 데미지" => MapleStatus.StatusType.BOSS_DAMAGE,
            "방어율 무시" => MapleStatus.StatusType.IGNORE_DEF,
            "데미지" => MapleStatus.StatusType.DAMAGE,
            "공격력" => MapleStatus.StatusType.ATTACK_POWER,
            "마력" => MapleStatus.StatusType.MAGIC_POWER,
            "주력 스탯" => MapleStatus.StatusType.MAIN_STAT_FLAT,
            _ => MapleStatus.StatusType.OTHER
        };
    }

    public static double GetStatusValue(MapleStatus.StatusType statusType, int level, bool isMain, MapleClass.ClassType classType)
    {
        if (isMain) level = level + Math.Max(level - 4, 0) + Math.Max(level - 7, 0) + Math.Max(level - 9, 0);
        return statusType switch
        {
            MapleStatus.StatusType.CRITICAL_DAMAGE => 0.35 * level,
            MapleStatus.StatusType.BOSS_DAMAGE => 1 * level,
            MapleStatus.StatusType.IGNORE_DEF => 1 * level,
            MapleStatus.StatusType.DAMAGE => 0.75 * level,
            MapleStatus.StatusType.ATTACK_POWER => 5 * level,
            MapleStatus.StatusType.MAGIC_POWER => 5 * level,
            MapleStatus.StatusType.MAIN_STAT_FLAT => classType == MapleClass.ClassType.XENON ? 48 * level : classType == MapleClass.ClassType.DEMON_AVENGER ? 2100 : 100,
            _ => 0,
        };
    }
}