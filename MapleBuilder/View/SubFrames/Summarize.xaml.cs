using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
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
    
    public static void StartProgressBar()
    {
        selfInstance!.Dispatcher.Invoke(() =>
        {
            selfInstance.ctProgress.Visibility = Visibility.Visible;
            selfInstance.ctProgressLabel.Visibility = Visibility.Visible;
            selfInstance.ctProgress.Value = 0;
        });
    }

    public static void UpdateProgressBar(int current, int max)
    {
        if (selfInstance is not {IsInitialized: true}) return;
        selfInstance!.Dispatcher.Invoke(() =>
        {
            selfInstance.ctProgress.Value = current * 100.0 / max;
            selfInstance.ctProgressLabel.Content = $"( {current:N0} / {max:N0} )";

        });
    }

    public static void FinishProgressBar()
    {
        if (selfInstance is not {IsInitialized: true}) return;
        selfInstance!.Dispatcher.Invoke(() =>
        {
            selfInstance.ctProgress.Visibility = Visibility.Collapsed;
            selfInstance.ctProgressLabel.Visibility = Visibility.Collapsed;
        });
    }
    
    private void OnDataUpdated(PlayerData pdata)
    {
        MapleStatContainer dp = pdata.GetStatus();
        MapleStatus.StatusType[] status = pdata.AffectTypes;
        Dispatcher.BeginInvoke(() =>
        {
            ctMainStatType.Content =
                $"주스탯 [{status[0]}]";
            ctMainStatFlat.Content = dp[status[0]];
            ctMainStatRate.Content = dp[status[0] + 0x10];
            ctMainStatNonRateFlat.Content = dp[status[0] + 0x20];

            if (status[1] != MapleStatus.StatusType.OTHER)
            {
                ctSubStat1Grid.Visibility = Visibility.Visible;
                ctSubStat1Type.Content = $"부스탯 [{status[1]}]";
                ctSubStat1Flat.Content = dp[status[1]];
                ctSubStat1Rate.Content = dp[status[1] + 0x10];
                ctSubStat1NonRateFlat.Content = dp[status[1] + 0x20];
            } else ctSubStat1Grid.Visibility = Visibility.Collapsed;
            
            if (status[2] != MapleStatus.StatusType.OTHER)
            {
                ctSubStat2Grid.Visibility = Visibility.Visible;
                ctSubStat2Type.Content = $"부스탯 [{status[2]}]";
                ctSubStat2Flat.Content = dp[status[2]];
                ctSubStat2Rate.Content = dp[status[2] + 0x10];
                ctSubStat2NonRateFlat.Content = dp[status[2] + 0x20];
            } else ctSubStat2Grid.Visibility = Visibility.Collapsed;

            ctSymbolArcane.Content = $"{dp[MapleStatus.StatusType.ARCANE_FORCE]:N0}";
            ctSymbolAuthentic.Content = $"{dp[MapleStatus.StatusType.AUTHENTIC_FORCE]:N0}";

            ctBossDmg.Content = $"{dp[MapleStatus.StatusType.BOSS_DAMAGE]}%";
            ctIgnoreArmor.Content = $"{dp[MapleStatus.StatusType.IGNORE_DEF]:F2}%";
            ctCommonDmg.Content = $"{dp[MapleStatus.StatusType.COMMON_DAMAGE]}%";
            ctDropItem.Content = $"{dp[MapleStatus.StatusType.ITEM_DROP]}%";
            ctDropMeso.Content = $"{dp[MapleStatus.StatusType.MESO_DROP]}%";

            ctDmg.Content = $"{dp[MapleStatus.StatusType.DAMAGE]}%";
            ctCritChance.Content = $"{dp[MapleStatus.StatusType.CRITICAL_CHANCE]}%";
            ctCritDmg.Content = $"{dp[MapleStatus.StatusType.CRITICAL_DAMAGE]}%";
            ctDurBuff.Content = $"{dp[MapleStatus.StatusType.BUFF_DURATION]}%";
            ctDurSummon.Content = $"{dp[MapleStatus.StatusType.SUMMON_DURATION]}%";
            ctDebuffDmg.Content = $"{dp[MapleStatus.StatusType.DEBUFF_DAMAGE]}%";
            ctCooldownDecrease.Content = $"{dp[MapleStatus.StatusType.COOL_DEC_SECOND]}초, {dp[MapleStatus.StatusType.COOL_DEC_RATE]}%";
            ctCooldownIgnore.Content = $"{dp[MapleStatus.StatusType.COOL_IGNORE]:F2}%";
            ctTolerance.Content = $"{dp[MapleStatus.StatusType.ABN_STATUS_RESIS]}";
            ctIgnoreImmune.Content = $"{dp[MapleStatus.StatusType.IGNORE_IMMUNE]:F2}%";

            ctAtkLabel.Content = dp.AttackFlatType == MapleStatus.StatusType.ATTACK_POWER ? "공격력" : "마력";
            ctAtkrLabel.Content = dp.AttackRateType == MapleStatus.StatusType.ATTACK_RATE ? "공격력%" : "마력%";
            ctAtkVal.Content = $"{dp[dp.AttackFlatType]:N0}";
            ctAtkRate.Content = $"{dp[dp.AttackRateType]}%";

            ulong power = dp.GetPower();
            ulong top = power / 100_000_000;
            ulong mid = power % 100_000_000 / 10000;
            ulong bot = power % 10000;

            if (top > 0) ctDisplayPower.Content = $"{top}억 {mid:0000}만 {bot:0000}";
            else if(mid > 0) ctDisplayPower.Content = $"{mid}만 {bot:0000}";
            else ctDisplayPower.Content = $"{bot}";
        });
    }
    
    public Summarize()
    {
        selfInstance = this;
        
        InitializeComponent();
        ctLoadBtn.Visibility = Visibility.Collapsed;
        FinishProgressBar();
        
        GlobalDataController.OnDataUpdated += OnDataUpdated;
        WzDatabase.OnWzDataLoadCompleted += OnWzLoadCompleted;
    }

    private void OnWzLoadCompleted(WzDatabase database)
    {
        Dispatcher.Invoke(() => ctLoadBtn.Visibility = Visibility.Visible);
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
            // ResourceManager.GetCharacterInfo(lastOcid);
            GlobalDataController.Instance.LoadPlayerData(lastOcid);
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