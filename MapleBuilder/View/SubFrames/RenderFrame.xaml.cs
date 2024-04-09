using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubFrames;

public partial class RenderFrame : UserControl
{
    
    public RenderFrame()
    {
        renderType = RenderScreenType.NOT_READY;
        selfInstance = this;
        InitializeComponent();

        ScreenElements = new Dictionary<RenderScreenType, List<UIElement>>
        {
            {RenderScreenType.OVERVIEW_EQUIPMENT, new List<UIElement> {ctCharacterTop, ctFrameOverview}},
            {RenderScreenType.STAT_SYMBOL, new List<UIElement> {ctCharacterTop, ctStatSymbol}},
            {RenderScreenType.SPECIAL_EQUIPS, new List<UIElement> {ctCharacterTop}},
            {RenderScreenType.CASH_EQUIPS, new List<UIElement> {ctCharacterTop}},
            {RenderScreenType.UNION, new List<UIElement> {ctUnionOverview}},
            {RenderScreenType.ETC, new List<UIElement> {}},
        };
        
        WaitReadyWithDisplayHelp();
    }
    
    public enum RenderScreenType
    {
        OVERVIEW_EQUIPMENT,
        STAT_SYMBOL,
        SPECIAL_EQUIPS,
        CASH_EQUIPS,
        UNION,
        ETC,
        NOT_READY
    }
    private static RenderFrame? selfInstance;
    private static RenderScreenType renderType;

    public static RenderScreenType RenderType
    {
        get => renderType;
        set
        {
            selfInstance!.CollapseScreen();
            renderType = value;
            selfInstance.RenderScreen();
        }
    }

    #region 화면 전환
    private Dictionary<RenderScreenType, List<UIElement>> ScreenElements;
    private void CollapseScreen()
    {
        if (!ScreenElements.TryGetValue(renderType, out var list)) return;
        Dispatcher.Invoke(() => { list.ForEach(element => element.Visibility = Visibility.Collapsed); });
    }

    private void RenderScreen()
    {
        if (!ScreenElements.TryGetValue(renderType, out var list)) return;
        Dispatcher.Invoke(() => { list.ForEach(element => element.Visibility = Visibility.Visible); });
    }
    #endregion

    #region 사전설정 이전 로딩
    private int lastStage = -1;
    private void DisplayHelps(int stage)
    {
        ctSeq1Title.Visibility = Visibility.Collapsed;
        ctSeq1Desc.Visibility = Visibility.Collapsed;
        ctSeq2Title.Visibility = Visibility.Collapsed;
        ctSeq2Desc.Visibility = Visibility.Collapsed;
        ctSeq3Title.Visibility = Visibility.Collapsed;
        ctSeq3Desc.Visibility = Visibility.Collapsed;

        switch (stage)
        {
            case 1:
                ctSeq1Title.Visibility = Visibility.Visible;
                ctSeq1Desc.Visibility = Visibility.Visible;
                break;
            case 2:
                ctSeq2Title.Visibility = Visibility.Visible;
                ctSeq2Desc.Visibility = Visibility.Visible;
                break;
            case 3:
                ctSeq3Title.Visibility = Visibility.Visible;
                ctSeq3Desc.Visibility = Visibility.Visible;
                break;
        }

        lastStage = stage;
    }

    private async void WaitReadyWithDisplayHelp()
    {
        await Task.Delay(1000);
        int stage = TitleBar.PreSettingStage + (Summarize.IsLoadComplete ? 1 : 0);
        if (stage != lastStage)
            DisplayHelps(stage);
        if (stage < 4) WaitReadyWithDisplayHelp();
        else RenderType = RenderScreenType.OVERVIEW_EQUIPMENT;
    }
    #endregion

    public static void UpdateCharacterTop()
    {
        selfInstance?.Dispatcher.BeginInvoke(() =>
        {
            var charTop = selfInstance.ctCharacterTop;
            charTop.UpdateProfileImage();
            charTop.ctCharacterName.Content = BuilderDataContainer.CharacterInfo!.UserName;
            charTop.ctCharacterGuild.Content = $"길드 {BuilderDataContainer.CharacterInfo.GuildName}";
            charTop.ctCharacterLevelAndClass.Content = $"Lv. {BuilderDataContainer.CharacterInfo.Level} {BuilderDataContainer.CharacterInfo.ClassString}";

            Dictionary<MapleSymbol.SymbolType, int> symbols = BuilderDataContainer.PlayerStatus!.lastSymbols;
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
            charTop.ctCharacterSymbol.Content = $"아케인 {arcane:N0}\n어센틱 {authentic:N0}";
        });
    }
    
}