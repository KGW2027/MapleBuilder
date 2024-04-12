namespace MapleAPI.Enum;

public class MapleArtifact
{
    public struct ArtifactPanel
    {
        public MapleStatus.StatusType[] StatusTypes;
        public int Level;

        public ArtifactPanel(int level)
        {
            Level = level;
            StatusTypes = new MapleStatus.StatusType[3];
        }
    }
    
    private static readonly List<string> OPTION_LIST = new()
    {
        "올스탯", "최대 HP/MP", "공격력/마력", "데미지", "보스 몬스터 공격 시 데미지", "몬스터 방어율 무시",
        "버프 지속시간", "재사용 대기시간 미적용", "메소 획득량", "아이템 드롭률", "크리티컬 확률",
        "크리티컬 데미지", "추가 경험치 획득", "상태 이상 내성", "소환수 지속시간", "파이널 어택류 스킬 데미지"
    };

    public static List<string> GetOptionList()
    {
        return OPTION_LIST;
    }
    
    public static MapleStatus.StatusType GetArtifactType(string name)
    {
        return name switch
        {
            "올스탯 증가" => MapleStatus.StatusType.ALL_STAT,
            "최대 HP/MP 증가" => MapleStatus.StatusType.HP_AND_MP,
            "공격력/마력 증가" => MapleStatus.StatusType.ATTACK_AND_MAGIC,
            "데미지 증가" => MapleStatus.StatusType.DAMAGE,
            "보스 몬스터 공격 시 데미지 증가" => MapleStatus.StatusType.BOSS_DAMAGE,
            "몬스터 방어율 무시 증가" => MapleStatus.StatusType.IGNORE_DEF,
            "버프 지속시간 증가" => MapleStatus.StatusType.BUFF_DURATION,
            "재사용 대기시간 미적용 증가" => MapleStatus.StatusType.COOL_IGNORE,
            "메소 획득량 증가" => MapleStatus.StatusType.MESO_DROP,
            "아이템 드롭률 증가" => MapleStatus.StatusType.ITEM_DROP,
            "크리티컬 확률 증가" => MapleStatus.StatusType.CRITICAL_CHANCE,
            "크리티컬 데미지 증가" => MapleStatus.StatusType.CRITICAL_DAMAGE,
            "추가 경험치 획득 증가" => MapleStatus.StatusType.EXP_INCREASE,
            "상태 이상 내성 증가" => MapleStatus.StatusType.ABN_STATUS_RESIS,
            "소환수 지속시간 증가" => MapleStatus.StatusType.SUMMON_DURATION,
            "파이널 어택류 스킬 데미지" => MapleStatus.StatusType.FINAL_ATTACK,
            _ => MapleStatus.StatusType.OTHER
        };
    }

    public static string GetArtifactTypeString(MapleStatus.StatusType statusType)
    {
        return statusType switch
        {
            MapleStatus.StatusType.ALL_STAT => "올스탯",
            MapleStatus.StatusType.HP_AND_MP => "최대 HP/MP",
            MapleStatus.StatusType.ATTACK_AND_MAGIC => "공격력/마력",
            MapleStatus.StatusType.DAMAGE => "데미지",
            MapleStatus.StatusType.BOSS_DAMAGE => "보스 몬스터 공격 시 데미지",
            MapleStatus.StatusType.IGNORE_DEF => "몬스터 방어율 무시",
            MapleStatus.StatusType.BUFF_DURATION => "버프 지속시간",
            MapleStatus.StatusType.COOL_IGNORE => "재사용 대기시간 미적용",
            MapleStatus.StatusType.MESO_DROP => "메소 획득량",
            MapleStatus.StatusType.ITEM_DROP => "아이템 드롭률",
            MapleStatus.StatusType.CRITICAL_CHANCE => "크리티컬 확률",
            MapleStatus.StatusType.CRITICAL_DAMAGE => "크리티컬 데미지",
            MapleStatus.StatusType.EXP_INCREASE => "추가 경험치 획득",
            MapleStatus.StatusType.ABN_STATUS_RESIS => "상태 이상 내성",
            MapleStatus.StatusType.SUMMON_DURATION => "소환수 지속시간",
            MapleStatus.StatusType.FINAL_ATTACK => "파이널 어택류 스킬 데미지",
            _ => "잠김"
        };
    }
}