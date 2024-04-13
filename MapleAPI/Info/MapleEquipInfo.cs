using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;

namespace MapleAPI.Info;

public class MapleEquipInfo : MapleInfo
{
    public MapleEquipInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        itemHashes = new HashSet<string>();
        Items = new List<MapleItemBase>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.ITEM;
    }

    private readonly HashSet<string> itemHashes;
    public readonly List<MapleItemBase> Items;

    private protected override void ParseInfo()
    {
        for (int idx = 1; idx <= 3; idx++)
        {
            if (!TryGet($"item_equipment_preset_{idx}", out JsonArray? itemList) || itemList == null) continue;
            foreach (var id in itemList)
            {
                MapleCommonItem item = new MapleCommonItem(id!.AsObject());
                if (!itemHashes.Add(item.Hash)) continue;
                Items.Add(item);
            }
        }
        Items.Add(new MapleTitleItem(Get<JsonObject>("title")!));
        
        OnFinishParseInfo!.Invoke();
    }
}