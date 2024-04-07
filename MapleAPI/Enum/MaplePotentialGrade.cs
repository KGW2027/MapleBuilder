using System.Text.Json.Nodes;

namespace MapleAPI.Enum;

public class MaplePotentialGrade
{
    public enum GradeType
    {
        NONE = 0,
        RARE = 1,
        EPIC = 2,
        UNIQUE = 3,
        LEGENDARY = 4
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

    public static string GetPotentialGradeString(GradeType type)
    {
        return type switch
        {
            GradeType.RARE => "레어",
            GradeType.EPIC => "에픽",
            GradeType.UNIQUE => "유니크",
            GradeType.LEGENDARY => "레전드리",
            _ => ""
        };
    }
}