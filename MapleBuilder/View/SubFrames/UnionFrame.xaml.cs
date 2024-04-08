using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MapleAPI.Enum;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.View.SubFrames;

public partial class UnionFrame : UserControl
{
    private static UnionFrame? selfInstance;
    private Dictionary<MapleClass.ClassType, UnionCharacter> UnionSlots;

    public static void UpdateUnionRank(MapleClass.ClassType classType, MapleUnion.RaiderRank rank)
    {
        selfInstance?.Dispatcher.BeginInvoke(() =>
        {
            selfInstance.UnionSlots[classType].ctRaiderRankBox.SelectedItem = rank;
        });
    }
    
    public UnionFrame()
    {
        selfInstance = this;
        InitializeComponent();

        UnionSlots = new Dictionary<MapleClass.ClassType, UnionCharacter>();
        foreach (MapleClass.ClassType ct in Enum.GetValues<MapleClass.ClassType>())
        {
            UnionCharacter unionSlot = new UnionCharacter
            {
                TargetClass = ct,
                Margin = new Thickness(0, 8, 0, 0)
            };
            ctUnionChars.Children.Add(unionSlot);
            UnionSlots.Add(ct, unionSlot);
        }
    }
}