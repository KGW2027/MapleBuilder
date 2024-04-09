using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleBuilder.Control;
using MapleBuilder.MapleData;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.View.SubFrames;

public partial class RenderOverview : UserControl
{
    private static RenderOverview? selfInstance;
    private static readonly Style? BUTTON_STYLE = (Style?)Application.Current.Resources[typeof(Button)];
    
    public static void Update(CharacterInfo cInfo)
    {
        selfInstance?.Dispatcher.Invoke(() =>
        {
            selfInstance.ctEquips.Clear();
            selfInstance.ctEquips.UpdateEquipments(cInfo);
        });
    }

    public static void UpdateSetDisplay(string setDisplay)
    {
        selfInstance?.Dispatcher.BeginInvoke(() =>
        {
            selfInstance.ctSetPanel.Children.Clear();
            string[] split = setDisplay.Split(" ");

            for (int index = 0; index < split.Length; index += 2)
            {
                if (!Enum.TryParse(split[index], out SetEffect.SetType setType)) continue;
                string setName = SetEffect.GetSetTypeString(setType);

                selfInstance.ctSetPanel.Children.Add(new SetEffectDisplay
                {
                    SetName = setName,
                    SetCount = split[index + 1]
                });
            }
        });
    }

    private void OnRegisterItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dictionary<string, MapleCommonItem> hashToItem = new();
        Dictionary<string, bool> hashToBool = new();

        foreach (MapleCommonItem item in BuilderDataContainer.RegisterItems)
        {
            hashToBool.Add(item.Hash, false);
            hashToItem.Add(item.Hash, item);
        }

        selfInstance!.Dispatcher.BeginInvoke(() =>
        {
            List<UIElement> removes = new();
            foreach (var element in ctItemListStack.Children)
            {
                string eHash = ((Button) element).Tag.ToString()!;
                if (hashToBool.ContainsKey(eHash)) hashToBool[eHash] = true;
                else removes.Add((UIElement) element);
            }

            removes.ForEach(obj => ctItemListStack.Children.Remove(obj));

            foreach (var pair in hashToItem)
            {
                if (hashToBool[pair.Key]) continue;

                Button newButton = new Button
                {
                    Tag = pair.Key,
                    Style = BUTTON_STYLE,
                    Content = pair.Value.DisplayName,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Width = 537,
                    Height = 32,
                    Padding = new Thickness(12, 0, 0, 0)
                };
                newButton.Click += EditItem;
                ctItemListStack.Children.Add(newButton);
            }
        });
    }


    public RenderOverview()
    {
        selfInstance = this;
        BuilderDataContainer.RegisterItems.CollectionChanged += OnRegisterItemsChanged;
        InitializeComponent();
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

    // private void OnChangeScreenToEquip(object sender, RoutedEventArgs e)
    // {
    //     RenderFrame.RenderType = RenderFrame.RenderScreenType.OVERVIEW_EQUIPMENT;
    // }
    //
    // private void OnChangeScreenToStatSymbol(object sender, RoutedEventArgs e)
    // {
    //     RenderFrame.RenderType = RenderFrame.RenderScreenType.STAT_SYMBOL;
    // }
    //
    // private void OnChangeScreenToSpecialEquip(object sender, RoutedEventArgs e)
    // {
    //     RenderFrame.RenderType = RenderFrame.RenderScreenType.SPECIAL_EQUIPS;
    // }
    //
    // private void OnChangeScreenToCashEquip(object sender, RoutedEventArgs e)
    // {
    //     RenderFrame.RenderType = RenderFrame.RenderScreenType.CASH_EQUIPS;
    // }
}