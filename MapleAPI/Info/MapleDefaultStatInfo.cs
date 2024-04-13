using System.Text.Json.Nodes;
using MapleAPI.DataType;

namespace MapleAPI.Info;

public class MapleDefaultStatInfo : MapleInfo
{
    public MapleDefaultStatInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        ApStats = new int[5];
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.STAT;
    }

    public readonly int[] ApStats;

    private protected override void ParseInfo()
    {
        if (!TryGet("final_stat", out JsonArray? array) || array == null) return;
        foreach (var node in array)
        {
            if (node is not JsonObject jo) continue;
            int key = jo["stat_name"]!.ToString() switch
            {
                "AP 배분 STR" => 0,
                "AP 배분 DEX" => 1,
                "AP 배분 INT" => 2,
                "AP 배분 LUK" => 3,
                "AP 배분 HP" => 4,
                _ => -1
            };
            if (key == -1) continue;
            ApStats[key] = int.Parse(jo["stat_value"]!.ToString());
        }
    }
}