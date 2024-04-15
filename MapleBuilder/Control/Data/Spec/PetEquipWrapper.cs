using System;
using System.Collections.Generic;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.Control.Data.Spec;

public class PetEquipWrapper : StatWrapper
{
    public PetEquipWrapper(StatusChanged onStatusChanged) : base(new Dictionary<MapleStatus.StatusType, double>(), onStatusChanged)
    {
        petItems = new PetItem?[] { null, null, null };
    }

    private readonly PetItem?[] petItems;

    public PetItem? this[int slot]
    {
        get => petItems[Math.Clamp(slot, 0, 2)];
        set
        {
            slot = Math.Clamp(slot, 0, 2);
            if(petItems[slot] != null)
                petItems[slot]!.UnequipItem(GlobalDataController.Instance.PlayerInstance!);
            petItems[slot] = value;
            value?.EquipItem(GlobalDataController.Instance.PlayerInstance!);
        }
    }

    protected override void CallStatusChanged(MapleStatus.StatusType statusType, double prev, double next)
    {
        
    }
}