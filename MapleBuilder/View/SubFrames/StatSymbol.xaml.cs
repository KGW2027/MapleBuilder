using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
using MapleBuilder.View.SubObjects;
using MapleBuilder.View.SubObjects.Equipment;
using MapleBuilder.View.SubObjects.StatSymbol;

namespace MapleBuilder.View.SubFrames;

#pragma warning disable CS0168

public partial class StatSymbol : UserControl
{
    private readonly List<HyperStatSlot> hyperStatSlots;
    private bool isInitialized;
    
    public StatSymbol()
    {
        InitializeComponent();

        hyperStatSlots = new List<HyperStatSlot>();
        foreach(var child in ParentGrid.GetChildren<HyperStatSlot>())
            hyperStatSlots.Add(child);

        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {        
        Dispatcher.BeginInvoke(() =>
        {
            if (isInitialized) return;
            isInitialized = true;
            
            // 하이퍼스텟 레벨 바인딩
            foreach (var pair in pdata.HyperStat)
                foreach (var hSlot in hyperStatSlots.Where(hSlot => hSlot.StatType == pair.Key))
                    hSlot.Level = (int) pair.Value;
        });
    }
    
}