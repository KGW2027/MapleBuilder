using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.View.SubObjects.Equipment;

public partial class EquipmentSlot : UserControl
{
    public class ItemSlot
    {
        public string Text { get; set; }
        public BitmapImage Image { get; set; }
        public int Hash { get; set; }
        public ItemBase? Item { get; set; }
    }
    
    private static readonly Brush NON_HOVER = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
    private static readonly Brush HOVER = new SolidColorBrush(Color.FromArgb(0xA0, 0x00, 0x00, 0x00));
    public readonly ObservableCollection<ItemSlot> ItemsSources;

    private static readonly ItemSlot EMPTY = new()
    {
        Item = null,
        Image = new BitmapImage(),
        Hash = -1,
        Text = "장착 해제"
    };
    
    public EquipmentSlot()
    {
        InitializeComponent();
        ItemsSources = new ObservableCollection<ItemSlot>();
        ItemsSources.Add(EMPTY);
        DisplayBox.ItemsSource = ItemsSources;
        
        ItemDatabase.Instance.CachedItemList.CollectionChanged += CachedItemListOnCollectionChanged;
    }

    private void CachedItemListOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (e.NewItems == null || e.NewItems.Count == 0) return;
            foreach (var o in e.NewItems)
            {
                if (o is not ItemBase item || item.EquipType != TargetEquipType || item.EquipData == null) continue;

                var slot = new ItemSlot
                {
                    Text = item.DisplayName,
                    Image = item.EquipData.Image,
                    Hash = item.GetHashCode(),
                    Item = item
                };
                ItemsSources.Add(slot);
            }
        });
    }

    #region XAML Property
    
    public static readonly DependencyProperty DisplayItemProperty =
        DependencyProperty.Register(nameof(DisplayItem), typeof(ItemBase), typeof(EquipmentSlot), new PropertyMetadata(null, OnDisplayItemChanged));

    private static void OnDisplayItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null) return;
        
        var control = (EquipmentSlot) d;
        ItemBase newItem = (ItemBase) e.NewValue;

        bool found = false;
        foreach (var item in control.ItemsSources)
        {
            if (item.Hash == newItem.GetHashCode())
            {
                control.DisplayBox.SelectedItem = item;
                found = true;
                break;
            }
        }

        if (found || newItem.EquipData == null) return;

        var slot = new ItemSlot
        {
            Text = newItem.DisplayName,
            Image = newItem.EquipData.Image,
            Hash = newItem.GetHashCode()
        };
        control.ItemsSources.Add(slot);
        control.DisplayBox.SelectedItem = slot;
    }

    public ItemBase DisplayItem
    {
        get => (ItemBase) GetValue(DisplayItemProperty);
        set => SetValue(DisplayItemProperty, value);
    }
    
    public static readonly DependencyProperty TargetEquipTypeProperty =
        DependencyProperty.Register(nameof(TargetEquipType), typeof(MapleEquipType.EquipType), typeof(EquipmentSlot));
    
    public MapleEquipType.EquipType TargetEquipType
    {
        get => (MapleEquipType.EquipType) GetValue(TargetEquipTypeProperty);
        set => SetValue(TargetEquipTypeProperty, value);
    }
    
    public static readonly DependencyProperty TargetEquipIndexProperty =
        DependencyProperty.Register(nameof(TargetEquipIndex), typeof(int), typeof(EquipmentSlot));
    
    public int TargetEquipIndex
    {
        get => (int) GetValue(TargetEquipIndexProperty);
        set => SetValue(TargetEquipIndexProperty, value);
    }
    #endregion

    
    // TODO: Hover 시 아이템 이름 나오는 기능 만들기 + 아이템 변경 기능 만들기. (MEMO 0418)
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

    private void OnUserChangedEquipment(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized || e.AddedItems.Count == 0 || e.AddedItems[0] == null) return;
        if (GlobalDataController.Instance.PlayerInstance == null) return;

        ItemSlot newSlot = (ItemSlot) e.AddedItems[0]!;
        GlobalDataController.Instance.PlayerInstance.Equipment[TargetEquipType, TargetEquipIndex] = newSlot.Item;
    }
}