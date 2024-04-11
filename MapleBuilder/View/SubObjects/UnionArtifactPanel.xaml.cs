using System;
using System.Windows;
using System.Windows.Controls;

namespace MapleBuilder.View.SubObjects;

public partial class UnionArtifactPanel : UserControl
{
    public UnionArtifactPanel()
    {
        InitializeComponent();
    }

    private void OnArtifactSettingChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized || e is not UnionArtifactControl.ArtifactSettingChangedEventArgs args) return;
        
        Console.Write($"{args.Level} : {args.StatusTypes[0]} {args.StatusTypes[1]} {args.StatusTypes[2]} ");
        if(args.IsChangedLevel) Console.WriteLine($"Before Level : {args.BeforeLevel}");
        else Console.WriteLine($"Before Type : {args.BeforeType}");
        
    }
}