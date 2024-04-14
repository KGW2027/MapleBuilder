using System;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class ArtifactWrapper : StatWrapper
{
    public ArtifactWrapper(List<MapleArtifact.ArtifactPanel> panels, StatusChanged onStatusChanged) : base(new Dictionary<MapleStatus.StatusType, double>(), onStatusChanged)
    {
        InitPanels = panels;
        foreach (var panel in panels)
        {
            foreach (var statusType in panel.StatusTypes)
                this[statusType] += panel.Level;
        }
    }

    public readonly List<MapleArtifact.ArtifactPanel> InitPanels;

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        double pVal = GetArtifactStatus(statusType, (int) prev);
        double nVal = GetArtifactStatus(statusType, (int) next);
        OnStatusChanged!.Invoke(PlayerData.StatSources.UNION, statusType, pVal, nVal);
    }
    
    private double GetArtifactStatus(MapleStatus.StatusType statusType, int level)
    {
        level = Math.Clamp(level, 0, 10);
        switch (statusType)
        {
            case MapleStatus.StatusType.ALL_STAT:
                return level * 15;
            case MapleStatus.StatusType.HP_AND_MP:
                return level * 750;
            case MapleStatus.StatusType.ATTACK_AND_MAGIC:
            case MapleStatus.StatusType.FINAL_ATTACK:
                return level * 3;
            case MapleStatus.StatusType.DAMAGE:
            case MapleStatus.StatusType.BOSS_DAMAGE:
                return level * 1.5;
            case MapleStatus.StatusType.IGNORE_DEF:
            case MapleStatus.StatusType.BUFF_DURATION:
            case MapleStatus.StatusType.CRITICAL_CHANCE:
            case MapleStatus.StatusType.SUMMON_DURATION:
                return level * 2;
            case MapleStatus.StatusType.COOL_IGNORE:
                return level * 0.75;
            case MapleStatus.StatusType.EXP_INCREASE:
            case MapleStatus.StatusType.MESO_DROP:
            case MapleStatus.StatusType.ITEM_DROP:
            case MapleStatus.StatusType.ABN_STATUS_RESIS:
                double rate = level;
                if (level >= 5) rate += 1;
                if (level >= 10) rate += 1;
                return rate;
            case MapleStatus.StatusType.CRITICAL_DAMAGE:
                return level * 0.4;
        }

        return 0;
    }
}