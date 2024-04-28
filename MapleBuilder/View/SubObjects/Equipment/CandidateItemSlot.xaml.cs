using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.Equipment;

public partial class CandidateItemSlot : UserControl
{
    public CandidateItemSlot()
    {
        InitializeComponent();
        HoverBorder.Visibility = Visibility.Collapsed;
        HoverLabel.Visibility = Visibility.Collapsed;
    }

    #region XAML Property

    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(EquipmentData), typeof(CandidateItemSlot),
        new PropertyMetadata(null, OnTargetItemChanged));

    private static void OnTargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CandidateItemSlot) d;
        if (e.NewValue is not EquipmentData data) return;

        control.HoverLabel.Text = data.Name;
        control.ImageThumbnail.Source = data.Image;
    }

    public EquipmentData? TargetItem
    {
        get => (EquipmentData?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }

    #endregion

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        HoverBorder.Visibility = Visibility.Visible;
        HoverLabel.Visibility = Visibility.Visible;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        HoverBorder.Visibility = Visibility.Collapsed;
        HoverLabel.Visibility = Visibility.Collapsed;
    }
}