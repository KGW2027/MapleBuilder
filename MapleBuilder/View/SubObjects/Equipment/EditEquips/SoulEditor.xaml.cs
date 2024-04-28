using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class SoulEditor : UserControl
{
    public SoulEditor()
    {
        InitializeComponent();
        
        Prefixes.Items.Clear();
        Bosses.Items.Clear();
        Options.Items.Clear();

        foreach (var soul in Enum.GetValues<SoulWeapon.SoulType>())
            Bosses.Items.Add(SoulWeapon.GetSoulTypeString(soul));
    }

    #region XAML Property

    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(CommonItem), typeof(SoulEditor), new PropertyMetadata(null, OnTargetItemChanged)
        );

    private static void OnTargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SoulEditor) d;
        if (e.NewValue is not CommonItem cItem) return;
        if (cItem.SoulOption == null) return;

        control.Update();
    }

    public CommonItem? TargetItem
    {
        get => (CommonItem?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }
    
    #endregion

    private void Update()
    {
        if (TargetItem?.SoulName == null) return;
        string[] split = TargetItem.SoulName.Split(" ", 2);
        if (split.Length < 2) return;
        
        // 보스 소울 찾기
        bool bossFound = false;
        split[1] = split[1].Replace(" ", "");
        foreach (var item in Bosses.Items)
        {
            if (item == null || !item.ToString()!.Replace(" ", "").Equals(split[1])) continue;
            Bosses.SelectedItem = item;
            bossFound = true;
            break;
        }

        // 접두사 찾기
        if (!bossFound) return;
        bool prefixFound = false;
        foreach (var item in Prefixes.Items)
        {
            if (item == null || !item.ToString()!.Equals(split[0])) continue;
            Prefixes.SelectedItem = item;
            prefixFound = true;
            break;
        }

        // 소울 옵션 찾기
        if (!prefixFound || !SoulWeapon.SoulDescriptions.TryGetValue(TargetItem.SoulOption!.Value.Key, out string? text)) return;
        text = text.Replace("%d", TargetItem.SoulOption.Value.Value.ToString());
        foreach (var item in Options.Items)
        {
            if (item == null || !item.ToString()!.Equals(text)) continue;
            Options.SelectedItem = item;
            break;
        }
    }

    private SoulWeapon.SoulType GetSoulType(string overrideString = "")
    {
        if (string.IsNullOrEmpty(overrideString))
            overrideString = Bosses.SelectedItem == null ? Bosses.Text : Bosses.SelectedItem.ToString()!;
        return SoulWeapon.GetSoulType(overrideString);
    }

    private SoulWeapon.SoulPrefix GetSoulPrefix(string overrideString = "")
    {
        if (string.IsNullOrEmpty(overrideString))
            overrideString = Prefixes.SelectedItem == null ? Prefixes.Text : Prefixes.SelectedItem.ToString()!;
        return SoulWeapon.GetSoulPrefix(overrideString);
    }
    
    private void OnSoulBossChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TargetItem == null) return;
        if (e.AddedItems.Count < 0) return;
        SoulWeapon.SoulType bossType = GetSoulType(e.AddedItems[0]!.ToString()!);
        Prefixes.Items.Clear();
        foreach (var prefix in SoulWeapon.GetSoulPrefixes(bossType))
            Prefixes.Items.Add(SoulWeapon.GetSoulPrefixString(prefix));

        string[] split = TargetItem.SoulName!.Split(" ", 2);
        TargetItem.SoulName = $"{split[0]} {e.AddedItems[0]!}";
    }

    private void OnSoulPrefixChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TargetItem == null) return;
        if (e.AddedItems.Count < 0) return;
        SoulWeapon.SoulType bossType = GetSoulType();
        SoulWeapon.SoulPrefix prefix = GetSoulPrefix(e.AddedItems[0]!.ToString()!);

        Options.Items.Clear();
        foreach (var option in SoulWeapon.GetSoulOption(bossType, prefix))
        {
            string text = SoulWeapon.SoulDescriptions.GetValueOrDefault(option.Key, "");
            Options.Items.Add(text.Replace("%d", option.Value.ToString()));
        }
        
        string[] split = TargetItem.SoulName!.Split(" ", 2);
        TargetItem.SoulName = $"{e.AddedItems[0]} {split[1]}";
    }

    private void OnOptionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TargetItem == null) return;
        if (e.AddedItems.Count < 0) return;

        string text = e.AddedItems[0]!.ToString()!;
        foreach (var option in SoulWeapon.SoulDescriptions)
        {
            string[] split = option.Value.Split("%d");
            if (!text.StartsWith(split[0])) continue;
            int num = 0, idx = split[0].Length;
            while (idx < text.Length && '0' <= text[idx] && text[idx] <= '9') num = num * 10 + text[idx++] - '0';
            if (!option.Value.Replace("%d", num.ToString()).Trim().Equals(text.Trim())) continue;
            TargetItem.SoulOption = KeyValuePair.Create(option.Key, num);
            break;
        }
    }
}