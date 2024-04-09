using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects;

public partial class EquipmentSlot : UserControl
{
    public delegate void OnItemChanged(MapleItemBase? prevItem, MapleItemBase? newItem);
    
    private MapleItemBase? itemInfo;
    private WzItem? wzItemInfo;
    public OnItemChanged? itemChanged;
    private static readonly Brush NON_HOVER = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
    private static readonly Brush HOVER = new SolidColorBrush(Color.FromArgb(0xA0, 0x00, 0x00, 0x00));
    
    public EquipmentSlot()
    {
        InitializeComponent();
        
        itemInfo = null;
        wzItemInfo = null;
    }

    private void Update()
    {
        if(itemInfo == null) return;
        wzItemInfo = ResourceManager.GetItemIcon(itemInfo.Name);
        if (wzItemInfo == null)
        {
            Console.WriteLine($"Not found {itemInfo.Name}");
            return;
        }
        
        Dispatcher.BeginInvoke(() =>
        {
            ctItemRenderer.Source = wzItemInfo.IconRaw;
        });
    }
    
    public bool SetItemIfNull(MapleItemBase commonItem)
    {
        if (itemInfo != null) return false;
        itemInfo = commonItem;
        Update();
        itemChanged?.Invoke(null, commonItem);
        return true;
    }

    public bool GetItem(out MapleItemBase? item)
    {
        item = null;
        if (itemInfo == null || wzItemInfo == null) return false;
        item = itemInfo;
        return true;
    }

    public void SetItem(MapleItemBase? newItem)
    {
        MapleItemBase? prevItem = itemInfo;
        itemInfo = newItem;
        itemChanged?.Invoke(prevItem, newItem);
    }

    public void Clear()
    {
        itemInfo = null;
        wzItemInfo = null;
    }

    private void OnHover(object sender, MouseEventArgs e)
    {
        ctBorder.Background = HOVER;
        ctEditLabel.Visibility = Visibility.Visible;
    }

    private void OnHoverEnd(object sender, MouseEventArgs e)
    {
        ctBorder.Background = NON_HOVER;
        ctEditLabel.Visibility = Visibility.Collapsed;
    }
}