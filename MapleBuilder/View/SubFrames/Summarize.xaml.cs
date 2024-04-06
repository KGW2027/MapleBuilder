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
        selfInstance!.Dispatcher.BeginInvoke(() =>
        {
            selfInstance.ctMainStatType.Content =
                $"주스탯 [{BuilderDataContainer.PlayerStatus!.MainStat.Stat.ToString()}]";
            selfInstance.ctMainStatFlat.Content = BuilderDataContainer.PlayerStatus!.MainStat.BaseValue;
            selfInstance.ctMainStatRate.Content = BuilderDataContainer.PlayerStatus.MainStat.RateValue;
            selfInstance.ctMainStatNonRateFlat.Content = BuilderDataContainer.PlayerStatus.MainStat.FlatValue;
            
            selfInstance.ctSubStatType.Content =
                $"부스탯 [{BuilderDataContainer.PlayerStatus.SubStat.Stat.ToString()}]";
            selfInstance.ctSubStatFlat.Content = BuilderDataContainer.PlayerStatus.SubStat.BaseValue;
            selfInstance.ctSubStatRate.Content = BuilderDataContainer.PlayerStatus.SubStat.RateValue;
            selfInstance.ctSubStatNonRateFlat.Content = BuilderDataContainer.PlayerStatus.SubStat.FlatValue;

            selfInstance.ctSubStat2Grid.Visibility =
                BuilderDataContainer.PlayerStatus.SubStat2.Stat == MaplePotentialOption.OptionType.OTHER
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            selfInstance.ctSubStat2Type.Content =
                $"부스탯 [{BuilderDataContainer.PlayerStatus.SubStat2.Stat.ToString()}]";
            selfInstance.ctSubStat2Flat.Content = BuilderDataContainer.PlayerStatus.SubStat2.BaseValue;
            selfInstance.ctSubStat2Rate.Content = BuilderDataContainer.PlayerStatus.SubStat2.RateValue;
            selfInstance.ctSubStat2NonRateFlat.Content = BuilderDataContainer.PlayerStatus.SubStat2.FlatValue;

            selfInstance.ctBossDmg.Content = $"{BuilderDataContainer.PlayerStatus.BossDamage}%";
            selfInstance.ctIgnoreArmor.Content = $"{BuilderDataContainer.PlayerStatus.IgnoreArmor:F2}%";
            
            selfInstance.ctCommonDmg.Content = $"{BuilderDataContainer.PlayerStatus.CommonDamage}%";
            selfInstance.ctDropItem.Content = $"{BuilderDataContainer.PlayerStatus.ItemDropIncrease}%";
            selfInstance.ctDropMeso.Content = $"{BuilderDataContainer.PlayerStatus.MesoDropIncrease}%";
            
            selfInstance.ctDmg.Content = $"{BuilderDataContainer.PlayerStatus.Damage}%";
            selfInstance.ctCritChance.Content = $"{BuilderDataContainer.PlayerStatus.CriticalChance}%";
            selfInstance.ctCritDmg.Content = $"{BuilderDataContainer.PlayerStatus.CriticalDamage:F2}%";
            selfInstance.ctDurBuff.Content = $"{BuilderDataContainer.PlayerStatus.BuffDurationIncrease}%";
            selfInstance.ctDurSummon.Content = $"{BuilderDataContainer.PlayerStatus.SummonDurationIncrease}%";
            selfInstance.ctDebuffDmg.Content = $"{BuilderDataContainer.PlayerStatus.DebuffDamage}%";
            selfInstance.ctCooldownDecrease.Content = $"{BuilderDataContainer.PlayerStatus.CooldownDecreaseValue}초, {BuilderDataContainer.PlayerStatus.CooldownDecreaseRate}%";
            selfInstance.ctCooldownIgnore.Content = $"{BuilderDataContainer.PlayerStatus.CooldownIgnoreRate:F2}%";
            selfInstance.ctTolerance.Content = $"{BuilderDataContainer.PlayerStatus.Immune}%";
            selfInstance.ctIgnoreImmune.Content = $"{BuilderDataContainer.PlayerStatus.IgnoreImmune:F2}%";
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
}