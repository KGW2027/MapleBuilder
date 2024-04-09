using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType.Item;

public class MapleCashItem : MapleItemBase
{ 

    public MapleCashItem(JsonObject data) : base(data)
    {
        internalData = data;
        IsEmpty = data["cash_item_name"] == null;
        if (IsEmpty) return;

        Name = data["cash_item_name"]!.ToString();
        DisplayName = $"{Name} (수정가능)";

        LabelType = MapleCashEquip.LabelType.OTHER;
        EquipType = MapleEquipType.GetEquipType(data["cash_item_equipment_slot"]!.ToString().TrimEnd('1', '2', '3', '4'));
        
        JsonArray options = data["cash_item_option"]!.AsArray();
        foreach (var cashOption in options)
        {
            if (cashOption is not JsonObject optionObject) continue;
            
            int value = int.Parse(optionObject["option_value"]!.ToString());
            string optionType = optionObject["option_type"]!.ToString();

            if (optionType.Equals("STR")) Status[MapleStatus.StatusType.STR] += value;
            else if (optionType.Equals("DEX")) Status[MapleStatus.StatusType.DEX] += value;
            else if (optionType.Equals("INT")) Status[MapleStatus.StatusType.INT] += value;
            else if (optionType.Equals("LUK")) Status[MapleStatus.StatusType.LUK] += value;
            else if (optionType.Equals("공격력")) Status[MapleStatus.StatusType.ATTACK_POWER] += value;
            else if (optionType.Equals("마력")) Status[MapleStatus.StatusType.MAGIC_POWER] += value;
        }
    }
    
    public MapleCashEquip.LabelType LabelType { get; private set; }
}