using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item;

#pragma warning disable CS8604

public abstract class ItemBase
{
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

    public abstract MapleStatContainer GetItemStatus();

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
        if (itemBase is MapleCommonItem)
        {
            switch (itemBase.EquipType)
            {
                case MapleEquipType.EquipType.PENDANT:
                case MapleEquipType.EquipType.FACE:
                case MapleEquipType.EquipType.EYE:
                case MapleEquipType.EquipType.EARRING:
                case MapleEquipType.EquipType.BELT:
                case MapleEquipType.EquipType.HEART:
                    break;
                case MapleEquipType.EquipType.RING:
                    break;
                case MapleEquipType.EquipType.SHOULDER:
                    break;
                case MapleEquipType.EquipType.POCKET:
                    break;
                case MapleEquipType.EquipType.ANDROID:
                    break;
                case MapleEquipType.EquipType.BADGE:
                case MapleEquipType.EquipType.MEDAL:
                    break;
                case MapleEquipType.EquipType.WEAPON:
                    break;
                case MapleEquipType.EquipType.SUB_WEAPON:
                    break;
                case MapleEquipType.EquipType.EMBLEM:
                    break;
                case MapleEquipType.EquipType.HELMET:
                case MapleEquipType.EquipType.TOP:
                case MapleEquipType.EquipType.BOTTOM:
                case MapleEquipType.EquipType.TOP_BOTTOM:
                case MapleEquipType.EquipType.GLOVE:
                case MapleEquipType.EquipType.CAPE:
                case MapleEquipType.EquipType.BOOT:
                    return new ArmorItem((itemBase as MapleCommonItem)!);
                case MapleEquipType.EquipType.TITLE:
                    break;
                default:
                    return null;
            }
        }

        return null;
    }
}