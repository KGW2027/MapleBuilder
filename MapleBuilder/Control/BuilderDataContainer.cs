using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
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
    
    public static ObservableCollection<MapleItemBase> RegisterItems = new();

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
        foreach (MapleItemBase newItem in charInfo.Items)
        {
            if (!RegisteredItemHashes.Add(newItem.Hash)) continue;
            RegisterItems.Add(newItem);
        }
        
        MapleStatus.StatusType[] statTypes = MapleClass.GetClassStatType(charInfo.Class);
        PlayerStatus = new PlayerInfo(charInfo.Level, statTypes[0], statTypes[1], statTypes[2]);
        PlayerStatus.ApplySymbolData(charInfo.SymbolLevels);
        PlayerStatus.ApplyPetItem(charInfo.PetInfo);
        StatSymbol.InitAbility(charInfo.AbilityValues);
        StatSymbol.InitHyperStat(charInfo.HyperStatLevels);
        RenderOverview.Update(charInfo);
        foreach (var unionInfo in charInfo.UnionInfo)
            UnionFrame.UpdateUnionRank(unionInfo.classType, unionInfo.raiderRank);
        UnionRaiderMap.UpdateUnionRaider(charInfo.UnionInfo, charInfo.UnionInner);
        UnionArtifactPanel.UpdateArtifactPanel(charInfo.ArtifactPanels);
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
    private static void OnSlotItemChanged(MapleItemBase? prevItem, MapleItemBase? newItem)
    {
        if (prevItem is MapleCommonItem mci)
            PlayerStatus!.SubCommonItem(mci);
        if (newItem is MapleCommonItem mci2)
            PlayerStatus!.AddCommonItem(mci2);
        
        RenderOverview.UpdateSetDisplay(PlayerStatus!.SetEffects.GetSetOptionString());
        Summarize.DispatchSummary();
    }
}