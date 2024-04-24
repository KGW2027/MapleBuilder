using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.Control.Data.Spec;

public class EquipWrapper : StatWrapper
{
    public delegate void EquipmentChanged(MapleEquipType.EquipType type, int slot);

    public static EquipmentChanged? OnEquipmentChanged;
    
    public EquipWrapper(StatusChanged onStatusChanged) : base(new Dictionary<MapleStatus.StatusType, double>(), onStatusChanged)
    {
        commonEquips = new Dictionary<MapleEquipType.EquipType, ItemBase?>();
        SetEffect = new SetEffect();
        rings = new ItemBase?[] { null, null, null, null };
        pendants = new ItemBase?[] {null, null};
    }

    private static MapleStatContainer DEBUG_EquipContainer = new MapleStatContainer();

    private readonly Dictionary<MapleEquipType.EquipType, ItemBase?> commonEquips;
    private readonly ItemBase?[] rings;
    private readonly ItemBase?[] pendants;

    protected internal readonly SetEffect SetEffect;
    
    private PlayerData? PlayerData => GlobalDataController.Instance.PlayerInstance;

    public ItemBase? this[MapleEquipType.EquipType equipType, int slot]
    {
        get
        {
            if (equipType == MapleEquipType.EquipType.RING)
                return rings[Math.Clamp(slot, 0, 3)];
            if (equipType == MapleEquipType.EquipType.PENDANT)
                return pendants[Math.Clamp(slot, 0, 1)];
            return commonEquips.GetValueOrDefault(equipType, null);
        }
        set
        {
            ItemBase? prevItem = this[equipType, slot];
            MapleStatContainer prevSetEffect = SetEffect.GetSetOptions();
            if (prevItem != null)
            {
                prevItem.UnequipItem(PlayerData!);
                SetEffect.Sub(prevItem);
            }

            if (value != null)
            {
                value.EquipItem(PlayerData!);
                SetEffect.Add(value);
            }

            if (equipType == MapleEquipType.EquipType.RING)
                rings[Math.Clamp(slot, 0, 3)] = value;
            else if (equipType == MapleEquipType.EquipType.PENDANT)
                pendants[Math.Clamp(slot, 0, 1)] = value;
            else
            {
                commonEquips.TryAdd(equipType, null);
                commonEquips[equipType] = value;
            }
            
            OnEquipmentChanged!.Invoke(equipType, slot);
            SetChanged(prevSetEffect, SetEffect.GetSetOptions());
        }
    }

    private void SetChanged(MapleStatContainer prevSetEffect, MapleStatContainer currSetEffect)
    {
        PlayerData![PlayerData.StatSources.EQUIPMENT] += (currSetEffect - prevSetEffect);
    }

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        
    }
}