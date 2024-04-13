using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private HashSet<string> cachedHashes;

    public bool RegisterItem(MapleItemBase itemBase, out ItemBase? outItem, string author = "")
    {
        outItem = null;
        if (!cachedHashes.Add(itemBase.Hash)) return false;

        if (itemBase is MapleCommonItem commonItem)
        {
            CraftableItem cItem = GetCraftableItem(commonItem);
            if (author != "") cItem.DisplayName += $" (from {author})";
            CachedItemList.Add(cItem);
            outItem = cItem;
            return true;
        }

        return false;
    }

    private CraftableItem GetCraftableItem(MapleCommonItem commonItem)
    {
        CraftableItem item = new CraftableItem(commonItem.EquipType, commonItem.Name);
        item.StatContainer += commonItem.Status;
        
        return item;
    }

}