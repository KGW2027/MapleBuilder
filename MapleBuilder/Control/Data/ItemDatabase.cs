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
    private readonly HashSet<string> cachedHashes;

    public bool RegisterItem(MapleItemBase itemBase, out ItemBase? outItem, string author = "")
    {
        outItem = null;
        if (!cachedHashes.Add(itemBase.Hash)) return false;
        
        ItemBase? parsedItem = ItemBase.ParseItemBase(itemBase);
        if (parsedItem == null) return false;
        parsedItem.DisplayName += $" (by {author})";
        CachedItemList.Add(parsedItem);
        return true;
    }

}