using System.Reflection.Emit;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleCashItem : MapleItem
{ 

    public MapleCashItem(JsonObject data)
    {
        this.data = data;
        IsEmpty = data["cash_item_name"] == null;
        Option = new MapleOption();
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

            if (optionType.Equals("STR")) Option.Str += value;
            else if (optionType.Equals("DEX")) Option.Dex += value;
            else if (optionType.Equals("INT")) Option.Int += value;
            else if (optionType.Equals("LUK")) Option.Luk += value;
            else if (optionType.Equals("공격력")) Option.AttackPower += value;
            else if (optionType.Equals("마력")) Option.MagicPower += value;
        }

    }
    
    public bool IsEmpty { get; private set; }
    public MapleOption Option { get; private set; }
    public MapleCashEquip.LabelType LabelType { get; private set; }
}