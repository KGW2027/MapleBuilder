using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.StatSymbol;

public partial class SymbolPanel : UserControl
{

    private bool executeFirstOnly;
    
    public SymbolPanel()
    {
        InitializeComponent();
        WzDatabase.OnWzDataLoadCompleted += OnWzDataLoadCompleted;
        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {
        Dispatcher.BeginInvoke(() =>
        {
            int arc = (int) pdata[PlayerData.StatSources.SYMBOL, MapleStatus.StatusType.ARCANE_FORCE];
            int aut = (int) pdata[PlayerData.StatSources.SYMBOL, MapleStatus.StatusType.AUTHENTIC_FORCE];
            
            bool isXenon = pdata.Class == MapleClass.ClassType.XENON;
            bool isAvenger = pdata.Class == MapleClass.ClassType.DEMON_AVENGER;
            int arcStat = (int) Math.Floor(arc * (isXenon ? 4.8 : isAvenger ? 210 : 10));
            int autStat = (int) ((isXenon ? pdata[PlayerData.StatSources.SYMBOL, MapleStatus.StatusType.STR_FLAT]
                : isAvenger ? pdata[PlayerData.StatSources.SYMBOL, MapleStatus.StatusType.HP]
                : pdata[PlayerData.StatSources.SYMBOL, pdata.AffectTypes[0]+0x20]) - arcStat);
            string prefix = isXenon ? "STR, DEX, LUK +" : isAvenger ? "MAX HP +" : $"{pdata.AffectTypes[0]} +";

            ArcaneDisplay.Content = $"ARC +{arc:N0}\n{prefix}{arcStat:N0}";
            AuthenticDisplay.Content = $"AUT +{aut:N0}\n{prefix}{autStat:N0}";
            
            if (executeFirstOnly) return;
            executeFirstOnly = true;

            foreach (var s in Arcanes.Children)
            {
                if (s is not SymbolSlot slot) continue;
                slot.Level = pdata[slot.Symbol];
            }
            foreach (var s in Authentics.Children)
            {
                if (s is not SymbolSlot slot) continue;
                slot.Level = pdata[slot.Symbol];
            }
        });
    }

    private void OnWzDataLoadCompleted(WzDatabase database)
    {
        WzDatabase.OnWzDataLoadCompleted -= OnWzDataLoadCompleted;
        Dispatcher.BeginInvoke(() =>
        {
            foreach (var e in Enum.GetValues(typeof(MapleSymbol.SymbolType)))
            {
                MapleSymbol.SymbolType symbol = (MapleSymbol.SymbolType) e;
                if (symbol == MapleSymbol.SymbolType.UNKNOWN) continue;

                SymbolSlot slot = new SymbolSlot
                {
                    Symbol = symbol,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                if (((int) symbol) < 0x10) Arcanes.Children.Add(slot);
                else Authentics.Children.Add(slot);
            }
        });
    }
}