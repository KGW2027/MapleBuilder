using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MaplePropensityInfo : MapleInfo
{
    public MaplePropensityInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        PropensityLevels = new Dictionary<MaplePropensity.PropensityType, int>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.PROPENSITY;
    }

    public readonly Dictionary<MaplePropensity.PropensityType, int> PropensityLevels;

    private protected override void ParseInfo()
    {
        foreach (MaplePropensity.PropensityType propensityType in
                 System.Enum.GetValues<MaplePropensity.PropensityType>())
        {
            if (!TryGet($"{propensityType.ToString().ToLower()}_level", out int level)) continue;
            PropensityLevels[propensityType] = level;
        }
    }
}