using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.View.SubFrames;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.Control;

public class BuilderDataContainer
{
    private static CharacterInfo? charInfo;
    public static CharacterInfo? CharacterInfo
    {
        get => charInfo;
        set {
            charInfo = value;
            UpdateCharacterInfo();
        }
    }

    public static PlayerInfo? PlayerStatus { get; private set; }

    private static HashSet<string> RegisteredItemHashes = new();
    private static List<EquipmentSlot> Equipments = new();
    
    public static ObservableCollection<MapleItem> RegisterItems = new();

    private static void UpdateCharacterInfo()
    {
        if (charInfo == null) return;
        foreach (MapleItem newItem in charInfo.Items)
        {
            if (!RegisteredItemHashes.Add(newItem.Hash)) continue;
            RegisterItems.Add(newItem);
        }
        
        MaplePotentialOptionType[] statTypes = CharacterInfo.GetClassStatType(charInfo.Class);
        PlayerStatus = new PlayerInfo(charInfo.Level, statTypes[0], statTypes[1], statTypes[2]);
        RenderOverview.Update(charInfo);
    }

    public static void InitEquipmentSlots(List<UIElement> slots)
    {
        Equipments.Clear();
        foreach(var element in slots)
            if (element is EquipmentSlot slot)
            {
                slot.itemChanged += OnSlotItemChanged;
                Equipments.Add(slot);
            }
    }

    private static void OnSlotItemChanged(MapleItem? prevItem, MapleItem? newItem)
    {
        if (prevItem != null)
            PlayerStatus!.SubtractItem(prevItem);
        if (newItem != null)
            PlayerStatus!.AddItem(newItem);
        
        RenderOverview.UpdateSetDisplay(PlayerStatus!.SetEffects.GetSetOptionString());
        Summarize.DispatchSummary();
    }
}