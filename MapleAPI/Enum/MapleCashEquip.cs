using System.Reflection.Emit;

namespace MapleAPI.Enum;

public class MapleCashEquip
{
    public enum LabelType : int
    {
        RED = 1,
        BLACK = 2,
        MASTER = 3,
        OTHER = 0
    }

    // Idx 0 : Weapon, Idx 1 : Armors
    private static Dictionary<LabelType, int[][]> LabelStatusCap = new()
    {
        {LabelType.OTHER,   new[] { new[]{ 0,  0}, new[]{ 0,  0} }},
        {LabelType.RED,     new[] { new[]{16, 20}, new[]{ 6, 10} }},
        {LabelType.BLACK,   new[] { new[]{21, 25}, new[]{16, 20} }},
        {LabelType.MASTER,  new[] { new[]{30, 30}, new[]{30, 30} }},
    };

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

    public static string GetLabelTypeString(LabelType labelType)
    {
        return labelType switch
        {
            LabelType.RED => "레드라벨",
            LabelType.BLACK => "블랙라벨",
            LabelType.MASTER => "마스터라벨",
            LabelType.OTHER => "무라벨",
            _ => throw new ArgumentOutOfRangeException(nameof(labelType), labelType, null)
        };
    }

    public static int[] GetLabelOptionCap(LabelType labelType, bool isWeapon)
    {
        return LabelStatusCap[labelType][isWeapon ? 0 : 1];
    }
}