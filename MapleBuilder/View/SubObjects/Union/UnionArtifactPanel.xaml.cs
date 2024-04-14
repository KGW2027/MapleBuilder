using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.Union;

public partial class UnionArtifactPanel : UserControl
{
    private readonly List<UnionArtifactControl> artifactControls;
    private bool isInitialized;
    
    public UnionArtifactPanel()
    {
        artifactControls = new List<UnionArtifactControl>();
        
        InitializeComponent();
        foreach(var control in ctMainGrid.GetChildren<UnionArtifactControl>())
            artifactControls.Add(control);
        
        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {
        if (isInitialized) return;
        isInitialized = true;
        
        Dispatcher.BeginInvoke(() =>
        {
            var artifacts = pdata.Artifact.InitPanels;
            for (int index = 0; index < artifacts.Count; index++)
            {
                MapleArtifact.ArtifactPanel panel = artifacts[index];
                UnionArtifactControl control = artifactControls[index];

                control.ctLevelSelector.Text = $"LV {panel.Level}";
                control.ctOptionSelector1.Text = MapleArtifact.GetArtifactTypeString(panel.StatusTypes[0]);
                control.ctOptionSelector2.Text = MapleArtifact.GetArtifactTypeString(panel.StatusTypes[1]);
                control.ctOptionSelector3.Text = MapleArtifact.GetArtifactTypeString(panel.StatusTypes[2]);
            }
        });
    }

    private void OnArtifactSettingChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized || e is not UnionArtifactControl.ArtifactSettingChangedEventArgs args) return;
        if (GlobalDataController.Instance.PlayerInstance == null) return;

        if (args.IsChangedLevel)
        {
            int dLevel = args.Level - args.BeforeLevel;
            foreach (var statusType in args.StatusTypes)
                GlobalDataController.Instance.PlayerInstance.Artifact[statusType] += dLevel;
        }
        else
        {
            GlobalDataController.Instance.PlayerInstance.Artifact[args.BeforeType] -= args.Level;
            GlobalDataController.Instance.PlayerInstance.Artifact[args.NewType] += args.Level;
        }
    }
}