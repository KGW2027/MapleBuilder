using System.Reflection.Emit;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleCashItem : MapleItem
{ 

    public MapleCashItem(JsonObject data)
    {
        IsEmpty = data["cash_item_name"] == null;
        if (IsEmpty) return;

        Name = data["cash_item_name"]!.ToString();
        DisplayName = $"{Name} (수정가능)";

        option = new MapleOption();
        LabelType = MapleCashEquip.LabelType.OTHER;
        EquipType = MapleEquipType.GetEquipType(data["cash_item_equipment_slot"]!.ToString().TrimEnd('1', '2', '3', '4'));
        
        JsonArray options = data["cash_item_option"]!.AsArray();
        foreach (var cashOption in options)
        {
            if (cashOption is not JsonObject optionObject) continue;
            
            int value = int.Parse(optionObject["option_value"]!.ToString());
            string optionType = optionObject["option_type"]!.ToString();

            if (optionType.Equals("STR")) option.Str += value;
            else if (optionType.Equals("DEX")) option.Dex += value;
            else if (optionType.Equals("INT")) option.Int += value;
            else if (optionType.Equals("LUK")) option.Luk += value;
            else if (optionType.Equals("공격력")) option.AttackPower += value;
            else if (optionType.Equals("마력")) option.MagicPower += value;
        }

    }
    
    public bool IsEmpty { get; private set; }
    public MapleOption option { get; private set; }
    public MapleCashEquip.LabelType LabelType { get; private set; }
    public MapleEquipType.EquipType EquipType { get; private set; }
}