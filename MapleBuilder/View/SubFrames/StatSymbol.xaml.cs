﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubFrames;

public partial class StatSymbol : UserControl
{
    private static StatSymbol? selfInstnace;

    public static void Update()
    {
        selfInstnace!.Dispatcher.BeginInvoke(() =>
        {
            int arcane = 0, arcaneStat = 0, authentic = 0, authenticStat = 0;
            bool isXenon = BuilderDataContainer.CharacterInfo!.Class == MapleClass.ClassType.XENON;
            bool isAvenger = BuilderDataContainer.CharacterInfo.Class == MapleClass.ClassType.DEMON_AVENGER;
            foreach (var pair in selfInstnace.symbolLevels)
            {
                int level = BuilderDataContainer.PlayerStatus!.LastSymbols.TryGetValue(pair.Key, out int val) ? val : 0;
                ((TextBox) pair.Value).Text = level.ToString();

                if (level == 0) continue;
                
                switch (pair.Key)
                {
                    case MapleSymbol.SymbolType.YEORO:
                    case MapleSymbol.SymbolType.CHUCHU:
                    case MapleSymbol.SymbolType.LACHELEIN:
                    case MapleSymbol.SymbolType.ARCANA:
                    case MapleSymbol.SymbolType.MORAS:
                    case MapleSymbol.SymbolType.ESFERA:
                        arcane += (level + 2) * 10;
                        arcaneStat += isXenon ? 48 * (level + 2) : isAvenger ? 2100 * (level + 2) : 100 * (level + 2);
                        break;
                    case MapleSymbol.SymbolType.CERNIUM:
                    case MapleSymbol.SymbolType.ARCS:
                    case MapleSymbol.SymbolType.ODIUM:
                    case MapleSymbol.SymbolType.DOWONKYUNG:
                    case MapleSymbol.SymbolType.ARTERIA:
                    case MapleSymbol.SymbolType.CARCION:
                        authentic += level * 10;
                        authenticStat += isXenon ? 48 * (2 * level + 3) : isAvenger ? 2100 * (2 * level + 3) : 100 * (2 * level + 3);
                        break;
                    case MapleSymbol.SymbolType.UNKNOWN:
                    default:
                        break;
                }
            }

            selfInstnace.ctArcaneForceDisplay.Content = $"ARC +{arcane:N0}";
            selfInstnace.ctArcaneStatDisplay.Content = isXenon ? $"STR, DEX, LUK +{arcaneStat:N0}" :
                isAvenger ? $"MAX HP +{arcaneStat:N0}" :
                $"{BuilderDataContainer.PlayerStatus!.MainStat.Stat} +{arcaneStat:N0}";
            
            selfInstnace.ctAuthenticForceDisplay.Content = $"AUT +{authentic:N0}";
            selfInstnace.ctAuthenticStatDisplay.Content = isXenon ? $"STR, DEX, LUK +{authenticStat:N0}" :
                isAvenger ? $"MAX HP +{authenticStat:N0}" :
                $"{BuilderDataContainer.PlayerStatus!.MainStat.Stat} +{authenticStat:N0}";
        });
    }

    private Dictionary<MapleSymbol.SymbolType, UIElement> symbolLevels;
    
    public StatSymbol()
    {
        selfInstnace = this;
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

        new Thread(LoadSymbolIcons).Start();
    }

    private void LoadSymbolIcons()
    {
        while(!ResourceManager.itemIconLoaded) { }

        Dispatcher.Invoke(() =>
        {
            ctYeoroSlot.ctEditLabel.Content = "소멸의 여로";
            ctYeoroSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 소멸의 여로")!.IconRaw;

            ctChuchuSlot.ctEditLabel.Content = "츄츄 포레스트";
            ctChuchuSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 츄츄 아일랜드")!.IconRaw;

            ctLacheleinSlot.ctEditLabel.Content = "레헬른";
            ctLacheleinSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 레헬른")!.IconRaw;

            ctArcanaSlot.ctEditLabel.Content = "아르카나";
            ctArcanaSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 아르카나")!.IconRaw;

            ctMorasSlot.ctEditLabel.Content = "모라스";
            ctMorasSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 모라스")!.IconRaw;

            ctEsferaSlot.ctEditLabel.Content = "에스페라";
            ctEsferaSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 에스페라")!.IconRaw;
            
            ctCerniumSlot.ctEditLabel.Content = "세르니움";
            ctCerniumSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 세르니움")!.IconRaw;
            
            ctArcsSlot.ctEditLabel.Content = "아르크스";
            ctArcsSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 아르크스")!.IconRaw;
            
            ctOdiumSlot.ctEditLabel.Content = "오디움";
            ctOdiumSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 오디움")!.IconRaw;
            
            ctDowonkyungSlot.ctEditLabel.Content = "도원경";
            ctDowonkyungSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 도원경")!.IconRaw;
            
            ctArteriaSlot.ctEditLabel.Content = "아르테리아";
            ctArteriaSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 아르테리아")!.IconRaw;
            
            ctCarsionSlot.ctEditLabel.Content = "카르시온";
            ctCarsionSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 카르시온")!.IconRaw;
        });
    }

    private void CheckTextNumberOnly(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.Length < 1 || e.Text[0] < '0' || e.Text[0] > '9';
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            int level = int.Parse(((TextBox) sender).Text);
            string name = ((TextBox) sender).Name;
            MapleSymbol.SymbolType sType = MapleSymbol.SymbolType.UNKNOWN;

            foreach (var pair in symbolLevels)
                if (((TextBox) pair.Value).Name.Equals(name))
                    sType = pair.Key;

            if (sType == MapleSymbol.SymbolType.UNKNOWN) return;

            int maxLevel = sType < MapleSymbol.SymbolType.CERNIUM ? 20 : 11;
            if (level > maxLevel)
            {
                ((TextBox) sender).Text = maxLevel.ToString();
                return;
            }

            Dictionary<MapleSymbol.SymbolType, int> newSymbolTable = symbolLevels.ToDictionary(pair => pair.Key, pair => int.Parse(((TextBox) pair.Value).Text));
            BuilderDataContainer.PlayerStatus!.ApplySymbolData(newSymbolTable);
        }
        catch (Exception ex)
        {
            // ignored
        }
    }

    private void CtAbilitySlider1_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Console.WriteLine($"CT_SLIDER_1 VAL CHANGED : {e.NewValue}");
    }
}