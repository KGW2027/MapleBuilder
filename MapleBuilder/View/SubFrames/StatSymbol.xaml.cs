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
using MapleBuilder.View.SubObjects.StatSymbol;

namespace MapleBuilder.View.SubFrames;

#pragma warning disable CS0168

public partial class StatSymbol : UserControl
{
    private readonly List<HyperStatSlot> hyperStatSlots;
    private readonly Dictionary<MapleSymbol.SymbolType, UIElement> symbolLevels;
    private bool isInitialized;
    
    public StatSymbol()
    {
        InitializeComponent();

        symbolLevels = new Dictionary<MapleSymbol.SymbolType, UIElement>
        {
            {MapleSymbol.SymbolType.YEORO, ctYeoroText},
            {MapleSymbol.SymbolType.CHUCHU, ctChuchuText},
            {MapleSymbol.SymbolType.LACHELEIN, ctLacheleinText},
            {MapleSymbol.SymbolType.ARCANA, ctArcanaText},
            {MapleSymbol.SymbolType.MORAS, ctMorasText},
            {MapleSymbol.SymbolType.ESFERA, ctEsferaText},
            {MapleSymbol.SymbolType.CERNIUM, ctCerniumText},
            {MapleSymbol.SymbolType.ARCS, ctArcsText},
            {MapleSymbol.SymbolType.ODIUM, ctOdiumText},
            {MapleSymbol.SymbolType.DOWONKYUNG, ctDowonkyungText},
            {MapleSymbol.SymbolType.ARTERIA, ctArteriaText},
            {MapleSymbol.SymbolType.CARCION, ctCarsionText},
        };

        hyperStatSlots = new List<HyperStatSlot>();
        foreach(var child in ParentGrid.GetChildren<HyperStatSlot>())
            hyperStatSlots.Add(child);

        GlobalDataController.OnDataUpdated += OnDataUpdated;
        WzDatabase.OnWzDataLoadCompleted += OnWzDataLoadCompleted;
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
            
            ctArcaneForceDisplay.Content = $"ARC +{arc:N0}";
            ctArcaneStatDisplay.Content = $"{prefix}{arcStat:N0}";
            ctAuthenticForceDisplay.Content = $"AUT +{aut:N0}";
            ctAuthenticStatDisplay.Content = $"{prefix}{autStat:N0}";

            if (isInitialized) return;
            isInitialized = true;
            
            // 심볼 레벨 바인딩
            foreach (var pair in symbolLevels)
                ((TextBox) pair.Value).Text = pdata[pair.Key].ToString();

            // 하이퍼스텟 레벨 바인딩
            foreach (var pair in pdata.HyperStat)
                foreach (var hSlot in hyperStatSlots.Where(hSlot => hSlot.StatType == pair.Key))
                    hSlot.Level = (int) pair.Value;
        });
    }

    private void OnWzDataLoadCompleted(WzDatabase database)
    {
        Dictionary<string, EquipmentSlot> symbolSlots = new Dictionary<string, EquipmentSlot>
        {
            {"소멸의 여로", ctYeoroSlot}, {"츄츄 아일랜드", ctChuchuSlot}, {"레헬른", ctLacheleinSlot},
            {"아르카나", ctArcanaSlot}, {"모라스", ctMorasSlot}, {"에스페라", ctEsferaSlot},
        
            {"세르니움", ctCerniumSlot}, {"아르크스", ctArcsSlot}, {"오디움", ctOdiumSlot},
            {"도원경", ctDowonkyungSlot}, {"아르테리아", ctArteriaSlot}, {"카르시온", ctCarsionSlot}
        };
        
        // 심볼 이미지 로드
        Dispatcher.Invoke(() =>
        {
            int idx = 0;
            foreach (var pair in symbolSlots)
            {
                string prefix = idx++ < 6 ? "아케인심볼" : "어센틱심볼";
                pair.Value.ItemLabel.Content = pair.Key;
                string itemName = $"{prefix} : {pair.Key}";
                if (!database.EquipmentDataList.TryGetValue(itemName, out var item)) continue;
                pair.Value.DisplayImage.Source = item.Image;
            }
        });
    }
    

    #region 심볼
    private void CheckTextNumberOnly(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.Length < 1 || e.Text[0] < '0' || e.Text[0] > '9';
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var pInstance = GlobalDataController.Instance.PlayerInstance;
            if (pInstance == null) return;
            
            TextBox box = (TextBox) sender;
            MapleSymbol.SymbolType boxSymbolType = MapleSymbol.SymbolType.UNKNOWN;
            foreach (var pair in symbolLevels)
            {
                if (pair.Value == box) boxSymbolType = pair.Key;
            }

            if (boxSymbolType == MapleSymbol.SymbolType.UNKNOWN) return;
            int level = int.Parse(box.Text);

            pInstance[boxSymbolType] = level;
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
    #endregion
}