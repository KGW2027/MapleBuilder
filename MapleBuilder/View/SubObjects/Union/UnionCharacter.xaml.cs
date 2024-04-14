using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.View.SubObjects;

public partial class UnionCharacter : UserControl
{
    public UnionCharacter()
    {
        InitializeComponent();
        
        ctRaiderRankBox.Items.Clear();
        foreach (MapleUnion.RaiderRank rank in Enum.GetValues<MapleUnion.RaiderRank>())
            ctRaiderRankBox.Items.Add(rank);
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
        MapleUnion.RaiderRank prevRank = e.RemovedItems.Count > 0
            ? (MapleUnion.RaiderRank) e.RemovedItems[0]!
            : MapleUnion.RaiderRank.NONE;
        
        // GlobalDataController.Instance.PlayerInstance.

        if (BuilderDataContainer.PlayerStatus == null) return;
        int delta = value - MapleUnion.GetRaiderEffectValue(raiderEffect, prevRank);
        BuilderDataContainer.PlayerStatus.ApplyUnionRaider(raiderEffect, delta);
    }
}