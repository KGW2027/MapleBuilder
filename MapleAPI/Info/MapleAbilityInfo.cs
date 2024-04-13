using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MapleAbilityInfo : MapleInfo
{
    public MapleAbilityInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        AbilityValues = new Dictionary<MapleStatus.StatusType, int>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.ABILITY;
    }

    public readonly Dictionary<MapleStatus.StatusType, int> AbilityValues;
    
    private protected override void ParseInfo()
    {
        if (!TryGet("ability_info", out JsonArray? abilityData) || abilityData == null) return;
        foreach (var value in abilityData)
        {
            string abValue = value!["ability_value"]!.ToString();
            MapleStatus.StatusType abType = MapleAbility.TryParse(abValue);
            string abStr = MapleAbility.GetAbilityString(abType);
            int valueIndex = abStr.IndexOf("%d", StringComparison.CurrentCulture);
            string parse = valueIndex >= 0 ? abValue.Substring(valueIndex, 2).Replace("%", "").Trim() : "";
        
            if (int.TryParse(parse, out int abNumValue))
            {
                AbilityValues.Add(abType, abNumValue);
            }
            else
            {
                MaplePotentialGrade.GradeType abGrade =
                    MaplePotentialGrade.GetPotentialGrade(value["ability_grade"]);
                AbilityValues.Add(abType, MapleAbility.GetMinMax(abType, abGrade)[0]);
            }
        }
    }
}