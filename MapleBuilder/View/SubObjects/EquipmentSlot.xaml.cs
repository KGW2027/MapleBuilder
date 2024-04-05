using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MapleAPI.DataType;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects;

public partial class EquipmentSlot : UserControl
{
    private MapleItem? itemInfo;
    private WzItem? wzItemInfo;
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
        ctItemRenderer.Source = wzItemInfo.IconRaw;
    }

    public bool SetItemIfNull(MapleItem item)
    {
        if (itemInfo != null) return false;
        itemInfo = item;
        Update();
        return true;
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