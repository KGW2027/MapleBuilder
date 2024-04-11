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
            MapleStatContainer pInfo = BuilderDataContainer.PlayerStatus!.PlayerStat;
            pInfo.Flush();
            MapleStatus.StatusType atkFlat = pInfo.mainStatType == MapleStatus.StatusType.INT
                ? MapleStatus.StatusType.MAGIC_POWER
                : MapleStatus.StatusType.ATTACK_POWER;
            MapleStatus.StatusType atkRate = pInfo.mainStatType == MapleStatus.StatusType.INT
                ? MapleStatus.StatusType.MAGIC_RATE
                : MapleStatus.StatusType.ATTACK_RATE;
            
            int mainStat = (int) Math.Floor(pInfo[pInfo.mainStatType] * (1 + pInfo[pInfo.mainStatType+0x10] / 100.0) + pInfo[pInfo.mainStatType+0x20]);
            int subStat  = (int) Math.Floor(pInfo[pInfo.subStatType ] * (1 + pInfo[pInfo.subStatType +0x10] / 100.0) + pInfo[pInfo.subStatType +0x20]);
            int subStat2 = pInfo.subStat2Type == MapleStatus.StatusType.OTHER ? 0 
                : (int) Math.Floor(pInfo[pInfo.subStat2Type] * (1 +pInfo[pInfo.subStat2Type + 0x10] / 100.0) + pInfo[pInfo.subStat2Type + 0x20]);
            double attackRate = 1 + pInfo[atkRate] / 100.0;
            double dmg = 1 + (pInfo[MapleStatus.StatusType.DAMAGE] + pInfo[MapleStatus.StatusType.BOSS_DAMAGE]) / 100.0;
            double critDmg = 1.35 + pInfo[MapleStatus.StatusType.CRITICAL_DAMAGE] / 100.0;
            
            var totalDmg = (long) Math.Round((mainStat * 4 + subStat + subStat2) * 0.01 * (pInfo[atkFlat] * attackRate) * dmg * critDmg, 0);
            var hMil = totalDmg / 100000000;
            var hTho = totalDmg % 100000000 / 10000;
            var tho = totalDmg % 10000;

            if (hMil > 0) selfInstance.ctDisplayPower.Content = $"{hMil:0000}억 {hTho:0000}만 {tho:0000}";
            else if (hTho > 0) selfInstance.ctDisplayPower.Content = $"{hTho:0000}만 {tho:0000}";
            else selfInstance.ctDisplayPower.Content = $"{tho:0000}";

            selfInstance.ctMainStatType.Content =
                $"주스탯 [{pInfo.mainStatType.ToString()}]";
            selfInstance.ctMainStatFlat.Content = pInfo[pInfo.mainStatType];
            selfInstance.ctMainStatRate.Content = pInfo[pInfo.mainStatType + 0x10];
            selfInstance.ctMainStatNonRateFlat.Content = pInfo[pInfo.mainStatType + 0x20];

            selfInstance.ctSymbolArcane.Content = $"{pInfo[MapleStatus.StatusType.ARCANE_FORCE]:N0}";
            selfInstance.ctSymbolAuthentic.Content = $"{pInfo[MapleStatus.StatusType.AUTHENTIC_FORCE]:N0}";

            selfInstance.ctSubStatType.Content =
                $"부스탯 [{pInfo.subStatType.ToString()}]";
            selfInstance.ctSubStatFlat.Content = pInfo[pInfo.subStatType];
            selfInstance.ctSubStatRate.Content = pInfo[pInfo.subStatType + 0x10];
            selfInstance.ctSubStatNonRateFlat.Content = pInfo[pInfo.subStatType + 0x20];

            selfInstance.ctSubStat2Grid.Visibility =
                pInfo.subStat2Type == MapleStatus.StatusType.OTHER
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            selfInstance.ctSubStat2Type.Content =
                $"부스탯 [{pInfo.subStat2Type.ToString()}]";
            selfInstance.ctSubStat2Flat.Content = pInfo[pInfo.subStat2Type];
            selfInstance.ctSubStat2Rate.Content = pInfo[pInfo.subStat2Type + 0x10];
            selfInstance.ctSubStat2NonRateFlat.Content = pInfo[pInfo.subStat2Type + 0x20];

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

            selfInstance.ctAtkVal.Content = $"{pInfo[atkFlat]:N0}";
            selfInstance.ctAtkRate.Content = $"{pInfo[atkRate]}%";
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