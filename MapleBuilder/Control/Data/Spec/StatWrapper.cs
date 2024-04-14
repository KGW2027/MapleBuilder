using System;
using System.Collections;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public abstract class StatWrapper : IEnumerable<KeyValuePair<MapleStatus.StatusType, int>>
{
    private readonly Dictionary<MapleStatus.StatusType, int> wrappedDict;

    public delegate void StatusChanged(PlayerData.StatSources source, MapleStatus.StatusType type, double prev, double next);
    public StatusChanged? OnStatusChanged;

    protected StatWrapper(Dictionary<MapleStatus.StatusType, int> dict)
    {
        wrappedDict = dict;
    }

    protected abstract void CallStatusChanged(MapleStatus.StatusType statusType, int prev, int next);

    public int this[MapleStatus.StatusType type]
    {
        get => wrappedDict.GetValueOrDefault(type, 0);
        set
        {
            wrappedDict.TryAdd(type, 0);
            int prev = wrappedDict[type];
            wrappedDict[type] = value;
            CallStatusChanged(type, prev, value);
        }
    }

    public IEnumerator<KeyValuePair<MapleStatus.StatusType, int>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<MapleStatus.StatusType, int>>) wrappedDict).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}