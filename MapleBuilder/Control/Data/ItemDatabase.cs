using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MapleAPI.DataType.Item;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.Control.Data;

public class ItemDatabase
{
    private static ItemDatabase? _self;

    public static ItemDatabase Instance
    {
        get { return _self ??= new ItemDatabase(); }
    }

    private ItemDatabase()
    {
        CachedItemList = new ObservableCollection<ItemBase>();
        cachedHashes = new HashSet<string>();
    }

    public readonly ObservableCollection<ItemBase> CachedItemList;
    private readonly HashSet<string> cachedHashes;

    public bool RegisterItem(MapleItemBase itemBase, out ItemBase? outItem, string author = "")
    {
        outItem = null;
        if (!cachedHashes.Add(itemBase.Hash)) return false;
        
        ItemBase? parsedItem = ItemBase.ParseItemBase(itemBase);
        if (parsedItem == null) return false;
        parsedItem.DisplayName += $" (by {author})";
        CachedItemList.Add(parsedItem);
        outItem = parsedItem;
        return true;
    }

    public bool RegisterItem(EquipmentData? equipData, out ItemBase? outItem)
    {
        outItem = null;
        if (equipData == null) return false;
        if (!cachedHashes.Add(equipData.DataHash)) return false;
        CommonItem parsedItem = new CommonItem(equipData);
        parsedItem.DisplayName += " (추가됨)";
        CachedItemList.Add(parsedItem);
        outItem = parsedItem;
        return true;
    }

    public static bool TryFindItemFromHash(string hash, out ItemBase? outItem)
    {
        outItem = null;
        if (!Instance.cachedHashes.Contains(hash))
        {
            Debug.WriteLine($"Failed to find Instance.cachedHashes from ");
            return false;
        }
        outItem = Instance.CachedItemList.FirstOrDefault(i => i.ItemHash.Equals(hash));
        return true;
    }

}