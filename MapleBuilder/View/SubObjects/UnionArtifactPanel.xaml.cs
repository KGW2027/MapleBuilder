using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects;

public partial class UnionArtifactPanel : UserControl
{
    private static UnionArtifactPanel? selfInstance;

    public static void UpdateArtifactPanel(List<MapleArtifact.ArtifactPanel> artifacts)
    {
        selfInstance!.Dispatcher.BeginInvoke(() =>
        {
            selfInstance.artifactLevels.Clear();

            for (int index = 0; index < artifacts.Count; index++)
            {
                MapleArtifact.ArtifactPanel panel = artifacts[index];
                UnionArtifactControl control = selfInstance.artifactControls[index];

                control.ctLevelSelector.Text = $"LV {panel.Level}";
                control.ctOptionSelector1.Text = MapleArtifact.GetArtifactTypeString(panel.StatusTypes[0]);
                control.ctOptionSelector2.Text = MapleArtifact.GetArtifactTypeString(panel.StatusTypes[1]);
                control.ctOptionSelector3.Text = MapleArtifact.GetArtifactTypeString(panel.StatusTypes[2]);
                
                selfInstance.SetArtifactLevel(panel.StatusTypes[0], panel.Level);
                selfInstance.SetArtifactLevel(panel.StatusTypes[1], panel.Level);
                selfInstance.SetArtifactLevel(panel.StatusTypes[2], panel.Level);
            }
        });
    }
    
    private readonly Dictionary<MapleStatus.StatusType, int> artifactLevels;
    private readonly List<UnionArtifactControl> artifactControls;
    
    public UnionArtifactPanel()
    {
        artifactLevels = new Dictionary<MapleStatus.StatusType, int>();
        artifactControls = new List<UnionArtifactControl>();
        selfInstance = this;
        
        InitializeComponent();

        foreach(var control in ctMainGrid.GetChildren<UnionArtifactControl>())
            artifactControls.Add(control);
    }

    private double GetDeltaStatus(MapleStatus.StatusType statusType, int level)
    {
        level = Math.Clamp(level, 0, 10);
        switch (statusType)
        {
            case MapleStatus.StatusType.ALL_STAT:
                return level * 15;
            case MapleStatus.StatusType.HP_AND_MP:
                return level * 750;
            case MapleStatus.StatusType.ATTACK_AND_MAGIC:
            case MapleStatus.StatusType.FINAL_ATTACK:
                return level * 3;
            case MapleStatus.StatusType.DAMAGE:
            case MapleStatus.StatusType.BOSS_DAMAGE:
                return level * 1.5;
            case MapleStatus.StatusType.IGNORE_DEF:
            case MapleStatus.StatusType.BUFF_DURATION:
            case MapleStatus.StatusType.CRITICAL_CHANCE:
            case MapleStatus.StatusType.SUMMON_DURATION:
                return level * 2;
            case MapleStatus.StatusType.COOL_IGNORE:
                return level * 0.75;
            case MapleStatus.StatusType.EXP_INCREASE:
            case MapleStatus.StatusType.MESO_DROP:
            case MapleStatus.StatusType.ITEM_DROP:
            case MapleStatus.StatusType.ABN_STATUS_RESIS:
                double rate = level;
                if (level >= 5) rate += 1;
                if (level >= 10) rate += 1;
                return rate;
            case MapleStatus.StatusType.CRITICAL_DAMAGE:
                return level * 0.4;
        }

        return 0;
    }

    private void SetArtifactLevel(MapleStatus.StatusType type, int deltaLevel)
    {
        artifactLevels.TryAdd(type, 0);
        int prevLevel = artifactLevels[type];
        artifactLevels[type] = prevLevel + deltaLevel;

        if (BuilderDataContainer.PlayerStatus == null) return;
        BuilderDataContainer.PlayerStatus.PlayerStat[type] +=
            GetDeltaStatus(type, artifactLevels[type]) - GetDeltaStatus(type, prevLevel);
    }

    private void OnArtifactSettingChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized || e is not UnionArtifactControl.ArtifactSettingChangedEventArgs args) return;

        if (args.IsChangedLevel)
        {
            int deltaLevel = args.Level - args.BeforeLevel;
            foreach (MapleStatus.StatusType type in args.StatusTypes)
                SetArtifactLevel(type, deltaLevel);
        }
        else
        {
            SetArtifactLevel(args.BeforeType, -args.Level);
            SetArtifactLevel(args.NewType, args.Level);
        }
        
        if (BuilderDataContainer.PlayerStatus == null) return;
        BuilderDataContainer.PlayerStatus.PlayerStat.Flush();
        BuilderDataContainer.RefreshAll();
        
        
    }
}