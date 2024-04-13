using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MapleHyperStatInfo : MapleInfo
{
    public MapleHyperStatInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        HyperStatLevels = new Dictionary<MapleStatus.StatusType, int>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.HYPER_STAT;
    }

    public readonly Dictionary<MapleStatus.StatusType, int> HyperStatLevels;

    private protected override void ParseInfo()
    {
        int presetId = TryGet("use_preset_no", out int id) ? id : 1;
        if (!TryGet($"hyper_stat_preset_{presetId}", out JsonArray? hyperStats) || hyperStats == null) return;

        foreach (var stat in hyperStats)
        {
            if (stat is not JsonObject statObject) continue;
            MapleStatus.StatusType statType = MapleHyperStat.GetStatType(statObject["stat_type"]!.ToString());
            if (statType == MapleStatus.StatusType.OTHER) continue;
            if (!int.TryParse(statObject["stat_level"]!.ToString(), out int statLevel)) continue;
            if (!HyperStatLevels.TryAdd(statType, statLevel)) HyperStatLevels[statType] = statLevel;
        }
    }
}