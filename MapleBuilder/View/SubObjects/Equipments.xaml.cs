using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.View.SubObjects;

public partial class Equipments : UserControl
{
    private Dictionary<MapleEquipType, List<UIElement>> SlotMap;
    
    public Equipments()
    {
        InitializeComponent();

        SlotMap = new Dictionary<MapleEquipType, List<UIElement>>
        {
            {MapleEquipType.RING, new List<UIElement> {Ring4, Ring3, Ring2, Ring1}},
            {MapleEquipType.POCKET, new List<UIElement> {Pocket}},
            {MapleEquipType.PENDANT, new List<UIElement> {Pendant1, Pendant2}},
            {MapleEquipType.WEAPON, new List<UIElement> {Weapon}},
            {MapleEquipType.BELT, new List<UIElement> {Belt}},
            {MapleEquipType.HELMET, new List<UIElement> {Cap}},
            {MapleEquipType.FACE, new List<UIElement> {Face}},
            {MapleEquipType.EYE, new List<UIElement> {Eye}},
            {MapleEquipType.TOP, new List<UIElement> {Top}},
            {MapleEquipType.BOTTOM, new List<UIElement> {Bottom}},
            {MapleEquipType.BOOT, new List<UIElement> {Boot}},
            {MapleEquipType.EARRING, new List<UIElement> {Earring}},
            {MapleEquipType.SHOULDER, new List<UIElement> {Shoulder}},
            {MapleEquipType.GLOVE, new List<UIElement> {Gloves}},
            {MapleEquipType.TITLE, new List<UIElement> {Title}},
            {MapleEquipType.EMBLEM, new List<UIElement> {Emblem}},
            {MapleEquipType.BADGE, new List<UIElement> {Badge}},
            {MapleEquipType.MEDAL, new List<UIElement> {Medal}},
            {MapleEquipType.SUB_WEAPON, new List<UIElement> {SubWeapon}},
            {MapleEquipType.CAPE, new List<UIElement> {Cape}},
            {MapleEquipType.HEART, new List<UIElement> {Heart}},
        };
    }

    private void PushEquipment(MapleEquipType equipType, MapleItem item)
    {
        foreach (UIElement itemSlot in SlotMap[equipType])
            if (((EquipmentSlot) itemSlot).SetItemIfNull(item))
                break;
    }

    public void UpdateEquipments(CharacterInfo cInfo)
    {
        foreach(MapleItem item in cInfo.Items)
            PushEquipment(item.EquipType, item);
    }

    public void Clear()
    {
        foreach(var elements in SlotMap.Values)
            elements.ForEach(element => ((EquipmentSlot)element).Clear());
    }
}