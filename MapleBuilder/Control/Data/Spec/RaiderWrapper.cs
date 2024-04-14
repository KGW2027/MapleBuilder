using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class RaiderWrapper : StatWrapper
{
    public RaiderWrapper(List<MapleUnion.UnionBlock> blocks, List<MapleStatus.StatusType> inners, StatusChanged onStatusChanged) : base(new Dictionary<MapleStatus.StatusType, int>(), onStatusChanged)
    {
        
    }

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        OnStatusChanged!.Invoke(PlayerData.StatSources.UNION, statusType, prev, next);
    }

    

}