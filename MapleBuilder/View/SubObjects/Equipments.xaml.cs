using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects;

public partial class Equipments : UserControl
{
    private Dictionary<MapleEquipType.EquipType, List<UIElement>> SlotMap;
    
    public Equipments()
    {
        InitializeComponent();

        SlotMap = new Dictionary<MapleEquipType.EquipType, List<UIElement>>
        {
            {MapleEquipType.EquipType.RING, new List<UIElement> {Ring1, Ring2, Ring3, Ring4}},
            {MapleEquipType.EquipType.POCKET, new List<UIElement> {Pocket}},
            {MapleEquipType.EquipType.PENDANT, new List<UIElement> {Pendant1, Pendant2}},
            {MapleEquipType.EquipType.WEAPON, new List<UIElement> {Weapon}},
            {MapleEquipType.EquipType.BELT, new List<UIElement> {Belt}},
            {MapleEquipType.EquipType.HELMET, new List<UIElement> {Cap}},
            {MapleEquipType.EquipType.FACE, new List<UIElement> {Face}},
            {MapleEquipType.EquipType.EYE, new List<UIElement> {Eye}},
            {MapleEquipType.EquipType.TOP, new List<UIElement> {Top}},
            {MapleEquipType.EquipType.BOTTOM, new List<UIElement> {Bottom}},
            {MapleEquipType.EquipType.BOOT, new List<UIElement> {Boot}},
            {MapleEquipType.EquipType.EARRING, new List<UIElement> {Earring}},
            {MapleEquipType.EquipType.SHOULDER, new List<UIElement> {Shoulder}},
            {MapleEquipType.EquipType.GLOVE, new List<UIElement> {Gloves}},
            {MapleEquipType.EquipType.TITLE, new List<UIElement> {Title}},
            {MapleEquipType.EquipType.EMBLEM, new List<UIElement> {Emblem}},
            {MapleEquipType.EquipType.BADGE, new List<UIElement> {Badge}},
            {MapleEquipType.EquipType.MEDAL, new List<UIElement> {Medal}},
            {MapleEquipType.EquipType.SUB_WEAPON, new List<UIElement> {SubWeapon}},
            {MapleEquipType.EquipType.CAPE, new List<UIElement> {Cape}},
            {MapleEquipType.EquipType.HEART, new List<UIElement> {Heart}},
        };
        
        BuilderDataContainer.InitEquipmentSlots(new List<UIElement>
        {
            Ring1, Ring2, Ring3, Ring4, Pocket, Pendant1, Pendant2, Weapon, Belt, Cap, Face, Eye, Top, Bottom,
            Boot, Earring, Shoulder, Gloves, Title, Emblem, Badge, Medal, SubWeapon, Cape, Heart
        });
    }

    private void PushEquipment(MapleEquipType.EquipType equipType, MapleCommonItem commonItem)
    {
        foreach (UIElement itemSlot in SlotMap[equipType])
            if (((EquipmentSlot) itemSlot).SetItemIfNull(commonItem))
                break;
    }

    public void UpdateEquipments(CharacterInfo cInfo)
    {
        foreach(MapleCommonItem item in cInfo.Items)
            PushEquipment(item.EquipType, item);
    }

    public void Clear()
    {
        foreach(var elements in SlotMap.Values)
            elements.ForEach(element => ((EquipmentSlot)element).Clear());
    }
}