using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;

namespace MapleAPI.Info;

public class MapleCashEquipInfo : MapleInfo
{
    public MapleCashEquipInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        CashEquips = new List<MapleCashItem>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.CASH_ITEM;
    }

    public readonly List<MapleCashItem> CashEquips;

    private protected override void ParseInfo()
    {
        if (!TryGet("cash_item_equipment_base", out JsonArray? cashItems) || cashItems == null) return;
        foreach (var cashItemNode in cashItems)
        {
            if (cashItemNode is not JsonObject cashItemData) continue;
            MapleCashItem cashItem = new MapleCashItem(cashItemData);
            if (!cashItem.IsEmpty) CashEquips.Add(cashItem);
        }
    }
}