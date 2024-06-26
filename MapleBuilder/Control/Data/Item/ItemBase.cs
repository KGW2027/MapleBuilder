﻿using System;
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
    public string ItemHash;
    public MapleEquipType.EquipType EquipType;
    public MapleStatContainer DefaultStats;
    public int ItemLevel;

    public EquipmentData? EquipData;

    protected ItemBase(MapleItemBase itemBase)
    {
        UniqueName = DisplayName = itemBase.Name;
        EquipType = itemBase.EquipType;
        WzDatabase.Instance.EquipmentDataList.TryGetValue(UniqueName, out EquipData);
        ItemHash = itemBase.Hash;
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

    protected ItemBase(EquipmentData data)
    {
        UniqueName = DisplayName = data.Name;
        EquipType = data.ISlot switch
        {
            "Af" => MapleEquipType.EquipType.FACE,
            "Ay" => MapleEquipType.EquipType.EYE,
            "Ae" => MapleEquipType.EquipType.EARRING,
            "Pe" => MapleEquipType.EquipType.PENDANT,
            "Be" => MapleEquipType.EquipType.BELT,
            "Me" => MapleEquipType.EquipType.MEDAL,
            "Sh" => MapleEquipType.EquipType.SHOULDER,
            "Po" => MapleEquipType.EquipType.POCKET,
            "Ba" => MapleEquipType.EquipType.BADGE,
            "Cp" => MapleEquipType.EquipType.HELMET,
            "Sr" => MapleEquipType.EquipType.CAPE,
            "Ma" => MapleEquipType.EquipType.TOP,
            "Gv" => MapleEquipType.EquipType.GLOVE,
            "MaPn" => MapleEquipType.EquipType.TOP_BOTTOM,
            "Pn" => MapleEquipType.EquipType.BOTTOM,
            "Ri" => MapleEquipType.EquipType.RING,
            "So" => MapleEquipType.EquipType.BOOT,
            "Wp" => MapleEquipType.EquipType.WEAPON,
            "WpSi" => MapleEquipType.EquipType.WEAPON,
            "Si" => data.IconPath!.Contains("Accessory") ? MapleEquipType.EquipType.EMBLEM : MapleEquipType.EquipType.SUB_WEAPON,
            _ => throw new ArgumentOutOfRangeException()
        };
        EquipData = data;
        ItemHash = data.DataHash;
        ItemLevel = data.Level;
        DefaultStats = data.GetStatus();
    }

#pragma warning disable CS8618
    protected ItemBase() { }
#pragma warning restore CS8618

    public abstract MapleStatContainer GetItemStatus();
    public abstract MapleStatContainer GetUpStatus();
    public abstract ItemBase Clone();

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
        else if (itemBase is MaplePetItem petEquip)
        {
            return new PetItem(petEquip);
        }

        return null;
    }
}