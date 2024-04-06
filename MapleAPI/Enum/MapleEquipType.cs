namespace MapleAPI.Enum;

public class MapleEquipType
{
    public enum EquipType
    {
        RING,
        PENDANT,
        FACE,
        EYE,
        EARRING,
        BADGE,
        MEDAL,
        BELT,
        POCKET,
        HEART,
        ANDROID,

        WEAPON,
        SUB_WEAPON,
        EMBLEM,

        HELMET,
        TOP,
        BOTTOM,
        TOP_BOTTOM,
        SHOULDER,
        GLOVE,
        CAPE,
        BOOT,

        TITLE,
    }
    public static EquipType GetEquipType(string slot)
    {
        return slot.TrimEnd('1', '2', '3', '4') switch
        {
            "모자" => EquipType.HELMET,
            "얼굴장식" => EquipType.FACE,
            "눈장식" => EquipType.EYE,
            "귀고리" => EquipType.EARRING,
            "상의" => EquipType.TOP,
            "하의" => EquipType.BOTTOM,
            "한벌옷" => EquipType.TOP_BOTTOM,
            "신발" => EquipType.BOOT,
            "장갑" => EquipType.GLOVE,
            "망토" => EquipType.CAPE,
            "보조무기" => EquipType.SUB_WEAPON,
            "무기" => EquipType.WEAPON,
            "반지" => EquipType.RING,
            "펜던트" => EquipType.PENDANT,
            "훈장" => EquipType.MEDAL,
            "벨트" => EquipType.BELT,
            "어깨장식" => EquipType.SHOULDER,
            "포켓 아이템" => EquipType.POCKET,
            "기계 심장" => EquipType.HEART,
            "뱃지" => EquipType.BADGE,
            "엠블렘" => EquipType.EMBLEM,
            _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
        };
    }
}