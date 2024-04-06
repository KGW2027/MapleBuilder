﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.View.SubFrames;
using MapleBuilder.View.SubObjects;
using MaplePotentialOption = MapleAPI.Enum.MaplePotentialOption;

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

    /// <summary>
    ///    내부 데이터가 업데이트되었을 때, 디스플레이들에게 이를 알리는 역할을 합니다.
    /// </summary>
    public static void RefreshAll()
    {
        RenderFrame.UpdateCharacterTop();
        StatSymbol.Update();
        Summarize.DispatchSummary();
    }
    
    /// <summary>
    /// 새로운 캐릭터를 불러올 때 실행됩니다.
    /// 아이템을 캐시하고, 플레이어 정보를 새로 생성한 후 로드합니다.
    /// </summary>
    private static void UpdateCharacterInfo()
    {
        if (charInfo == null) return;
        foreach (MapleItem newItem in charInfo.Items)
        {
            if (!RegisteredItemHashes.Add(newItem.Hash)) continue;
            RegisterItems.Add(newItem);
        }
        
        MaplePotentialOption.OptionType[] statTypes = MapleClass.GetClassStatType(charInfo.Class);
        PlayerStatus = new PlayerInfo(charInfo.Level, statTypes[0], statTypes[1], statTypes[2]);
        PlayerStatus.ApplySymbolData(charInfo.SymbolLevels);
        RenderOverview.Update(charInfo);
        RefreshAll();
    }

    /// <summary>
    /// 프로그램 초기에만 실행됩니다. 슬롯에 아이템 교체 이벤트를 등록합니다.
    /// </summary>
    /// <param name="prevItem"></param>
    /// <param name="newItem"></param>
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

    /// <summary>
    /// 슬롯 아이템이 교체 되었을 때 실행됩니다.
    /// 세트효과와 요약이 새로고침됩니다.
    /// </summary>
    /// <param name="prevItem"></param>
    /// <param name="newItem"></param>
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