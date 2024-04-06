using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleItem
{
    /// <summary>
    /// Nexon API에서 가져온 JSON 객체를 토대로 구성한 MapleItem 객체
    /// </summary>
    /// <param name="data">NEXON API RESULT</param>
    private MapleItem(JsonObject data)
    {
        this.data = data;
        Name = data["item_name"]!.ToString();
        DisplayName = $"{Name} (수정 가능)";
        MaxUpgrade =
            int.TryParse(data["scroll_upgrade"]!.ToString(), out int scroll) &&
            int.TryParse(data["scroll_upgradeable_count"]!.ToString(), out int upgradeable)
                ? scroll + upgradeable
                : 0;
        EquipType = MapleEquipType.GetEquipType(data["item_equipment_slot"]!.ToString());
        SpecialRingLevel = int.TryParse(data["special_ring_level"]!.ToString(), out int val2) ? val2 : 0;
        BaseOption = new MapleOption(data["item_base_option"]!.AsObject());
        ItemLevel = int.TryParse(data["item_base_option"]!.AsObject()["base_equipment_level"]!.ToString(),
            out int val3)
            ? val3
            : 0;
        ExceptionalOption = new MapleOption(data["item_exceptional_option"]!.AsObject());
        AddOption = new MapleOption(data["item_add_option"]!.AsObject());
        EtcOption = new MapleOption(data["item_etc_option"]!.AsObject());
        Specials = new List<KeyValuePair<Enum.MaplePotentialOption.OptionType, int>>();
        
        CachedStarforce = new Dictionary<int, MapleOption>();
        StarForce = int.TryParse(data["starforce"]!.ToString(), out int val) ? val : 0;

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

    /// <summary>
    /// Nexon API에서 가져온 JSON 객체에서 TITLE(칭호) 부분을 토대로 구성한 MapleItem 객체
    /// </summary>
    /// <param name="name">NEXON API RESULT</param>
    /// <param name="desc">NEXON API RESULT</param>
    private MapleItem(string name, string desc)
    {
        Name = name;
        DisplayName = Name;
        SpecialRingLevel = 0;
        MaxUpgrade = 0;
        ItemLevel = 0;
        EquipType = MapleEquipType.EquipType.TITLE;
        data = new JsonObject
        {
            {"title", name},
            {"description", desc}
        };
        AddOption = new MapleOption();
        Specials = new List<KeyValuePair<MaplePotentialOption.OptionType, int>>();
        
        CachedStarforce = new Dictionary<int, MapleOption>();
        StarForce = 0;
        
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
    
    private string? hash;
    /// <summary>
    /// MapleItem 객체간 비교를 위한 SHA256 Hash 데이터
    /// </summary>
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

    private int sfv; // star force value
    public int StarForce
    {
        get => sfv;
        set
        {
            sfv = value;
            StarforceOption = ParseStarforceOption();
        }
    }
    

    private readonly JsonObject data;
    public MapleEquipType.EquipType EquipType { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; set; }
    public int ItemLevel { get; private set; }
    public int SpecialRingLevel { get; private set; }
    public int MaxUpgrade { get; private set; }
    public MapleOption? ExceptionalOption { get; private set; }
    public MapleOption? BaseOption { get; private set; }
    public MapleOption? AddOption { get; private set; }
    public MapleOption? EtcOption { get; private set; }
    public MapleOption? StarforceOption { get; private set; }
    public MapleItemPotential? Potential { get; private set; }
    public List<KeyValuePair<MaplePotentialOption.OptionType, int>> Specials { get; private set; }
    private Dictionary<int, MapleOption> CachedStarforce { get; }

    public static MapleItem Parse(JsonObject data)
    {
        return data.ContainsKey("title_name") ? new MapleItem(data["title_name"]!.ToString(), data["title_description"]!.ToString()) : new MapleItem(data);
    }

    #region 스타포스 시뮬레이트
    private int GetStatIncreaseByStarforce(int starforce)
    {
        if (starforce <= 5) return 2;
        if (starforce <= 15) return 3;
        if (starforce >= 23) return 0;
        if (ItemLevel >= 250) return 17;
        if (ItemLevel >= 200) return 15;
        if (ItemLevel >= 160) return 13;
        if (ItemLevel >= 150) return 11;
        if (ItemLevel >= 140) return 9;
        if (ItemLevel >= 130) return 7;
        return 0;
    }

    private int GetWeaponAttackIncreaseByStarforce(int starforce, int increase)
    {
        bool isMage = BaseOption!.AttackPower < BaseOption.MagicPower;
        if(starforce <= 15)
        {
            int refValue = isMage
                ? BaseOption.MagicPower + EtcOption!.MagicPower
                : BaseOption.AttackPower + EtcOption!.MagicPower;
            refValue += increase;
            return (int) Math.Floor(refValue / 50.0) + 1;
        }

        if (ItemLevel >= 198)
        {
            return starforce switch
            {
                16 => 13,
                17 => 13,
                18 => 14,
                19 => 14,
                20 => 15,
                21 => 16,
                22 => 17,
                23 => 34,
                24 => 35,
                25 => 36,
                _ => 0
            };
        }
        
        if (ItemLevel >= 158)
        {
            return starforce switch
            {
                16 => 9,
                17 => 9,
                18 => 10,
                19 => 11,
                20 => 12,
                21 => 13,
                22 => 14,
                23 => 32,
                24 => 33,
                25 => 34,
                _ => 0
            };
        }
        
        if (ItemLevel >= 148)
        {
            return starforce switch
            {
                16 => 8,
                17 => 9,
                18 => 9,
                19 => 10,
                20 => 11,
                21 => 12,
                22 => 13,
                23 => 31,
                24 => 32,
                25 => 33,
                _ => 0
            };
        }
        
        if (ItemLevel >= 138)
        {
            return starforce switch
            {
                16 => 7,
                17 => 8,
                18 => 8,
                19 => 9,
                20 => 10,
                21 => 11,
                22 => 12,
                23 => 30,
                24 => 31,
                25 => 32,
                _ => 0
            };
        }
        
        if (ItemLevel >= 128)
        {
            return starforce switch
            {
                16 => 6,
                17 => 7,
                18 => 7,
                19 => 8,
                20 => 9,
                _ => 0
            };
        }
        
        return 0;
    }

    private int GetArmorAttackIncreaseByStarforce(int starforce)
    {
        if (starforce <= 15) return 0;
        
        if (ItemLevel >= 198)
        {
            return starforce switch
            {
                16 => 12,
                17 => 13,
                18 => 14,
                19 => 15,
                20 => 16,
                21 => 17,
                22 => 19,
                23 => 21,
                24 => 23,
                25 => 25,
                _ => 0
            };
        }
        
        if (ItemLevel >= 158)
        {
            return starforce switch
            {
                16 => 10,
                17 => 11,
                18 => 12,
                19 => 13,
                20 => 14,
                21 => 15,
                22 => 17,
                23 => 19,
                24 => 21,
                25 => 23,
                _ => 0
            };
        }
        
        if (ItemLevel >= 148)
        {
            return starforce switch
            {
                16 => 9,
                17 => 10,
                18 => 11,
                19 => 12,
                20 => 13,
                21 => 14,
                22 => 16,
                23 => 18,
                24 => 20,
                25 => 22,
                _ => 0
            };
        }
        
        if (ItemLevel >= 138)
        {
            return starforce switch
            {
                16 => 8,
                17 => 9,
                18 => 10,
                19 => 11,
                20 => 12,
                21 => 13,
                22 => 15,
                23 => 17,
                24 => 19,
                25 => 21,
                _ => 0
            };
        }
        
        if (ItemLevel >= 128)
        {
            return starforce switch
            {
                16 => 7,
                17 => 8,
                18 => 9,
                19 => 10,
                20 => 11,
                _ => 0
            };
        }
        return 0;
    }
    
    public MapleOption ParseStarforceOption()
    {
        if (CachedStarforce.TryGetValue(sfv, out var opt)) return opt;
        MapleOption option = new MapleOption();
        if (sfv == 0) return option;

        for (int idx = 1; idx <= sfv; idx++)
        {
            option.AllStat += GetStatIncreaseByStarforce(idx);
            int apmp = EquipType == MapleEquipType.EquipType.WEAPON
                ? GetWeaponAttackIncreaseByStarforce(idx, option.AttackPower)
                : GetArmorAttackIncreaseByStarforce(idx);
            option.AttackPower += apmp;
            option.MagicPower += apmp;
        }
        return option;
    }
    #endregion
}