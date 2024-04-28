using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class AddOptionEditor : UserControl
{
    private readonly ComboBox[] comboBoxes;
    private readonly Slider[] sliders;
    private bool isInitializing;
    
    public AddOptionEditor()
    {
        InitializeComponent();

        comboBoxes = new ComboBox[4];
        int idx = 0;
        foreach (var box in Grid.GetChildren<ComboBox>())
        {
            comboBoxes[idx++] = box;
        }

        sliders = new Slider[4];
        idx = 0;
        foreach (var slider in Grid.GetChildren<Slider>())
        {
            sliders[idx++] = slider;
        }

        Reset();
    }

    private void Reset()
    {
        for (int idx = 0; idx < 4; idx++)
        {
            comboBoxes[idx].Items.Clear();
            comboBoxes[idx].Items.Add("없음");
            comboBoxes[idx].SelectedItem = "없음";
            sliders[idx].Minimum = sliders[idx].Maximum = sliders[idx].Value = 1.0;
        }
    }

    private void Init()
    {
        if (TargetItem == null) return;

        AddOptions.AddOptionType[] targetTypes = TargetItem.EquipType == MapleEquipType.EquipType.WEAPON
            ? AddOptions.WeaponOptions
            : AddOptions.ArmorOptions;
        for (int idx = 0; idx < 4; idx++)
        {
            foreach (var type in targetTypes)
                comboBoxes[idx].Items.Add(type.ToString());
            sliders[idx].Minimum = 1.0;
            sliders[idx].Maximum = 7.0;
            sliders[idx].Value = 1.0;
        }

        int idx2 = 0;
        foreach (var curOption in TargetItem.AddOptions!)
        {
            comboBoxes[idx2].SelectedItem = curOption.Key.ToString();
            sliders[idx2].Value = curOption.Value;
            idx2++;
        }
    }
    
    
    #region XAML Property
    
    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(CommonItem), typeof(AddOptionEditor), new PropertyMetadata(null, OnTargetItemChanged));

    private static void OnTargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AddOptionEditor) d;
        control.isInitializing = true;
        control.Reset();
        if (e.NewValue is not CommonItem cItem || cItem.AddOptions == null) return;
        control.Init();
        control.isInitializing = false;
    }

    public CommonItem? TargetItem
    {
        get => (CommonItem?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }

    public static readonly RoutedEvent ItemUpdatedEvent = EventManager.RegisterRoutedEvent(
        nameof(ItemUpdated), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddOptionEditor)
    );

    public event RoutedEventHandler ItemUpdated
    {
        add => AddHandler(ItemUpdatedEvent, value);
        remove => RemoveHandler(ItemUpdatedEvent, value);
    }

    #endregion

    private T? GetSameTagElement<T>(IEnumerable<T> container, string? tag) where T : System.Windows.Controls.Control
    {
        foreach (var value in container)
            if (value.Tag != null && value.Tag.Equals(tag)) return value;
        throw new Exception($"Cannot Found Tag::{tag}.");
    }
    

    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (isInitializing || TargetItem?.AddOptions == null) return;
        if (sender is not Slider slider || slider.Tag == null) return;
        ComboBox? pairComboBox = GetSameTagElement(comboBoxes, slider.Tag.ToString());
        if (pairComboBox == null) return;

        if (!Enum.TryParse(pairComboBox.SelectedItem.ToString(), out AddOptions.AddOptionType type) 
            || !TargetItem.AddOptions.ContainsKey(type)) return;

        TargetItem.AddOptions[type] = (int) e.NewValue;
        
        RaiseEvent(new RoutedEventArgs(ItemUpdatedEvent));
    }
    

    private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;
        if (isInitializing || TargetItem?.AddOptions == null) return;
        if (sender is not ComboBox box || box.Tag == null) return;
        Slider? pairSlider = GetSameTagElement(sliders, box.Tag.ToString());
        if (pairSlider == null) return;

        if (Enum.TryParse(e.RemovedItems[0]!.ToString()!,
                out AddOptions.AddOptionType befType))
        {
            TargetItem.AddOptions.Remove(befType);
        }

        if (Enum.TryParse(e.AddedItems[0]!.ToString()!, out AddOptions.AddOptionType newType))
        {
            TargetItem.AddOptions.TryAdd(newType, 0);
            pairSlider.Maximum = 7.0;
            TargetItem.AddOptions[newType] = (int) pairSlider.Value;
        }
        else
        {
            pairSlider.Minimum = pairSlider.Maximum = pairSlider.Value = 1.0;
        }
        
        
        RaiseEvent(new RoutedEventArgs(ItemUpdatedEvent));
    }
}