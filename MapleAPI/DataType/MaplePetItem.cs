using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MaplePetItem : MapleItem
{
    public MaplePetItem(JsonObject data, MaplePetType.PetType petType)
    {
        this.data = data;
        IsEmpty = data["item_name"] == null;
        if (IsEmpty) return;

        Name = data["item_name"]!.ToString();
        int upgrade = int.Parse(data["scroll_upgrade"]!.ToString());
        int upgradable = int.Parse(data["scroll_upgradable"]!.ToString());
        MaxUpgrade = upgradable + upgrade;
        if (upgrade > 0) DisplayName = $"{Name} (+{upgrade})";

        Attack = 0;
        Magic = 0;
        JsonArray options = data["item_option"]!.AsArray();
        foreach (var option in options)
        {
            if (option is not JsonObject optionObject) continue;
            
            int value = int.Parse(optionObject["option_value"]!.ToString());
            string optionType = optionObject["option_type"]!.ToString();

            if (optionType == "공격력") Attack = value;
            else if (optionType == "마력") Magic = value;
        }

        PetType = petType;
    }
    
    public bool IsEmpty { get; private set; }
    public int Attack { get; private set; }
    public int Magic { get; private set; }
    public MaplePetType.PetType PetType { get; private set; }
}