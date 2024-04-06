using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MapleBuilder.View.SubFrames;

public partial class RenderFrame : UserControl
{
    private int lastStage = -1;
    public bool isReady;
    
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
        else ctFrameOverview.Visibility = Visibility.Visible;
    }
}