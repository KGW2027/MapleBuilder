namespace MapleAPI.Enum;

public class MapleArtifact
{
    public enum ArtifactType
    {
        ALL_STAT,
        HP_MP,
        ATTACK_MAGIC_POWER,
        DAMAGE,
        BOSS_DAMAGE,
        IGNORE_ARMOR,
        BUFF_DURATION,
        COOLDOWN_IGNORE,
        MESO_DROP,
        ITEM_DROP,
        CRIT_CHANCE,
        CRIT_DAMAGE,
        EXP_UP,
        IMMUNE,
        SUMMON_DURATION,
        FINAL_ATTACK,
        UNKNOWN
    }

    public static ArtifactType GetArtifactType(string name)
    {
        return name switch
        {
            "올스탯 증가" => ArtifactType.ALL_STAT,
            "최대 HP/MP 증가" => ArtifactType.HP_MP,
            "공격력/마력 증가" => ArtifactType.ATTACK_MAGIC_POWER,
            "데미지 증가" => ArtifactType.DAMAGE,
            "보스 몬스터 공격 시 데미지 증가" => ArtifactType.BOSS_DAMAGE,
            "몬스터 방어율 무시 증가" => ArtifactType.IGNORE_ARMOR,
            "버프 지속시간 증가" => ArtifactType.BUFF_DURATION,
            "재사용 대기시간 미적용 증가" => ArtifactType.COOLDOWN_IGNORE,
            "메소 획득량 증가" => ArtifactType.MESO_DROP,
            "아이템 드롭률 증가" => ArtifactType.ITEM_DROP,
            "크리티컬 확률 증가" => ArtifactType.CRIT_CHANCE,
            "크리티컬 데미지 증가" => ArtifactType.CRIT_DAMAGE,
            "추가 경험치 획득 증가" => ArtifactType.EXP_UP,
            "상태 이상 내성 증가" => ArtifactType.IMMUNE,
            "소환수 지속시간 증가" => ArtifactType.SUMMON_DURATION,
            "파이널 어택 증가" => ArtifactType.FINAL_ATTACK,
            _ => ArtifactType.UNKNOWN
        };
    }
}