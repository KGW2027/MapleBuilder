using System;
using System.Collections.Generic;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Spec;

public class HyperStatWrapper : StatWrapper
{

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        double pValue = GetValue(statusType, prev);
        double nValue = GetValue(statusType, next);
        Console.WriteLine($"[HYPERSTAT {statusType}'s Level Changed from {prev} to {next}. so OPTION changed from {pValue} to {nValue}.]");
        OnStatusChanged!.Invoke(PlayerData.StatSources.HYPER_STAT, statusType, pValue, nValue);
    }
    
    private double GetIncrease(MapleStatus.StatusType type, int level)
    {
        return type switch
        {
            MapleStatus.StatusType.STR_FLAT => 30,
            MapleStatus.StatusType.DEX_FLAT => 30,
            MapleStatus.StatusType.INT_FLAT => 30,
            MapleStatus.StatusType.LUK_FLAT => 30,
            MapleStatus.StatusType.HP_RATE => 2,
            MapleStatus.StatusType.MP_RATE => 2,
            MapleStatus.StatusType.DF_TT_PP => level <= 10 ? 10 : 0,
            MapleStatus.StatusType.CRITICAL_CHANCE => level > 5 ? 2 : 1,
            MapleStatus.StatusType.CRITICAL_DAMAGE => 1,
            MapleStatus.StatusType.IGNORE_DEF => 3,
            MapleStatus.StatusType.DAMAGE => 3,
            MapleStatus.StatusType.BOSS_DAMAGE => level > 5 ? 4 : 3,
            MapleStatus.StatusType.COMMON_DAMAGE => level > 5 ? 4 : 3,
            MapleStatus.StatusType.ABN_STATUS_RESIS => level > 5 ? 2 : 1,
            MapleStatus.StatusType.ATTACK_AND_MAGIC => 3,
            MapleStatus.StatusType.EXP_INCREASE => level > 10 ? 1 : 0.5,
            MapleStatus.StatusType.ARCANE_FORCE => level > 10 ? 10 : 5,
            _ => 0
        };
    }

    private double GetValue(MapleStatus.StatusType statusType, double level)
    {
        double v = 0.0;
        int lv = (int) level;
        for (int i = 1; i <= lv; i++)
            v += GetIncrease(statusType, i);
        return v;
    }

    public HyperStatWrapper(Dictionary<MapleStatus.StatusType, int> dict, StatusChanged onStatusChanged) : base(dict, onStatusChanged)
    {
    }
}