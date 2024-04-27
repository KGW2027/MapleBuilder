using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.View.SubObjects.Equipment;

public partial class ItemButtonDisplay : UserControl
{
    public ItemButtonDisplay()
    {
        InitializeComponent();
    }

    #region XAML Property

    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(CommonItem), typeof(ItemButtonDisplay), new PropertyMetadata(null, OnTargetItemChanged));

    private static void OnTargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ItemButtonDisplay) d;
        if (e.NewValue is not CommonItem cItem) return;

        control.Tag = cItem.ItemHash;
        control.ItemName.Content = cItem.DisplayName;
        control.SummaryPanel.Children.Clear();
        if (cItem.EquipData != null) control.ItemThumbnail.Source = cItem.EquipData.Image;
        
        control.AddUpgrades();
        control.AddStarforce();
        control.AddAddOption();
        control.ApplyPotential();
        control.AddSpecialRing();
    }

    public CommonItem? TargetItem
    {
        get => (CommonItem?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }

    #endregion

    private void AddUpgrades()
    {
        if (TargetItem?.Upgrades == null || TargetItem.MaxUpgradeCount == null || TargetItem.RemainUpgradeCount == null) return;

        int curUpg = TargetItem.MaxUpgradeCount.Value - TargetItem.RemainUpgradeCount.Value;
        
        Image image = new Image
        {
            Width = 16,
            Height = 16,
            Margin = new Thickness(0, 0, 4, 0),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Source = WzDatabase.Instance.EquipmentDataList.TryGetValue("투구 방어력 주문서 60%", out var src) ? src.Image : new BitmapImage()
        };
        Label label = new Label
        {
            Content = $"{curUpg} / {TargetItem.MaxUpgradeCount.Value}",
            FontSize = 16,
            FontWeight = FontWeights.Thin,
            Margin = new Thickness(0, -6, 24, 0)
        };

        SummaryPanel.Children.Add(image);
        SummaryPanel.Children.Add(label);
    }

    private int GetMaxStarforce()
    {
        if (TargetItem?.EquipData == null) return 0;
        int level = TargetItem.ItemLevel;
        bool isSuperior = TargetItem.EquipData.IsSuperior;
        return isSuperior
            ? level switch // 슈페리얼 아이템
            {
                < 95 => 3,
                < 108 => 5,
                < 118 => 8,
                < 128 => 10,
                < 138 => 12,
                >= 138 => 15,
            }
            : level switch // 일반 아이템
            {
                < 95 => 5,
                < 108 => 8,
                < 118 => 10,
                < 128 => 15,
                < 138 => 20,
                >= 138 => 25,
            };
    }

    private void AddStarforce()
    {
        if (TargetItem?.Starforce == null) return;
        
        Polygon star = new Polygon
        {
            Stroke = Brushes.Black,
            StrokeThickness = 0.4,
            Margin = new Thickness(0, 0, 4, 0),
            Width = 16,
            Height = 16,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Fill = new SolidColorBrush(Color.FromRgb(0xB2, 0x7D, 0x00)),
            Points = { 
                new Point(8.000,0.000), new Point(9.796,5.528), new Point(15.600,5.528), new Point(10.912,8.946),
                new Point(12.702,14.472), new Point(8.000,11.040), new Point(3.298,14.472), new Point(5.088,8.946),
                new Point(0.400,5.528), new Point(6.204,5.528), new Point(8.000,0.000)
                }
        };

        Label sfvLabel = new Label
        {
            FontSize = 16,
            FontWeight = FontWeights.Thin,
            Margin = new Thickness(0, -6, 24, 0),
            Content = $"{TargetItem.Starforce} / {GetMaxStarforce()}"
        };

        SummaryPanel.Children.Add(star);
        SummaryPanel.Children.Add(sfvLabel);
    }

    private void AddAddOption()
    {
        if (TargetItem?.AddOptions == null) return;

        var pInst = GlobalDataController.Instance.PlayerInstance;
        MapleClass.ClassType pClass = pInst!.Class;
        string summary = "";

        for (int idx = 0; idx < pInst.AffectTypes.Length; idx++)
        {
            if (pInst.AffectTypes[idx] == MapleStatus.StatusType.OTHER ||
                (pClass != MapleClass.ClassType.XENON && idx > 0)) break;
            if (Enum.TryParse(pInst.AffectTypes[0].ToString(), out AddOptions.AddOptionType addType))
            {
                int val = 0;
                foreach (var pair in TargetItem.AddOptions)
                {
                    if (pair.Key <= AddOptions.AddOptionType.HP && (pair.Key & addType) == addType)
                    {
                        val += TargetItem.GetStatOption(pair.Value, pair.Key != addType);
                    }
                }
                if(val > 0) summary += $"{addType.ToString()} +{val} ";
            }
        }

        var atkType = pInst.AffectTypes[0] == MapleStatus.StatusType.INT
            ? AddOptions.AddOptionType.MAGIC
            : AddOptions.AddOptionType.ATTACK;
        if (TargetItem.AddOptions.TryGetValue(atkType, out var atkValue))
        {
            string prefix = atkType == AddOptions.AddOptionType.ATTACK ? "공" : "마";
            summary += $"{prefix} +{TargetItem.GetAttackOption(atkValue, atkType == AddOptions.AddOptionType.ATTACK)} ";
        }

        if (TargetItem.AddOptions.TryGetValue(AddOptions.AddOptionType.ALL_STAT, out var allStat) && pClass != MapleClass.ClassType.DEMON_AVENGER)
        {
            summary += $"올스탯 {allStat}% ";
        }

        int bdmg = TargetItem.AddOptions.GetValueOrDefault(AddOptions.AddOptionType.BOSS_DAMAGE, 0);
        int dmg = TargetItem.AddOptions.GetValueOrDefault(AddOptions.AddOptionType.DAMAGE, 0);
        if (bdmg + dmg > 0) summary += $"뎀 {bdmg + dmg}% ";

        summary = summary.Trim();
        if (string.IsNullOrEmpty(summary)) return;
        
        // UI Element Init
        Image image = new Image
        {
            Width = 16,
            Height = 16,
            Margin = new Thickness(0, 0, 4, 0),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Source = WzDatabase.Instance.EquipmentDataList.TryGetValue("영원한 환생의 불꽃", out var src) ? src.Image : new BitmapImage()
        };
        Label label = new Label
        {
            Content = summary,
            FontSize = 16,
            FontWeight = FontWeights.Thin,
            Margin = new Thickness(0, -6, 24, 0)
        };

        SummaryPanel.Children.Add(image);
        SummaryPanel.Children.Add(label);
    }


    private static readonly Dictionary<MaplePotentialGrade.GradeType, Brush> BORDER_COLOR =
        new()
        {
            {MaplePotentialGrade.GradeType.NONE, new SolidColorBrush(Color.FromRgb(0x31, 0x31, 0x31))},
            {MaplePotentialGrade.GradeType.RARE, new SolidColorBrush(Color.FromRgb(0x66, 0xFF, 0xFF))},
            {MaplePotentialGrade.GradeType.EPIC, new SolidColorBrush(Color.FromRgb(143, 123, 187))},
            {MaplePotentialGrade.GradeType.UNIQUE, new SolidColorBrush(Color.FromRgb(240, 184, 29))},
            {MaplePotentialGrade.GradeType.LEGENDARY, new SolidColorBrush(Color.FromRgb(121, 233, 4))}
        };
    private void ApplyPotential()
    {
        if (TargetItem?.Potential == null) return;

        PotentialLine.Stroke = BORDER_COLOR[TargetItem.TopGrade];
        AdditionalLine.Stroke = BORDER_COLOR[TargetItem.BottomGrade];
    }

    private void AddSpecialRing()
    {
        if (TargetItem?.SpecialSkillLevel == 0) return;
        
        
        // UI Element Init
        Image image = new Image
        {
            Width = 16,
            Height = 16,
            Margin = new Thickness(0, 0, 4, 0),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Source = WzDatabase.Instance.EquipmentDataList.TryGetValue("리스트레인트 링", out var src) ? src.Image : new BitmapImage()
        };
        Label label = new Label
        {
            Content = $"LV {TargetItem!.SpecialSkillLevel}",
            FontSize = 16,
            FontWeight = FontWeights.Thin,
            Margin = new Thickness(0, -6, 24, 0)
        };

        SummaryPanel.Children.Add(image);
        SummaryPanel.Children.Add(label);
    }
}