using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.View.SubObjects;

public partial class EquipmentSlot : UserControl
{
    private static readonly Brush NON_HOVER = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
    private static readonly Brush HOVER = new SolidColorBrush(Color.FromArgb(0xA0, 0x00, 0x00, 0x00));
    
    public EquipmentSlot()
    {
        InitializeComponent();
    }
    
    #region XAML Property
    
    public static readonly DependencyProperty DisplayItemProperty =
        DependencyProperty.Register(nameof(DisplayItem), typeof(ItemBase), typeof(EquipmentSlot), new PropertyMetadata(null, OnDisplayItemChanged));

    private static void OnDisplayItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (EquipmentSlot) d;
        ItemBase newItem = (ItemBase) e.NewValue;

        if(newItem.EquipData != null)
            control.DisplayImage.Source = newItem.EquipData.Image;
    }

    public ItemBase DisplayItem
    {
        get => (ItemBase) GetValue(DisplayItemProperty);
        set => SetValue(DisplayItemProperty, value);
    }
    
    #endregion

    private void OnHover(object sender, MouseEventArgs e)
    {
        Borderline.Background = HOVER;
        ItemLabel.Visibility = Visibility.Visible;
    }

    private void OnHoverEnd(object sender, MouseEventArgs e)
    {
        Borderline.Background = NON_HOVER;
        ItemLabel.Visibility = Visibility.Collapsed;
    }
}