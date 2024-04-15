using System;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item;

#pragma warning disable CS8604

public abstract class ItemBase
{
    [Flags]
    public enum ItemFlag
    {
        ADD_OPTION = 1 << 0,
        POTENTIAL  = 1 << 1,
        SOUL_ENCHANT = 1 << 2,
        STARFORCE  = 1 << 3,
        UPGRADE    = 1 << 4,
        SPECIAL    = 1 << 5,
    }
    
    public string UniqueName;
    public string DisplayName;
    public MapleEquipType.EquipType EquipType;
    public MapleStatContainer DefaultStats;
    public int ItemLevel;

    public EquipmentData? EquipData;

    protected ItemBase(MapleItemBase itemBase)
    {
        UniqueName = DisplayName = itemBase.Name;
        EquipType = itemBase.EquipType;
        WzDatabase.Instance.EquipmentDataList.TryGetValue(UniqueName, out EquipData);
        if (EquipData != null)
        {
            ItemLevel = EquipData.Level;
            DefaultStats = EquipData.GetStatus();
        }
        else
        {
            ItemLevel = 0;
            DefaultStats = new MapleStatContainer();
        }
    }

    protected abstract MapleStatContainer GetItemStatus();

    public MapleStatContainer DEBUG_GetItemStatus()
    {
        bool isRelease = true;
#if DEBUG
        isRelease = false;
#endif
        if (isRelease) throw new Exception("Executed debug mode only method on release mode.");
        return GetItemStatus();
    }

    public void EquipItem(PlayerData playerData)
    {
        if (playerData[PlayerData.StatSources.EQUIPMENT] == null)
            playerData[PlayerData.StatSources.EQUIPMENT] = new MapleStatContainer();
        playerData[PlayerData.StatSources.EQUIPMENT] += GetItemStatus();
    }

    public void UnequipItem(PlayerData playerData)
    {
        playerData[PlayerData.StatSources.EQUIPMENT] -= GetItemStatus();
    }

    public static ItemBase? ParseItemBase(MapleItemBase itemBase)
    {
        if (itemBase is MapleCommonItem commonBase)
        {
            switch (itemBase.EquipType)
            {
                case MapleEquipType.EquipType.HEART:
                case MapleEquipType.EquipType.RING:
                case MapleEquipType.EquipType.SHOULDER:
                    return new CommonItem(commonBase,
                        ItemFlag.UPGRADE | ItemFlag.POTENTIAL | ItemFlag.STARFORCE);
                case MapleEquipType.EquipType.POCKET:
                    return new CommonItem(commonBase, ItemFlag.ADD_OPTION);
                case MapleEquipType.EquipType.BADGE:
                case MapleEquipType.EquipType.MEDAL:
                    return new CommonItem(commonBase);
                case MapleEquipType.EquipType.WEAPON:
                    return new CommonItem(commonBase,
                        ItemFlag.UPGRADE | ItemFlag.ADD_OPTION | ItemFlag.POTENTIAL | ItemFlag.STARFORCE | ItemFlag.SOUL_ENCHANT);
                case MapleEquipType.EquipType.EMBLEM:
                    return new CommonItem(commonBase, ItemFlag.POTENTIAL);
                case MapleEquipType.EquipType.HELMET:
                case MapleEquipType.EquipType.TOP:
                case MapleEquipType.EquipType.BOTTOM:
                case MapleEquipType.EquipType.TOP_BOTTOM:
                case MapleEquipType.EquipType.GLOVE:
                case MapleEquipType.EquipType.CAPE:
                case MapleEquipType.EquipType.BOOT:
                case MapleEquipType.EquipType.PENDANT:
                case MapleEquipType.EquipType.FACE:
                case MapleEquipType.EquipType.EYE:
                case MapleEquipType.EquipType.EARRING:
                case MapleEquipType.EquipType.BELT:
                    return new CommonItem(commonBase,
                        ItemFlag.UPGRADE | ItemFlag.ADD_OPTION | ItemFlag.POTENTIAL | ItemFlag.STARFORCE);
                case MapleEquipType.EquipType.SUB_WEAPON:
                    if (itemBase.Name.EndsWith("블레이드") || itemBase.Name.EndsWith("실드") || itemBase.Name.EndsWith("프렐류드"))
                        return new CommonItem(commonBase, ItemFlag.UPGRADE | ItemFlag.POTENTIAL | ItemFlag.STARFORCE);
                    return new CommonItem(commonBase, ItemFlag.POTENTIAL);
                case MapleEquipType.EquipType.TITLE:
                case MapleEquipType.EquipType.ANDROID:
                    break;
            }
        }
        else if (itemBase is MapleTitleItem title)
        {
            return new TitleItem(title);
        }

        return null;
    }
}