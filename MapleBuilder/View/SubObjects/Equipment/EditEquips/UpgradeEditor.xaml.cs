using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class UpgradeEditor : UserControl
{
    
    public UpgradeEditor()
    {
        InitializeComponent();

        chaosLabels = new Label[8];
        int idx = 0;
        foreach (var label in ChaosEditor.GetChildren<Label>()) if(label.Tag != null) chaosLabels[idx++] = label;
        foreach (var slider in ChaosEditor.GetChildren<Slider>()) slider.ValueChanged += OnSliderValueChanged;
    }

    private static readonly Brush SELECT_HIGHLIGHT = new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0xAF, 0x17));
    private readonly Label[] chaosLabels;

    private Dictionary<MapleStatus.StatusType, double>? cachedChaosAvg;
    private UpgradeOption.UpgradeType[]? cachedUpgradeTypes;
    private int? cachedRemains;

    private Grid? selected;
    private bool isChaosAmazing;

    #region XAML Property

    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(CommonItem), typeof(UpgradeEditor), new PropertyMetadata(null, OnTargetItemChanged));

    private static void OnTargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (UpgradeEditor) d;
        control.ChaosEditor.Visibility = Visibility.Collapsed;
        control.AvailableUpgrades.Children.Clear();
        control.CurUpgrades.Children.Clear();
        control.selected = null;

        if (e.NewValue is not CommonItem cItem) return;

        control.cachedRemains = cItem.RemainUpgradeCount;
        control.cachedChaosAvg = new Dictionary<MapleStatus.StatusType, double>();
        if(cItem.ChaosAverage != null) {
            foreach (var pair in cItem.ChaosAverage)
            {
                control.cachedChaosAvg.TryAdd(pair.Key, pair.Value);
            }
        }
        
        if(cItem.Upgrades != null)
        {
            control.cachedUpgradeTypes = new UpgradeOption.UpgradeType[cItem.Upgrades.Length];
            for (int idx = 0; idx < cItem.Upgrades.Length; idx++) control.cachedUpgradeTypes[idx] = cItem.Upgrades[idx];
        }

        control.Update();
    }

    public CommonItem? TargetItem
    {
        get => (CommonItem?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }

    public static readonly RoutedEvent SavedEvent = EventManager.RegisterRoutedEvent(
        "SaveOrCancelled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UpgradeEditor));
    
    public event RoutedEventHandler OnSaveOrCancelled
    {
        add => AddHandler(SavedEvent, value);
        remove => RemoveHandler(SavedEvent, value);
    }

    #endregion
    private UpgradeOption.UpgradeType[] GetAvailableList(CommonItem item)
    {
        List<UpgradeOption.UpgradeType> candidateTypes = new List<UpgradeOption.UpgradeType>
        {
            UpgradeOption.UpgradeType.SPELL_TRACE_STR_100,
            UpgradeOption.UpgradeType.SPELL_TRACE_DEX_100,
            UpgradeOption.UpgradeType.SPELL_TRACE_INT_100,
            UpgradeOption.UpgradeType.SPELL_TRACE_LUK_100,
            UpgradeOption.UpgradeType.SPELL_TRACE_HP_100,
            UpgradeOption.UpgradeType.SPELL_TRACE_ALL_100,
            UpgradeOption.UpgradeType.SPELL_TRACE_STR_70,
            UpgradeOption.UpgradeType.SPELL_TRACE_DEX_70,
            UpgradeOption.UpgradeType.SPELL_TRACE_INT_70,
            UpgradeOption.UpgradeType.SPELL_TRACE_LUK_70,
            UpgradeOption.UpgradeType.SPELL_TRACE_HP_70,
            UpgradeOption.UpgradeType.SPELL_TRACE_ALL_70,
            UpgradeOption.UpgradeType.SPELL_TRACE_STR_30,
            UpgradeOption.UpgradeType.SPELL_TRACE_DEX_30,
            UpgradeOption.UpgradeType.SPELL_TRACE_INT_30,
            UpgradeOption.UpgradeType.SPELL_TRACE_LUK_30,
            UpgradeOption.UpgradeType.SPELL_TRACE_HP_30,
            UpgradeOption.UpgradeType.SPELL_TRACE_ALL_30,
        };

        if (item.EquipType.IsWeapon() || item.EquipType.IsArmor())
        {
            candidateTypes.Add(UpgradeOption.UpgradeType.SPELL_TRACE_STR_15);
            candidateTypes.Add(UpgradeOption.UpgradeType.SPELL_TRACE_DEX_15);
            candidateTypes.Add(UpgradeOption.UpgradeType.SPELL_TRACE_INT_15);
            candidateTypes.Add(UpgradeOption.UpgradeType.SPELL_TRACE_LUK_15);
            candidateTypes.Add(UpgradeOption.UpgradeType.SPELL_TRACE_HP_15);
            candidateTypes.Add(UpgradeOption.UpgradeType.SPELL_TRACE_ALL_15);

            if (item.EquipType is not MapleEquipType.EquipType.WEAPON and MapleEquipType.EquipType.SUB_WEAPON)
            {
                candidateTypes.Add(UpgradeOption.UpgradeType.ARMOR_70_ATK_1);
                candidateTypes.Add(UpgradeOption.UpgradeType.ARMOR_70_ATK_2);
                candidateTypes.Add(UpgradeOption.UpgradeType.ARMOR_70_MAG_1);
                candidateTypes.Add(UpgradeOption.UpgradeType.ARMOR_70_MAG_2);
                candidateTypes.Add(UpgradeOption.UpgradeType.MIRACLE_50_ATK_2);
                candidateTypes.Add(UpgradeOption.UpgradeType.MIRACLE_50_ATK_3);
                candidateTypes.Add(UpgradeOption.UpgradeType.MIRACLE_50_MAG_2);
                candidateTypes.Add(UpgradeOption.UpgradeType.MIRACLE_50_MAG_3);
            }
        }

        if (item.EquipType.IsAccessory())
        {
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_70_ATK_1);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_70_ATK_2);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_70_MAG_1);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_70_MAG_2);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_ATK_2);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_ATK_3);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_ATK_4);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_MAG_2);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_MAG_3);
            candidateTypes.Add(UpgradeOption.UpgradeType.ACCESSORY_MAG_4);
            candidateTypes.Add(UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_4);
            candidateTypes.Add(UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_5);
            candidateTypes.Add(UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_4);
            candidateTypes.Add(UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_5);
        }

        if (item.EquipType is MapleEquipType.EquipType.EARRING)
        {
            candidateTypes.Add(UpgradeOption.UpgradeType.EARRING_INT_10);
        }

        if (item.EquipType.IsWeapon() || item.EquipType is MapleEquipType.EquipType.HEART)
        {
            candidateTypes.Add(UpgradeOption.UpgradeType.MAGICAL_ATK_9);
            candidateTypes.Add(UpgradeOption.UpgradeType.MAGICAL_ATK_10);
            candidateTypes.Add(UpgradeOption.UpgradeType.MAGICAL_ATK_11);
            candidateTypes.Add(UpgradeOption.UpgradeType.MAGICAL_MAG_9);
            candidateTypes.Add(UpgradeOption.UpgradeType.MAGICAL_MAG_10);
            candidateTypes.Add(UpgradeOption.UpgradeType.MAGICAL_MAG_11);
        }

        if (item.EquipType is MapleEquipType.EquipType.HEART)
        {
            candidateTypes.Add(UpgradeOption.UpgradeType.HEART_ATK);
            candidateTypes.Add(UpgradeOption.UpgradeType.HEART_MAG);
        }

        if (item.EquipType is MapleEquipType.EquipType.PENDANT && item.UniqueName.Equals("도미네이터 펜던트"))
        {
            candidateTypes.Add(UpgradeOption.UpgradeType.DOMINATOR_FRAGMENT);
        }
        
        candidateTypes.Add(UpgradeOption.UpgradeType.CHAOS);

        return candidateTypes.ToArray();
    }

    private Grid CreateUpgradeSlot(UpgradeOption.UpgradeType upgType, bool isNew,
        Dictionary<MapleStatus.StatusType, double>? chaosAvg = null)
    {
        string gridtag = isNew ? "Add" : "Sub";
        int width = isNew ? 300 : 200;
        Grid grid = new Grid
        {
            Width = width,
            Height = 32,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Tag = gridtag
        };
        UpgradeSlot slot = new UpgradeSlot
        {
            ChaosAverage = chaosAvg ?? new Dictionary<MapleStatus.StatusType, double>(),
            UpgradeType = upgType,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };
        Label label = new Label
        {
            Content = slot.TooltipManager.ToolTip.ToString(),
            Margin = new Thickness(36, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };
        Border clickArea = new Border
        {
            Width = width,
            Height = 32,
            Background = Brushes.Transparent,
            Tag = "Selector"
        };
            
        grid.Children.Add(clickArea);
        grid.Children.Add(slot);
        grid.Children.Add(label);
            
        grid.MouseLeftButtonDown += NewUpgradeSelected;
        return grid;
    }
    
    private void Update()
    {
        if (TargetItem == null) return;
        CurStatus.Content = $"현재 작 상태 ({cachedRemains} / {TargetItem.MaxUpgradeCount})";

        // Selfs
        CurUpgrades.Children.Clear();
        if (cachedUpgradeTypes != null)
        {
            foreach (var selfUpg in cachedUpgradeTypes)
            {
                if (selfUpg == UpgradeOption.UpgradeType.NONE) continue;
                Grid grid = CreateUpgradeSlot(selfUpg, false, cachedChaosAvg);
                CurUpgrades.Children.Add(grid);
            }
        }


        // Availables
        AvailableUpgrades.Children.Clear();
        foreach (var upg in GetAvailableList(TargetItem))
        {
            if (upg == UpgradeOption.UpgradeType.NONE) continue;
            Grid grid = CreateUpgradeSlot(upg, true);
            AvailableUpgrades.Children.Add(grid);
        }
    }

    private void NewUpgradeSelected(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Grid grid) return;
        if (selected != null)
        {
            foreach (var border in selected.GetChildren<Border>())
                if (border.Tag != null && border.Tag.ToString()!.Equals("Selector"))
                    border.Background = Brushes.Transparent;
        }
        foreach (var border in grid.GetChildren<Border>()) 
            if (border.Tag != null && border.Tag.ToString()!.Equals("Selector"))
                border.Background = SELECT_HIGHLIGHT;
        selected = grid;

        var upgType = grid.GetChildren<UpgradeSlot>().First();
        ChaosEditor.Visibility = upgType.UpgradeType == UpgradeOption.UpgradeType.CHAOS ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SaveClick(object sender, RoutedEventArgs e)
    {
        if (TargetItem == null) return;
        TargetItem.ChaosAverage = cachedChaosAvg;
        TargetItem.Upgrades = cachedUpgradeTypes;
        TargetItem.RemainUpgradeCount = cachedRemains;

        var args = new RoutedEventArgs(SavedEvent);
        RaiseEvent(args);
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
        var args = new RoutedEventArgs(SavedEvent);
        RaiseEvent(args);
    }

    private void CheckedChaosIsAmazing(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized || sender is not CheckBox cb) return;
        isChaosAmazing = cb.IsChecked!.Value;
    }
    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsInitialized || sender is not Slider slider || slider.Tag == null) return;

        foreach (var label in chaosLabels)
        {
            if (label.Tag == null || !label.Tag.ToString()!.Equals(slider.Tag.ToString())) continue;
            int plusIdx = label.Content.ToString()!.IndexOf('+');
            int sliderValue = (int) slider.Value == 5 && isChaosAmazing ? 6 : (int) slider.Value;
            if (sliderValue == 50 && isChaosAmazing) sliderValue = 60;
            label.Content = $"{label.Content.ToString()![..(plusIdx+1)]}{sliderValue}";
        }
    }

    private void AddUpgrade(object sender, RoutedEventArgs e)
    {
        if (TargetItem == null || selected?.Tag == null || !selected.Tag.ToString()!.Equals("Add") || cachedRemains == 0) return;

        var slot = selected.GetChildren<UpgradeSlot>().First();
        if (slot.UpgradeType == UpgradeOption.UpgradeType.CHAOS)
        {
            slot.ChaosAverage ??= new Dictionary<MapleStatus.StatusType, double>();
            foreach (var slider in ChaosEditor.GetChildren<Slider>())
            {
                if (!Enum.TryParse(slider.Tag.ToString(), out MapleStatus.StatusType statType)) continue;
                double sliderValue = slider.Value;
                if (isChaosAmazing) sliderValue = sliderValue == 5.0 ? 6.0 : sliderValue == 50.0 ? 60.0 : sliderValue; 
                slot.ChaosAverage.TryAdd(statType, sliderValue);
            }
        }
        // 혼줌 능력치 추가
        if (slot is {UpgradeType: UpgradeOption.UpgradeType.CHAOS, ChaosAverage: not null})
        {
            int chaosCnt = cachedUpgradeTypes!.Count(i => i == UpgradeOption.UpgradeType.CHAOS);
            var dict = slot.ChaosAverage;
            if (cachedChaosAvg != null)
            {
                foreach (var pair in cachedChaosAvg)
                {
                    double value = pair.Value * chaosCnt + dict.GetValueOrDefault(pair.Key, 0);
                    dict.TryAdd(pair.Key, 0);
                    dict[pair.Key] = value;
                }
            }
            foreach (var pair in dict) dict[pair.Key] /= chaosCnt + 1;
            cachedChaosAvg = dict;
        }

        for (int i = 0; i < cachedUpgradeTypes!.Length; i++)
        {
            if(cachedUpgradeTypes[i] != UpgradeOption.UpgradeType.NONE) continue;
            cachedUpgradeTypes[i] = slot.UpgradeType;
            cachedRemains--;
            break;
        }
        
        foreach (var border in selected.GetChildren<Border>())
            if (border.Tag != null && border.Tag.ToString()!.Equals("Selector"))
                border.Background = Brushes.Transparent;
        selected = null;
        
        Update();
    }

    private void SubtractUpgrade(object sender, RoutedEventArgs e)
    {
        if (selected?.Tag == null || !selected.Tag.ToString()!.Equals("Sub") || cachedUpgradeTypes == null) return;

        // 작 제거
        var slot = selected.GetChildren<UpgradeSlot>().First();
        int chaosCnt = cachedUpgradeTypes.Count(i => i == UpgradeOption.UpgradeType.CHAOS);
        for (int idx = 0; idx < cachedUpgradeTypes.Length; idx++)
        {
            if (cachedUpgradeTypes[idx] == slot.UpgradeType)
            {
                cachedUpgradeTypes[idx] = UpgradeOption.UpgradeType.NONE;
                break;
            }
        }
        
        // 혼줌 능력치 제거
        if (slot.UpgradeType == UpgradeOption.UpgradeType.CHAOS && cachedChaosAvg != null)
        {
            var dict = new Dictionary<MapleStatus.StatusType, double>();
            foreach (var pair in cachedChaosAvg)
            {
                double value = chaosCnt == 1 ? 0 : pair.Value * chaosCnt - slot.ChaosAverage!.GetValueOrDefault(pair.Key, 0);
                dict.TryAdd(pair.Key, 0);
                dict[pair.Key] = value;
            }

            if (chaosCnt > 1)
                foreach (var pair in dict) dict[pair.Key] /= chaosCnt - 1;

            cachedChaosAvg = dict;
        }
        
        
        foreach (var border in selected.GetChildren<Border>())
            if (border.Tag != null && border.Tag.ToString()!.Equals("Selector"))
                border.Background = Brushes.Transparent;
        selected = null;

        cachedRemains++;

        Update();
    }
}