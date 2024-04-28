using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Spec;
using MapleBuilder.View.SubObjects;
using MapleBuilder.View.SubObjects.Equipment;

namespace MapleBuilder.View.SubFrames;

public partial class RenderOverview : UserControl
{
    private EquipmentSlot? this[MapleEquipType.EquipType type, int slot]
    {
        get
        {
            switch (type)
            {
                case MapleEquipType.EquipType.RING:
                    if (slot == 0) return ctEquips.Ring1;
                    if (slot == 1) return ctEquips.Ring2;
                    if (slot == 2) return ctEquips.Ring3;
                    return ctEquips.Ring4;
                case MapleEquipType.EquipType.PENDANT:
                    if (slot == 0) return ctEquips.Pendant1;
                    return ctEquips.Pendant2;
                case MapleEquipType.EquipType.FACE:
                    return ctEquips.Face;
                case MapleEquipType.EquipType.EYE:
                    return ctEquips.Eye;
                case MapleEquipType.EquipType.EARRING:
                    return ctEquips.Earring;
                case MapleEquipType.EquipType.BADGE:
                    return ctEquips.Badge;
                case MapleEquipType.EquipType.MEDAL:
                    return ctEquips.Medal;
                case MapleEquipType.EquipType.BELT:
                    return ctEquips.Belt;
                case MapleEquipType.EquipType.POCKET:
                    return ctEquips.Pocket;
                case MapleEquipType.EquipType.HEART:
                    return ctEquips.Heart;
                case MapleEquipType.EquipType.WEAPON:
                    return ctEquips.Weapon;
                case MapleEquipType.EquipType.SUB_WEAPON:
                    return ctEquips.SubWeapon;
                case MapleEquipType.EquipType.EMBLEM:
                    return ctEquips.Emblem;
                case MapleEquipType.EquipType.HELMET:
                    return ctEquips.Cap;
                case MapleEquipType.EquipType.TOP:
                    return ctEquips.Top;
                case MapleEquipType.EquipType.BOTTOM:
                    return ctEquips.Bottom;
                case MapleEquipType.EquipType.SHOULDER:
                    return ctEquips.Shoulder;
                case MapleEquipType.EquipType.GLOVE:
                    return ctEquips.Gloves;
                case MapleEquipType.EquipType.CAPE:
                    return ctEquips.Cape;
                case MapleEquipType.EquipType.BOOT:
                    return ctEquips.Boot;
                case MapleEquipType.EquipType.TITLE:
                    return ctEquips.Title;
                case MapleEquipType.EquipType.TOP_BOTTOM:
                    break;
            }

            return null;
        }
    }
    
    public RenderOverview()
    {
        InitializeComponent();

        EquipWrapper.OnEquipmentChanged += OnEquipmentChanged;
        ItemDatabase.Instance.CachedItemList.CollectionChanged += CachedItemListOnCollectionChanged;
        editingItem = null;
    }

    private ItemButtonDisplay? editingItem;

    private ItemButtonDisplay NewButton(CommonItem item)
    {
        var display = new ItemButtonDisplay
        {
            TargetItem = item,
            Margin = new Thickness(0, 2, 0, 0)
        };
        display.MouseLeftButtonDown += EditItem;
        return display;
    }

    private void CachedItemListOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (e.NewItems != null)
            {
                foreach (object o in e.NewItems)
                {
                    if (o is not CommonItem itemBase || itemBase.EquipType == MapleEquipType.EquipType.MEDAL ||
                        itemBase.EquipType == MapleEquipType.EquipType.BADGE) continue;
                    var button = NewButton(itemBase);
                    ItemButtonStackPanel.Children.Add(button);
                }
            }

            if (e.OldItems is not {Count: > 0}) return;

            List<string> oldItemHashes = new List<string>();
            foreach (var oldItem in e.OldItems)
            {
                ItemBase itembase = (ItemBase) oldItem;
                oldItemHashes.Add(itembase.ItemHash);
            }

            List<UIElement> deleteQueue = new List<UIElement>();
            foreach (var o in ItemButtonStackPanel.Children)
            {
                if (o is not Button button) continue;
                if (oldItemHashes.Contains(button.Tag.ToString()!))
                    deleteQueue.Add(button);
            }

            deleteQueue.ForEach(del => ItemButtonStackPanel.Children.Remove(del));
        });
    }

    private void OnEquipmentChanged(MapleEquipType.EquipType type, int slot)
    {
        EquipmentSlot? element = this[type, slot];
        if (element == null) return;

        Dispatcher.BeginInvoke(() =>
        {
            element.DisplayItem = GlobalDataController.Instance.PlayerInstance!.Equipment[type, slot]!;
            
            ctSetPanel.Children.Clear();
            var setOptions = GlobalDataController.Instance.PlayerInstance.Equipment.SetEffect.SetString;

            foreach (var set in setOptions.Split("|"))
            {
                string[] split = set.Split(":");
                if (split.Length < 2)
                {
                    continue;
                }
                SetEffectDisplay setDisplay = new SetEffectDisplay
                {
                    SetName = split[0],
                    SetCount = split[1]
                };
                ctSetPanel.Children.Add(setDisplay);
            }

        });
    }

    #region Event Handler
    private void OnSearchText(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        string search = textBox.Text;
        if (string.IsNullOrEmpty(search))
        {
            foreach (var panel in ItemButtonStackPanel.Children)
            {
                if (panel is not ItemButtonDisplay btn) continue;
                btn.Visibility = Visibility.Visible;
            }

            return;
        }

        foreach (var panel in ItemButtonStackPanel.Children)
        {
            if (panel is not ItemButtonDisplay btn || btn.TargetItem == null) continue;
            btn.Visibility = btn.TargetItem.DisplayName.Contains(search) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void CreateNewItem(object sender, RoutedEventArgs e)
    {
        RenderFrame.SetCharacterTopVisibility(Visibility.Collapsed);
        ctEquips.Visibility = Visibility.Collapsed;
        ctSetScroll.Visibility = Visibility.Collapsed;
        
        CreateItemPanel.Visibility = Visibility.Visible;
    }
    
    private void EditItem(object sender, RoutedEventArgs e)
    {
        if (sender is not ItemButtonDisplay button) return;
        if (button.Tag.ToString() == null || !ItemDatabase.TryFindItemFromHash(button.Tag.ToString()!, out var targetItem) || targetItem == null) return;
        editingItem = button;
        
        RenderFrame.SetCharacterTopVisibility(Visibility.Collapsed);
        ctEquips.Visibility = Visibility.Collapsed;
        ctSetScroll.Visibility = Visibility.Collapsed;
        
        ctEditEquipment.TargetItem = targetItem;
        ctEditEquipment.Visibility = Visibility.Visible;
    }
    #endregion

    private void OnEditSaved(object sender, RoutedEventArgs e)
    {
        if (e is not EditEquipment.SaveEquipmentEvent savedEvent) return;

        if (savedEvent.CloseWindow)
        {
            RenderFrame.SetCharacterTopVisibility(Visibility.Visible);
            ctEquips.Visibility = Visibility.Visible;
            ctSetScroll.Visibility = Visibility.Visible;

            ctEditEquipment.Visibility = Visibility.Collapsed;
        }

        if (GlobalDataController.Instance.PlayerInstance == null) return;
        if (savedEvent.NewItem is not CommonItem commonItem) return;

        if (editingItem != null)
        {
            editingItem.TargetItem = null;
            editingItem.TargetItem = commonItem;
            editingItem = null;
        }
        
        int max = commonItem.EquipType switch
        {
            MapleEquipType.EquipType.RING => 3,
            MapleEquipType.EquipType.PENDANT => 1,
            _ => 0
        };
        
        for (int idx = 0; idx <= max; idx++)
        {
            var equipItem = GlobalDataController.Instance.PlayerInstance.Equipment[commonItem.EquipType, idx];
            if (equipItem == null || equipItem.ItemHash != commonItem.ItemHash) continue;
            GlobalDataController.Instance.PlayerInstance.Equipment.Refresh();
            break;
        }
    }

    private void OnSelectByNewItem(object sender, RoutedEventArgs e)
    {
        if (e is not NewItemSelect.NewItemEvent args) return;

        if(args.ItemData != null) ItemDatabase.Instance.RegisterItem(args.ItemData, out _);
        RenderFrame.SetCharacterTopVisibility(Visibility.Visible);
        ctEquips.Visibility = Visibility.Visible;
        ctSetScroll.Visibility = Visibility.Visible;
        CreateItemPanel.Visibility = Visibility.Collapsed;
    }
}