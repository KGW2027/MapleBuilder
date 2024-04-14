using System;
using System.Collections;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public abstract class StatWrapper : IEnumerable<KeyValuePair<MapleStatus.StatusType, double>>
{
    protected readonly Dictionary<MapleStatus.StatusType, double> WrappedDict;

    public delegate void StatusChanged(PlayerData.StatSources source, MapleStatus.StatusType type, double prev, double next);
    protected readonly StatusChanged? OnStatusChanged;

    protected StatWrapper(Dictionary<MapleStatus.StatusType, double> dict, StatusChanged onStatusChanged)
    {
        OnStatusChanged += onStatusChanged;
        WrappedDict = new Dictionary<MapleStatus.StatusType, double>();

        foreach (var pair in dict)
            this[pair.Key] = pair.Value;
    }

    protected StatWrapper(Dictionary<MapleStatus.StatusType, int> dict, StatusChanged onStatusChanged)
    {
        OnStatusChanged += onStatusChanged;
        WrappedDict = new Dictionary<MapleStatus.StatusType, double>();

        foreach (var pair in dict)
            this[pair.Key] = pair.Value;
    }

    protected abstract void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next);

    public double this[MapleStatus.StatusType type]
    {
        get => WrappedDict.GetValueOrDefault(type, 0.0);
        set
        {
            WrappedDict.TryAdd(type, 0);
            double prev = WrappedDict[type];
            WrappedDict[type] = value;
            CallStatusChanged(type, prev, value);
        }
    }

    public IEnumerator<KeyValuePair<MapleStatus.StatusType, double>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<MapleStatus.StatusType, double>>) WrappedDict).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}