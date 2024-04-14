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

    public static void DispatchSummary()
    {
        return;
        selfInstance!.Dispatcher.BeginInvoke(() =>
        {
            MapleStatContainer pInfo = BuilderDataContainer.PlayerStatus!.PlayerStat;
            pInfo.Flush();

            var totalDmg = pInfo.GetPower(-140, false);
            var hMil = totalDmg / 100000000;
            var hTho = totalDmg % 100000000 / 10000;
            var tho = totalDmg % 10000;

            if (hMil > 0) selfInstance.ctDisplayPower.Content = $"{hMil:0000}억 {hTho:0000}만 {tho:0000}";
            else if (hTho > 0) selfInstance.ctDisplayPower.Content = $"{hTho:0000}만 {tho:0000}";
            else selfInstance.ctDisplayPower.Content = $"{tho:0000}";

            selfInstance.ctMainStatType.Content =
                $"주스탯 [{pInfo.MainStatType.ToString()}]";
            selfInstance.ctMainStatFlat.Content = pInfo[pInfo.MainStatType];
            selfInstance.ctMainStatRate.Content = pInfo[pInfo.MainStatType + 0x10];
            selfInstance.ctMainStatNonRateFlat.Content = pInfo[pInfo.MainStatType + 0x20];

            selfInstance.ctSymbolArcane.Content = $"{pInfo[MapleStatus.StatusType.ARCANE_FORCE]:N0}";
            selfInstance.ctSymbolAuthentic.Content = $"{pInfo[MapleStatus.StatusType.AUTHENTIC_FORCE]:N0}";

            selfInstance.ctSubStatType.Content =
                $"부스탯 [{pInfo.SubStatType.ToString()}]";
            selfInstance.ctSubStatFlat.Content = pInfo[pInfo.SubStatType];
            selfInstance.ctSubStatRate.Content = pInfo[pInfo.SubStatType + 0x10];
            selfInstance.ctSubStatNonRateFlat.Content = pInfo[pInfo.SubStatType + 0x20];

            selfInstance.ctSubStat2Grid.Visibility =
                pInfo.SubStat2Type == MapleStatus.StatusType.OTHER
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            selfInstance.ctSubStat2Type.Content =
                $"부스탯 [{pInfo.SubStat2Type.ToString()}]";
            selfInstance.ctSubStat2Flat.Content = pInfo[pInfo.SubStat2Type];
            selfInstance.ctSubStat2Rate.Content = pInfo[pInfo.SubStat2Type + 0x10];
            selfInstance.ctSubStat2NonRateFlat.Content = pInfo[pInfo.SubStat2Type + 0x20];

            selfInstance.ctBossDmg.Content = $"{pInfo[MapleStatus.StatusType.BOSS_DAMAGE]}%";
            selfInstance.ctIgnoreArmor.Content = $"{pInfo[MapleStatus.StatusType.IGNORE_DEF]:F2}%";

            selfInstance.ctCommonDmg.Content = $"{pInfo[MapleStatus.StatusType.COMMON_DAMAGE]}%";
            selfInstance.ctDropItem.Content = $"{pInfo[MapleStatus.StatusType.ITEM_DROP]}%";
            selfInstance.ctDropMeso.Content = $"{pInfo[MapleStatus.StatusType.MESO_DROP]}%";

            selfInstance.ctDmg.Content = $"{pInfo[MapleStatus.StatusType.DAMAGE]}%";
            selfInstance.ctCritChance.Content = $"{pInfo[MapleStatus.StatusType.CRITICAL_CHANCE]}%";
            selfInstance.ctCritDmg.Content = $"{pInfo[MapleStatus.StatusType.CRITICAL_DAMAGE]:F2}%";
            selfInstance.ctDurBuff.Content = $"{pInfo[MapleStatus.StatusType.BUFF_DURATION]}%";
            selfInstance.ctDurSummon.Content = $"{pInfo[MapleStatus.StatusType.SUMMON_DURATION]}%";
            selfInstance.ctDebuffDmg.Content = $"{pInfo[MapleStatus.StatusType.DEBUFF_DAMAGE]}%";
            selfInstance.ctCooldownDecrease.Content = $"{pInfo[MapleStatus.StatusType.COOL_DEC_SECOND]}초, {pInfo[MapleStatus.StatusType.COOL_DEC_RATE]}%";
            selfInstance.ctCooldownIgnore.Content = $"{pInfo[MapleStatus.StatusType.COOL_IGNORE]:F2}%";
            selfInstance.ctTolerance.Content = $"{pInfo[MapleStatus.StatusType.ABN_STATUS_RESIS]}";
            selfInstance.ctIgnoreImmune.Content = $"{pInfo[MapleStatus.StatusType.IGNORE_IMMUNE]:F2}%";

            selfInstance.ctAtkVal.Content = $"{pInfo[pInfo.AttackFlatType]:N0}";
            selfInstance.ctAtkRate.Content = $"{pInfo[pInfo.AttackRateType]}%";
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