using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MaplePotentialOption = MapleAPI.Enum.MaplePotentialOption;

namespace MapleBuilder.View.SubFrames;

#pragma warning disable CS4014

public partial class Summarize : UserControl
{
    private static Summarize? selfInstance;

    private string? lastOcid;

    public static void EnableNicknameInput()
    {
        selfInstance!.ctInputNickname.Focusable = true;
        selfInstance.ctInputNickname.IsReadOnly = false;
    }

    public static void VisibleLoadButton()
    {
        selfInstance!.Dispatcher.Invoke(() =>
        {
            selfInstance.ctLoadBtn.Visibility = Visibility.Visible;
        });
    }

    public static void DispatchSummary()
    {   
        Dictionary<MapleSymbol.SymbolType, int> symbols = BuilderDataContainer.PlayerStatus!.LastSymbols;
        int arcane = 0, authentic = 0;
        foreach (var pair in symbols)
        {
            switch (pair.Key)
            {
                case MapleSymbol.SymbolType.YEORO:
                case MapleSymbol.SymbolType.CHUCHU:
                case MapleSymbol.SymbolType.LACHELEIN:
                case MapleSymbol.SymbolType.ARCANA:
                case MapleSymbol.SymbolType.MORAS:
                case MapleSymbol.SymbolType.ESFERA:
                    arcane += (pair.Value + 2) * 10;
                    break;
                case MapleSymbol.SymbolType.CERNIUM:
                case MapleSymbol.SymbolType.ARCS:
                case MapleSymbol.SymbolType.ODIUM:
                case MapleSymbol.SymbolType.DOWONKYUNG:
                case MapleSymbol.SymbolType.ARTERIA:
                case MapleSymbol.SymbolType.CARCION:
                    authentic += pair.Value * 10;
                    break;
                case MapleSymbol.SymbolType.UNKNOWN:
                default:
                    break;
            }
        }
        selfInstance!.Dispatcher.BeginInvoke(() =>
        {
            PlayerInfo pInfo = BuilderDataContainer.PlayerStatus;
            int mainStat = (int) Math.Floor(pInfo.MainStat.BaseValue * (1 + pInfo.MainStat.RateValue / 100.0)) + pInfo.MainStat.FlatValue;
            int subStat  = (int) Math.Floor(pInfo.SubStat.BaseValue * (1 + pInfo.SubStat.RateValue / 100.0)) + pInfo.SubStat.FlatValue;
            int subStat2 = pInfo.SubStat2.Stat == MaplePotentialOption.OptionType.OTHER 
                ? 0 
                : (int) Math.Floor(pInfo.SubStat2.BaseValue * (1 + pInfo.SubStat2.RateValue / 100.0)) + pInfo.SubStat2.FlatValue;
            double attackRate = 1 + pInfo.AttackRate / 100.0;
            double dmg = 1 + (pInfo.Damage + pInfo.BossDamage) / 100.0;
            double critDmg = 1.35 + pInfo.CriticalDamage / 100.0;
            
            var totalDmg = (long) Math.Round((mainStat * 4 + subStat + subStat2) * 0.01 * (pInfo.AttackValue * attackRate) * dmg * critDmg, 0);
            var hMil = totalDmg / 100000000;
            var hTho = totalDmg % 100000000 / 10000;
            var tho = totalDmg % 10000;

            if (hMil > 0) selfInstance.ctDisplayPower.Content = $"{hMil:0000}억 {hTho:0000}만 {tho:0000}";
            else if (hTho > 0) selfInstance.ctDisplayPower.Content = $"{hTho:0000}만 {tho:0000}";
            else selfInstance.ctDisplayPower.Content = $"{tho:0000}";

            selfInstance.ctMainStatType.Content =
                $"주스탯 [{pInfo!.MainStat.Stat.ToString()}]";
            selfInstance.ctMainStatFlat.Content = pInfo!.MainStat.BaseValue;
            selfInstance.ctMainStatRate.Content = pInfo.MainStat.RateValue;
            selfInstance.ctMainStatNonRateFlat.Content = pInfo.MainStat.FlatValue;

            selfInstance.ctSymbolArcane.Content = $"{arcane:N0}";
            selfInstance.ctSymbolAuthentic.Content = $"{authentic:N0}";

            selfInstance.ctSubStatType.Content =
                $"부스탯 [{pInfo.SubStat.Stat.ToString()}]";
            selfInstance.ctSubStatFlat.Content = pInfo.SubStat.BaseValue;
            selfInstance.ctSubStatRate.Content = pInfo.SubStat.RateValue;
            selfInstance.ctSubStatNonRateFlat.Content = pInfo.SubStat.FlatValue;

            selfInstance.ctSubStat2Grid.Visibility =
                pInfo.SubStat2.Stat == MaplePotentialOption.OptionType.OTHER
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            selfInstance.ctSubStat2Type.Content =
                $"부스탯 [{pInfo.SubStat2.Stat.ToString()}]";
            selfInstance.ctSubStat2Flat.Content = pInfo.SubStat2.BaseValue;
            selfInstance.ctSubStat2Rate.Content = pInfo.SubStat2.RateValue;
            selfInstance.ctSubStat2NonRateFlat.Content = pInfo.SubStat2.FlatValue;

            selfInstance.ctBossDmg.Content = $"{pInfo.BossDamage}%";
            selfInstance.ctIgnoreArmor.Content = $"{pInfo.IgnoreArmor:F2}%";

            selfInstance.ctCommonDmg.Content = $"{pInfo.CommonDamage}%";
            selfInstance.ctDropItem.Content = $"{pInfo.ItemDropIncrease}%";
            selfInstance.ctDropMeso.Content = $"{pInfo.MesoDropIncrease}%";

            selfInstance.ctDmg.Content = $"{pInfo.Damage}%";
            selfInstance.ctCritChance.Content = $"{pInfo.CriticalChance}%";
            selfInstance.ctCritDmg.Content = $"{pInfo.CriticalDamage:F2}%";
            selfInstance.ctDurBuff.Content = $"{pInfo.BuffDurationIncrease}%";
            selfInstance.ctDurSummon.Content = $"{pInfo.SummonDurationIncrease}%";
            selfInstance.ctDebuffDmg.Content = $"{pInfo.DebuffDamage}%";
            selfInstance.ctCooldownDecrease.Content = $"{pInfo.CooldownDecreaseValue}초, {pInfo.CooldownDecreaseRate}%";
            selfInstance.ctCooldownIgnore.Content = $"{pInfo.CooldownIgnoreRate:F2}%";
            selfInstance.ctTolerance.Content = $"{pInfo.Immune}%";
            selfInstance.ctIgnoreImmune.Content = $"{pInfo.IgnoreImmune:F2}%";

            selfInstance.ctAtkVal.Content = $"{pInfo.AttackValue:N0}";
            selfInstance.ctAtkRate.Content = $"{pInfo.AttackRate}%";
        });
    }
    
    public Summarize()
    {
        selfInstance = this;
        
        InitializeComponent();
        ctLoadBtn.Visibility = Visibility.Collapsed;
    }

    public static bool IsLoadComplete {get; private set;}
    
    private void TrySearch(object sender, RoutedEventArgs e)
    {
        if (ctInputNickname.IsReadOnly) return;
        Dictionary<string, string> res = ResourceManager.RequestCharBasic(ctInputNickname.Text.Trim(), out var ocid);
        lastOcid = ocid;
        
        if (res.TryGetValue("error", out string? errMsg))
        {
            ctDisplayServer.Visibility = Visibility.Visible;
            ctDisplayClass.Visibility = Visibility.Collapsed;
            ctDisplayLevel.Visibility = Visibility.Collapsed;
            ctDisplayServer.Content = errMsg;
            return;
        }
        
        UpdateCharacterImage(res["character_image"]);
        ctDisplayServer.Visibility = Visibility.Visible;
        ctDisplayServer.Content = res["world_name"];
        ctDisplayLevel.Visibility = Visibility.Visible;
        ctDisplayLevel.Content = $"Lv. {res["character_level"]}";
        ctDisplayClass.Visibility = Visibility.Visible;
        ctDisplayClass.Content = res["character_class"];
        IsLoadComplete = false;
    }

    private async void UpdateCharacterImage(string url)
    {
        BitmapImage? image = await ResourceManager.LoadPngFromWebUrl(url);
        if (image == null) return;
        Dispatcher.BeginInvoke(() =>
        {
            ctDisplayCharImage.Source = image;
        });

    }

    private void LoadData(object sender, RoutedEventArgs e)
    {
        if (lastOcid == null || IsLoadComplete) return;
        new Thread(() =>
        {
            ResourceManager.GetCharacterInfo(lastOcid);
            IsLoadComplete = true;
        }).Start();
    }

    private void OnRenderScreenToEquip(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.OVERVIEW_EQUIPMENT;
    }

    private void OnRenderScreenToUnion(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.UNION;
    }

    private void OnRenderScreenToEtc(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.ETC;
    }
}