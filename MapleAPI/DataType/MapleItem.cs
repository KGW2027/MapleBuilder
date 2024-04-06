using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleItem
{
    private string? hash;
    public string Hash
    {
        get
        {
            if (hash != null) return hash;
            byte[] bytes = Encoding.UTF8.GetBytes(data.ToString());
            byte[] hashBytes;
            using SHA256 sha256 = SHA256.Create();
            hashBytes = sha256.ComputeHash(bytes);
            hash = "";
            foreach (byte b in hashBytes)
                hash += b.ToString("x2");
            return hash;
        }
    }
    

    private readonly JsonObject data;
    public MapleEquipType.EquipType EquipType { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; set; }
    public uint StarForce { get; private set; }
    public uint SpecialRingLevel { get; private set; }
    public uint MaxUpgrade { get; private set; }
    public MapleOption? ExceptionalOption { get; private set; }
    public MapleOption? BaseOption { get; private set; }
    public MapleOption? AddOption { get; private set; }
    public MapleOption? EtcOption { get; private set; }
    public MapleItemPotential? Potential { get; private set; }
    public List<KeyValuePair<MaplePotentialOption.OptionType, int>> Specials { get; private set; }
    
    private MapleItem(JsonObject data)
    {
        this.data = data;
        Name = data["item_name"]!.ToString();
        DisplayName = $"{Name} (수정 가능)";
        MaxUpgrade =
            uint.TryParse(data["scroll_upgrade"]!.ToString(), out uint scroll) &&
            uint.TryParse(data["scroll_upgradeable_count"]!.ToString(), out uint upgradeable)
                ? scroll + upgradeable
                : 0;
        EquipType = MapleEquipType.GetEquipType(data["item_equipment_slot"]!.ToString());
        StarForce = uint.TryParse(data["starforce"]!.ToString(), out uint val) ? val : 0;
        SpecialRingLevel = uint.TryParse(data["special_ring_level"]!.ToString(), out uint val2) ? val2 : 0;
        BaseOption = new MapleOption(data["item_base_option"]!.AsObject());
        ExceptionalOption = new MapleOption(data["item_exceptional_option"]!.AsObject());
        AddOption = new MapleOption(data["item_add_option"]!.AsObject());
        EtcOption = new MapleOption(data["item_etc_option"]!.AsObject());
        Specials = new List<KeyValuePair<Enum.MaplePotentialOption.OptionType, int>>();

        if (int.TryParse(data["item_base_option"]!.AsObject()["base_equipment_level"]!.ToString(), out int level))
        {
            Potential = new MapleItemPotential(level, EquipType, 
                MaplePotentialGrade.GetPotentialGrade(data["potential_option_grade"]),
                MaplePotentialGrade.GetPotentialGrade(data["additional_potential_option_grade"]));

            for (int idx = 1; idx <= 3; idx++)
            {
                string index = $"potential_option_{idx}";
                if (data[index] != null)
                    Potential.SetPotential(idx-1, MaplePotentialOption.ParseOptionFromPotential(data[index]!.ToString()));
                index = $"additional_{index}";
                if (data[index] != null)
                    Potential.SetAdditional(idx-1, MaplePotentialOption.ParseOptionFromPotential(data[index]!.ToString()));
            }
        }
    }

    private MapleItem(string name, string desc)
    {
        Name = name;
        DisplayName = Name;
        StarForce = 0;
        SpecialRingLevel = 0;
        MaxUpgrade = 0;
        EquipType = MapleEquipType.EquipType.TITLE;
        data = new JsonObject
        {
            {"title", name},
            {"description", desc}
        };
        AddOption = new MapleOption();
        Specials = new List<KeyValuePair<MaplePotentialOption.OptionType, int>>();
        foreach (string line in desc.Split("\n"))
        {
            foreach (string sep in line.Split(","))
            {
                var trim = sep.Trim();
                int plusIndex = trim.IndexOf('+');
                if (plusIndex < 0) continue;
                string value = trim[plusIndex..].Trim();
                string key = trim[..plusIndex];
                foreach (string prefix in key.Split("/"))
                {
                    var pair = MaplePotentialOption.ParseOptionFromPotential($"{prefix.Trim()}{value}");
                    if(pair.Key != MaplePotentialOption.OptionType.OTHER) Specials.Add(pair);
                }
            }
        }
    }



    public static MapleItem Parse(JsonObject data)
    {
        return data.ContainsKey("title_name") ? new MapleItem(data["title_name"]!.ToString(), data["title_description"]!.ToString()) : new MapleItem(data);
    }
}