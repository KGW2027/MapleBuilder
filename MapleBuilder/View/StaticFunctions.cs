﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.View;

public static class StaticFunctions
{
    public static IEnumerable<T> GetChildren<T>(this DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) yield break;

        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                yield return typedChild;
            }

            foreach (var descendant in GetChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }
    
    public static void CheckTextNumberOnly(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.Length < 1 || e.Text[0] < '0' || e.Text[0] > '9';
    }
    

    public static ulong GetPower(this MapleStatContainer container, int correctAtk, bool isGenesis)
    {
        // Stat const
        double mainStatVar = Math.Floor(container[container.MainStatType] * (100 + container[container.MainStatType + 0x10]) / 100) + container[container.MainStatType + 0x20];
        double subStatVar = Math.Floor(container[container.SubStatType] * (100 + container[container.SubStatType + 0x10]) / 100) + container[container.SubStatType + 0x20];
        if (container.SubStat2Type != MapleStatus.StatusType.OTHER)
            subStatVar += Math.Floor(container[container.SubStat2Type] * (100 + container[container.SubStat2Type + 0x10]) / 100) + container[container.SubStat2Type + 0x20];

        double stat = (mainStatVar * 4.0 + subStatVar) / 100;
        double atk = Math.Floor((container[container.AttackFlatType] + correctAtk + 30) * (100 + container[container.AttackRateType]) / 100); // TODO : 30 = 여축/정축 미리 입력(스킬 기능 추가 시 제거)
        double dam = (100 + container[MapleStatus.StatusType.DAMAGE] + container[MapleStatus.StatusType.BOSS_DAMAGE]) / 100;
        double cdam = (135 + container[MapleStatus.StatusType.CRITICAL_DAMAGE]) / 100;
        double fdam = isGenesis ? 1.1 : 1.0;

        return (ulong) Math.Floor(stat * atk * dam * cdam * fdam);
    }
}