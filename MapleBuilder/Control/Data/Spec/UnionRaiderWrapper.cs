using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class UnionRaiderWrapper : StatWrapper
{
    public UnionRaiderWrapper(List<MapleUnion.UnionBlock> blocks, StatusChanged onStatusChanged) : base(new Dictionary<MapleStatus.StatusType, double>(), onStatusChanged)
    {
        classRanks = new Dictionary<MapleClass.ClassType, MapleUnion.RaiderRank>();
        InitRanks = new Dictionary<MapleClass.ClassType, MapleUnion.RaiderRank>();
        foreach (var block in blocks)
            InitRanks.TryAdd(block.classType, block.raiderRank);
    }

    public readonly Dictionary<MapleClass.ClassType, MapleUnion.RaiderRank> InitRanks;
    private readonly Dictionary<MapleClass.ClassType, MapleUnion.RaiderRank> classRanks;

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        OnStatusChanged!.Invoke(PlayerData.StatSources.UNION, statusType, prev, next);
    }

    public MapleUnion.RaiderRank this[MapleClass.ClassType classType]
    {
        get => classRanks.GetValueOrDefault(classType, MapleUnion.RaiderRank.NONE);
        set
        {
            MapleStatus.StatusType statusType = MapleUnion.GetRaiderEffectByClass(classType);
            int prev = MapleUnion.GetRaiderEffectValue(statusType, this[classType]);
            classRanks.TryAdd(classType, MapleUnion.RaiderRank.NONE);
            classRanks[classType] = value;
            int next = MapleUnion.GetRaiderEffectValue(statusType, value);
            this[statusType] += (next - prev);
        }
    }
}