using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubFrames;

public partial class RenderFrame : UserControl
{
    private int lastStage = -1;
    private bool isReady = false;
    
    public RenderFrame()
    {
        InitializeComponent();
        WaitReadyWithDisplayHelp();
    }

    private void DisplayHelps(int stage)
    {
        ctSeq1Title.Visibility = Visibility.Collapsed;
        ctSeq1Desc.Visibility = Visibility.Collapsed;
        ctSeq2Title.Visibility = Visibility.Collapsed;
        ctSeq2Desc.Visibility = Visibility.Collapsed;
        ctSeq3Title.Visibility = Visibility.Collapsed;
        ctSeq3Desc.Visibility = Visibility.Collapsed;

        if (stage == 1)
        {
            ctSeq1Title.Visibility = Visibility.Visible;
            ctSeq1Desc.Visibility = Visibility.Visible;
        }
        else if (stage == 2)
        {
            ctSeq2Title.Visibility = Visibility.Visible;
            ctSeq2Desc.Visibility = Visibility.Visible;
        }
        else if (stage == 3)
        {
            ctSeq3Title.Visibility = Visibility.Visible;
            ctSeq3Desc.Visibility = Visibility.Visible;
        }

        lastStage = stage;
    }

    private async void WaitReadyWithDisplayHelp()
    {
        await Task.Delay(1000);
        int stage = TitleBar.PreSettingStage + (Summarize.IsSearchSuccess ? 1 : 0) + (ResourceManager.BaseResourceReady ? 1 : 0);
        if (stage != lastStage)
            DisplayHelps(stage);
        if (stage < 4) WaitReadyWithDisplayHelp();
        isReady = true;
    }
}