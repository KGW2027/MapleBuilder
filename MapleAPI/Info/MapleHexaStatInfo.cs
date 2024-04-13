using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MapleHexaStatInfo : MapleInfo
{
    public MapleHexaStatInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        HexaStatLevels = new MapleHexaStatus.HexaStatus();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.HEXA_STAT;
    }

    public MapleHexaStatus.HexaStatus HexaStatLevels;

    private protected override void ParseInfo()
    {
        if (!TryGet("character_hexa_stat_core/0", out JsonObject? obj) || obj == null) return;
        
        string[] keys = {"main_stat_name", "sub_stat_name_1", "sub_stat_name_2"};
        string[] values = {"main_stat_level", "sub_stat_level_1", "sub_stat_level_2"};
        KeyValuePair<MapleStatus.StatusType, int>[] newPairs = new KeyValuePair<MapleStatus.StatusType, int>[3];
        for (int idx = 0; idx <= 2; idx++)
        {
            MapleStatus.StatusType statusType =
                MapleHexaStatus.GetStatusTypeFromHexaStatus(obj[keys[idx]]!.ToString());
            if (statusType == MapleStatus.StatusType.OTHER ||
                !int.TryParse(obj[values[idx]]!.ToString(), out int statLevel)) continue;
            newPairs[idx] = KeyValuePair.Create(statusType, statLevel);
        }

        HexaStatLevels = new MapleHexaStatus.HexaStatus
        {
            MainStat = newPairs[0],
            SubStat1 = newPairs[1],
            SubStat2 = newPairs[2]
        };
    }
}