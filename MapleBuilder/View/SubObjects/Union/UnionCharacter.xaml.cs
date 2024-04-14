using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.Union;

public partial class UnionCharacter : UserControl
{
    public UnionCharacter()
    {
        InitializeComponent();
        
        ctRaiderRankBox.Items.Clear();
        foreach (MapleUnion.RaiderRank rank in Enum.GetValues<MapleUnion.RaiderRank>())
            ctRaiderRankBox.Items.Add(rank);
        
        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {
        GlobalDataController.OnDataUpdated -= OnDataUpdated;
        Dispatcher.BeginInvoke(() =>
        {
            MapleUnion.RaiderRank rank =
                pdata.Raider.InitRanks.GetValueOrDefault(TargetClass, MapleUnion.RaiderRank.NONE);
            ctRaiderRankBox.Text = rank.ToString();
            
            // MapleStatus.StatusType raiderEffect = MapleUnion.GetRaiderEffectByClass(TargetClass);
            // int value = MapleUnion.GetRaiderEffectValue(raiderEffect, rank);
            // ctRaiderEffect.Content = MapleUnion.GetRaiderEffectString(raiderEffect).Replace("%d", value.ToString());
        });
    }

    #region XAML Property
    
    public static readonly DependencyProperty ClassTypeProperty =
        DependencyProperty.Register("TargetClass", typeof(MapleClass.ClassType), typeof(UnionCharacter),
            new PropertyMetadata(MapleClass.ClassType.NONE, OnClassTypeChanged));

    
    public MapleClass.ClassType TargetClass
    {
        get => (MapleClass.ClassType) GetValue(ClassTypeProperty);
        set => SetValue(ClassTypeProperty, value);
    }
    
    private static void OnClassTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (UnionCharacter) d;
        MapleClass.ClassType ct = (MapleClass.ClassType) e.NewValue;
        control.ctClassLabel.Content = MapleClass.GetMapleClassString(ct);

        MapleStatus.StatusType raiderEffect = MapleUnion.GetRaiderEffectByClass(ct);
        int value = MapleUnion.GetRaiderEffectValue(raiderEffect, MapleUnion.RaiderRank.NONE);
        control.ctRaiderEffect.Content = MapleUnion.GetRaiderEffectString(raiderEffect).Replace("%d", value.ToString());

        if ((int)ct <= 45)
        {
            var image = new BitmapImage(new Uri($"pack://application:,,,/Resources/Image/class_{(int) ct + 1}.png"));
            control.ctClassProfile.Source = image;
        }
    }
    
    #endregion

    private void OnRaiderRankChanged(object sender, SelectionChangedEventArgs e)
    {        
        MapleStatus.StatusType raiderEffect = MapleUnion.GetRaiderEffectByClass(TargetClass);
        int value = MapleUnion.GetRaiderEffectValue(raiderEffect, (MapleUnion.RaiderRank) ctRaiderRankBox.SelectedItem);
        ctRaiderEffect.Content = MapleUnion.GetRaiderEffectString(raiderEffect).Replace("%d", value.ToString());

        if (GlobalDataController.Instance.PlayerInstance == null) return;
        GlobalDataController.Instance.PlayerInstance.Raider[TargetClass] = (MapleUnion.RaiderRank) ctRaiderRankBox.SelectedItem;
    }
}