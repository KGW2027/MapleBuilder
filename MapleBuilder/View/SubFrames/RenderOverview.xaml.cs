using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
using MapleBuilder.Control.Data.Spec;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.View.SubFrames;

public partial class RenderOverview : UserControl
{
    // private static readonly Style? BUTTON_STYLE = (Style?)Application.Current.Resources[typeof(Button)];
    //
    // private void OnRegisterItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    // {
    //     Dictionary<string, MapleItemBase> hashToItem = new();
    //     Dictionary<string, bool> hashToBool = new();
    //
    //     selfInstance!.Dispatcher.BeginInvoke(() =>
    //     {
    //         List<UIElement> removes = new();
    //         foreach (var element in ctItemListStack.Children)
    //         {
    //             string eHash = ((Button) element).Tag.ToString()!;
    //             if (hashToBool.ContainsKey(eHash)) hashToBool[eHash] = true;
    //             else removes.Add((UIElement) element);
    //         }
    //
    //         removes.ForEach(obj => ctItemListStack.Children.Remove(obj));
    //
    //         foreach (var pair in hashToItem)
    //         {
    //             if (hashToBool[pair.Key]) continue;
    //
    //             Button newButton = new Button
    //             {
    //                 Tag = pair.Key,
    //                 Style = BUTTON_STYLE,
    //                 Content = pair.Value.DisplayName,
    //                 HorizontalContentAlignment = HorizontalAlignment.Left,
    //                 Width = 537,
    //                 Height = 32,
    //                 Padding = new Thickness(12, 0, 0, 0)
    //             };
    //             newButton.Click += EditItem;
    //             ctItemListStack.Children.Add(newButton);
    //         }
    //     });
    // }

    public EquipmentSlot? this[MapleEquipType.EquipType type, int slot]
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
    }
    
    private void OnEquipmentChanged(MapleEquipType.EquipType type, int slot)
    {
        EquipmentSlot? element = this[type, slot];
        if (element == null) return;

        Dispatcher.BeginInvoke(() =>
        {
            element.DisplayItem = GlobalDataController.Instance.PlayerInstance!.Equipment[type, slot]!;
        });
    }

    #region Event Handler
    private void OnSearchText(object sender, TextChangedEventArgs e)
    {
        
    }

    private void CreateNewItem(object sender, RoutedEventArgs e)
    {
        
    }
    
    private void EditItem(object sender, RoutedEventArgs e)
    {
        
    }
    #endregion
}