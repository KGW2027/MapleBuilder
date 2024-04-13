using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MapleRaiderInfo : MapleInfo
{
    public MapleRaiderInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        UnionInfo = new List<MapleUnion.UnionBlock>();
        UnionInner = new List<MapleStatus.StatusType>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.UNION_RADIER;
    }

    public readonly List<MapleUnion.UnionBlock> UnionInfo;
    public readonly List<MapleStatus.StatusType> UnionInner;

    private protected override void ParseInfo()
    {
        if (!TryGet("union_block", out JsonArray? unionBlocks) || unionBlocks == null) return;
        if (!TryGet("union_inner_stat", out JsonArray? unionInners) || unionInners == null) return;

        foreach (var unionNode in unionBlocks)
        {
            if (unionNode is not JsonObject unionBlock) continue;

            MapleClass.ClassType blockClass = MapleClass.GetMapleClass(unionBlock["block_class"]!.ToString());
            MapleUnion.RaiderRank blockRank =
                MapleUnion.GetRaiderRank(int.Parse(unionBlock["block_level"]!.ToString()), blockClass);
            sbyte[][] claims = new sbyte[(int) blockRank][];
            JsonArray unionClaims = unionBlock["block_position"]!.AsArray();
            for (int idx = 0; idx < unionClaims.Count; idx++)
            {
                JsonObject claimVector = unionClaims[idx]!.AsObject();
                claims[idx] = new[]
                    {sbyte.Parse(claimVector["x"]!.ToString()), sbyte.Parse(claimVector["y"]!.ToString())};
            }

            UnionInfo.Add(new MapleUnion.UnionBlock
                {blockPositions = claims, classType = blockClass, raiderRank = blockRank});
        }

        foreach (var unionInnerNode in unionInners)
        {
            if (unionInnerNode is not JsonObject unionInner) continue;
            UnionInner.Add(MapleUnion.GetStatusTypeByUnionField(unionInner["stat_field_effect"]!.ToString()));
        }
    }
}