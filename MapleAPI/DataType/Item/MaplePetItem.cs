using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType.Item;

public class MaplePetItem : MapleItemBase
{
    public MaplePetItem(JsonObject data, MaplePetType.PetType petType) : base(data)
    {
        internalData = data;
        IsEmpty = data["item_name"] == null;
        if (IsEmpty) return;

        Name = data["item_name"]!.ToString();
        int upgrade = int.Parse(data["scroll_upgrade"]!.ToString());
        int upgradable = int.Parse(data["scroll_upgradable"]!.ToString());
        MaxUpgrade = upgradable + upgrade;
        if (upgrade > 0) DisplayName = $"{Name} (+{upgrade})";

        JsonArray options = data["item_option"]!.AsArray();
        foreach (var option in options)
        {
            if (option is not JsonObject optionObject) continue;
            
            int value = int.Parse(optionObject["option_value"]!.ToString());
            string optionType = optionObject["option_type"]!.ToString();

            if (optionType == "공격력") Status[MapleStatus.StatusType.ATTACK_POWER] = value;
            else if (optionType == "마력") Status[MapleStatus.StatusType.MAGIC_POWER] = value;
        }

        PetType = petType;
    }
    
    public MaplePetType.PetType PetType { get; private set; }
}