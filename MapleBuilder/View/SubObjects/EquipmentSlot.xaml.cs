using System;
using System.Windows.Controls;
using System.Windows.Input;
using MapleAPI.DataType;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects;

public partial class EquipmentSlot : UserControl
{
    private MapleItem? itemInfo;
    private WzItem? wzItemInfo;
    
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
        Console.WriteLine($"Hover in {Name}");
    }

    private void OnHoverEnd(object sender, MouseEventArgs e)
    {
        Console.WriteLine($"Hover out {Name}");
    }
}