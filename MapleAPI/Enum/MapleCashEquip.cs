namespace MapleAPI.Enum;

public class MapleCashEquip
{
    public enum LabelType
    {
        RED,
        BLACK,
        MASTER,
        OTHER
    }

    public static LabelType GetLabelType(string name)
    {
        return name switch
        {
            "레드라벨" => LabelType.RED,
            "블랙라벨" => LabelType.BLACK,
            "마스터라벨" => LabelType.MASTER,
            _ => LabelType.OTHER
        };
    }
}