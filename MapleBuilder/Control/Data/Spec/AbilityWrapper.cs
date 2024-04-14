using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class AbilityWrapper : StatWrapper
{
    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        OnStatusChanged!.Invoke(PlayerData.StatSources.ABILITY, statusType, prev, next);
    }

    public AbilityWrapper(Dictionary<MapleStatus.StatusType, int> dict, StatusChanged onStatusChanged) : base(dict, onStatusChanged)
    {
    }

    public KeyValuePair<MapleStatus.StatusType, double>[] GetAbilities()
    {
        return WrappedDict.ToArray();
    }
}