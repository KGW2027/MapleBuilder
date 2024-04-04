using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleBuilder.Control;
using Microsoft.Win32;

namespace MapleBuilder.View.SubFrames;

public partial class TitleBar : UserControl
{
    private static TitleBar? selfInstance;
    
    public static int PreSettingStage
    {
        get
        {
            if (selfInstance == null) return 0;
            if (selfInstance.ctWzPathButton.Visibility != Visibility.Collapsed) return 1;
            return selfInstance.ctAPIApplyButton.Visibility != Visibility.Collapsed ? 2 : 3;
        }
    }
    
    public TitleBar()
    {
        selfInstance = this;
        
        InitializeComponent();
        
        object[] asmVersion = Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
        string infoVersion = asmVersion.Length <= 0 ? "unknown" : ((AssemblyInformationalVersionAttribute) asmVersion[0]).InformationalVersion;

        ctProgramTitle.Content = $"Maple Builder {infoVersion}";
    }

    private void ApplyApiKey(object sender, RoutedEventArgs e)
    {
        bool result = ResourceManager.Instance.SetApiKey(ctInputAPI.Text);
        if (result)
        {
            ctInputAPI.IsReadOnly = true;
            ctInputAPI.Focusable = false;
            Thickness margin = ctInputAPI.Margin;
            margin.Left += 58;
            ctInputAPI.Margin = margin;
            ctInputAPI.Width += 58;
            ctAPIApplyButton.Visibility = Visibility.Collapsed;
        }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        MainWindow.Window!.DragMove();
    }

    private void SettingWzPath(object sender, RoutedEventArgs e)
    {
        OpenFileDialog fDialog = new OpenFileDialog();
        fDialog.Filter = "Base.wz|Base.wz";
        if (fDialog.ShowDialog() == true)
        {
            string select = Directory.GetParent(Directory.GetParent(fDialog.FileName)!.FullName)!.FullName;
            Console.WriteLine($"{select}");
            if (ResourceManager.Instance.SetWzPath(select))
            {
                ctWzPathButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}