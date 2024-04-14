﻿using System;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class UnionWrapper : StatWrapper
{
    public UnionWrapper(StatusChanged onStatusChanged, List<MapleStatus.StatusType> inners) : base(new Dictionary<MapleStatus.StatusType, int>(), onStatusChanged)
    {
        InnerTypes = inners;
        artifactLevels = new Dictionary<MapleStatus.StatusType, int>();
    }

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        OnStatusChanged!.Invoke(PlayerData.StatSources.UNION, statusType, prev, next);
    }

    private Dictionary<MapleStatus.StatusType, int> artifactLevels;
    
    public List<MapleUnion.UnionBlock>? RaiderBlocks;
    public List<MapleArtifact.ArtifactPanel>? Artifacts;
    public List<MapleStatus.StatusType> InnerTypes;

    public void AddBlocks(List<MapleUnion.UnionBlock> unionBlocks)
    {
        RaiderBlocks = unionBlocks;
        foreach (var block in unionBlocks)
        {
            MapleStatus.StatusType statusType = MapleUnion.GetRaiderEffectByClass(block.classType);
            int value = MapleUnion.GetRaiderEffectValue(statusType, block.raiderRank);
            this[statusType] += value;
        }
    }

    public void AddArtifacts(List<MapleArtifact.ArtifactPanel> panels)
    {
        Artifacts = panels;
        foreach (var panel in panels)
        {
            foreach (MapleStatus.StatusType statusType in panel.StatusTypes)
            {
                artifactLevels.TryAdd(statusType, 0);
                artifactLevels[statusType] += panel.Level;
            }
        }

        foreach (var pair in artifactLevels)
        {
            this[pair.Key] = GetArtifactStatus(pair.Key, pair.Value);
        }
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