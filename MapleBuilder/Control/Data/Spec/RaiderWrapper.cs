﻿using System;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class RaiderWrapper : StatWrapper
{
    public RaiderWrapper(List<MapleUnion.UnionBlock> blocks, List<MapleStatus.StatusType> inners, StatusChanged onStatusChanged) : base(new Dictionary<MapleStatus.StatusType, int>(), onStatusChanged)
    {
        claims = new Dictionary<MapleUnion.ClaimType, int>();

        List<byte[]> initClaims = new List<byte[]>();
        foreach (var block in blocks)
        {
            foreach (sbyte[] pos in block.blockPositions)
                initClaims.Add(new[]{(byte) (pos[0]+11), (byte) Math.Abs(pos[1]-10)});
        }

        InitClaimPoses = new byte[initClaims.Count, 2];
        for (int idx = 0; idx < initClaims.Count; idx++)
        {
            InitClaimPoses[idx, 0] = initClaims[idx][0];
            InitClaimPoses[idx, 1] = initClaims[idx][1];
        }

        InnerStatus = new List<MapleStatus.StatusType>();
        for (int idx = 0; idx < inners.Count; idx++)
        {
            int ridx = (idx + 1) % inners.Count;
            InnerStatus.Add(inners[ridx]);
        }
    }

    public readonly byte[,] InitClaimPoses;
    public readonly List<MapleStatus.StatusType> InnerStatus;

    public void SwapInners(int idx1, int idx2)
    {
        Console.WriteLine($"Swap {InnerStatus[idx1]} with {InnerStatus[idx2]}");
        (InnerStatus[idx1], InnerStatus[idx2]) = (InnerStatus[idx2], InnerStatus[idx1]);
    }

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        OnStatusChanged!.Invoke(PlayerData.StatSources.UNION, statusType, prev, next);
    }

    private readonly Dictionary<MapleUnion.ClaimType, int> claims;

    public int this[MapleUnion.ClaimType claim]
    {
        get => claims.GetValueOrDefault(claim, 0);
        set
        {
            claims.TryAdd(claim, 0);
            int prev = claims[claim];
            claims[claim] = value;
            ChangeClaimCount(claim, prev, value);
        }
    }

    private void ChangeClaimCount(MapleUnion.ClaimType claimType, int prev, int next)
    {
        MapleStatus.StatusType statusType = MapleUnion.GetStatusTypeByClaimType(claimType, InnerStatus);
        double multiplier = GetClaimMultiplier(statusType);
        Console.WriteLine($"Claim Changed {statusType} from {prev} to {next}");
        CallStatusChanged(statusType, prev * multiplier, next * multiplier);
    }
    
    private double GetClaimMultiplier(MapleStatus.StatusType statusType)
    {
        return statusType switch
        {
            MapleStatus.StatusType.STR => 5,
            MapleStatus.StatusType.DEX => 5,
            MapleStatus.StatusType.INT => 5,
            MapleStatus.StatusType.LUK => 5,
            MapleStatus.StatusType.MP => 250,
            MapleStatus.StatusType.HP => 250,
            MapleStatus.StatusType.EXP_INCREASE => 0.25,
            MapleStatus.StatusType.CRITICAL_DAMAGE => 0.5,
            _ => 1
        };
    }

    

}