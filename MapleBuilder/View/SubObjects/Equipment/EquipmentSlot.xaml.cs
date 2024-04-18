using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.View.SubObjects;

public partial class EquipmentSlot : UserControl
{
    public class ItemSlot
    {
        public string Text { get; set; }
        public BitmapImage Image { get; set; }
    }
    
    private static readonly Brush NON_HOVER = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
    private static readonly Brush HOVER = new SolidColorBrush(Color.FromArgb(0xA0, 0x00, 0x00, 0x00));
    public readonly ObservableCollection<ItemSlot> ItemsSources;
    
    public EquipmentSlot()
    {
        InitializeComponent();
        ItemsSources = new ObservableCollection<ItemSlot>();
    }
    
    #region XAML Property
    
    public static readonly DependencyProperty DisplayItemProperty =
        DependencyProperty.Register(nameof(DisplayItem), typeof(ItemBase), typeof(EquipmentSlot), new PropertyMetadata(null, OnDisplayItemChanged));
    
    private static void OnDisplayItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (EquipmentSlot) d;
        ItemBase newItem = (ItemBase) e.NewValue;

        if (newItem.EquipData != null)
        {
            var slot = new ItemSlot
            {
                Text = newItem.DisplayName,
                Image = newItem.EquipData.Image
            };
            control.ItemsSources.Add(slot);
            control.DisplayBox.ItemsSource = control.ItemsSources;
            control.DisplayBox.SelectedItem = slot;
        }
    }

    public ItemBase DisplayItem
    {
        get => (ItemBase) GetValue(DisplayItemProperty);
        set => SetValue(DisplayItemProperty, value);
    }
    
    #endregion

    private void OnHover(object sender, MouseEventArgs e)
    {
        // Borderline.Background = HOVER;
        // ItemLabel.Visibility = Visibility.Visible;
    }

    private void OnHoverEnd(object sender, MouseEventArgs e)
    {
        // Borderline.Background = NON_HOVER;
        // ItemLabel.Visibility = Visibility.Collapsed;
    }
}