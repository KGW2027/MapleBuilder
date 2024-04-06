using System.Text.Json.Nodes;

namespace MapleAPI.Enum;

public class MaplePotentialGrade
{
    public enum GradeType
    {
        NONE,
        RARE,
        EPIC,
        UNIQUE,
        LEGENDARY
    }
    
    public static GradeType GetPotentialGrade(JsonNode? value)
    {
        if (value == null) return GradeType.NONE;
        return value.ToString() switch
        {
            "레어" => GradeType.RARE,
            "에픽" => GradeType.EPIC,
            "유니크" => GradeType.UNIQUE,
            "레전드리" => GradeType.LEGENDARY,
            _ => GradeType.NONE
        };
    }
}