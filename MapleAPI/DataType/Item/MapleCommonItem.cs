using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType.Item;

public class MapleCommonItem : MapleItemBase
{
    /// <summary>
    /// Nexon API에서 가져온 JSON 객체를 토대로 구성한 MapleItem 객체
    /// </summary>
    /// <param name="data">NEXON API RESULT</param>
    public MapleCommonItem(JsonObject data) : base(data)
    {
        internalData = data;
        IsEmpty = data["item_name"] == null;
        SoulName = "";
        
        ExceptionalOption = new MapleStatContainer();
        BaseOption = new MapleStatContainer();
        AddOption = new MapleStatContainer();
        UpgradeOption = new MapleStatContainer();
        StarforceOption = new MapleStatContainer();
        
        if (IsEmpty) return;
        
        Name = data["item_name"]!.ToString();
        DisplayName = $"{Name} (수정 가능)";
        UpgradeCount = int.TryParse(data["scroll_upgrade"]!.ToString(), out int scroll) ? scroll : 0;
        int goldenHammer = data["golden_hammer_flag"]!.ToString().Equals("미적용") ? 1 : 0;
        MaxUpgrade = int.TryParse(data["scroll_upgradeable_count"]!.ToString(), out int upgradeable)
                ? UpgradeCount + upgradeable + goldenHammer
                : 0;
        EquipType = MapleEquipType.GetEquipType(data["item_equipment_slot"]!.ToString());
        SpecialRingLevel = int.TryParse(data["special_ring_level"]!.ToString(), out int val2) ? val2 : 0;
        BaseOption = MapleStatContainer.LoadFromJson(data["item_base_option"]!.AsObject());
        ItemLevel = int.TryParse(data["item_base_option"]!.AsObject()["base_equipment_level"]!.ToString(),
            out int val3)
            ? val3
            : 0;
        ExceptionalOption = MapleStatContainer.LoadFromJson(data["item_exceptional_option"]!.AsObject());
        AddOption = MapleStatContainer.LoadFromJson(data["item_add_option"]!.AsObject());
        UpgradeOption = MapleStatContainer.LoadFromJson(data["item_etc_option"]!.AsObject());

        StarForce = int.TryParse(data["starforce"]!.ToString(), out int val) ? val : 0;
        StarforceOption = MapleStatContainer.LoadFromJson(data["item_starforce_option"]!.AsObject());

        Potential = new MapleItemPotential();
        
        for (int idx = 1; idx <= 3; idx++)
        {
            string index = $"potential_option_{idx}";
            if (data[index] != null)
                Potential.SetPotential(idx - 1, MaplePotentialOption.ParseOptionFromPotential(data[index]!.ToString()));
            index = $"additional_{index}";
            if (data[index] != null)
                Potential.SetAdditional(idx - 1,
                    MaplePotentialOption.ParseOptionFromPotential(data[index]!.ToString()));
        }

        if (data["soul_name"] != null && data["soul_option"] != null)
        {
            SoulName = data["soul_name"]!.ToString().Replace("의 소울 적용", "");
            string option = data["soul_option"]!.ToString();
            int colon = option.IndexOf(':');

            string optionName = option[..(colon-1)].Trim();
            string optionValue = option[(colon + 1)..].Replace("+", "").Replace("%", "");
            int optionValueInt = int.Parse(optionValue);

            MapleStatus.StatusType soulStatusType = optionName switch
            {
                "STR" => MapleStatus.StatusType.STR,
                "DEX" => MapleStatus.StatusType.DEX,
                "INT" => MapleStatus.StatusType.INT,
                "LUK" => MapleStatus.StatusType.LUK,
                "올스탯" => option[^1] == '%' ? MapleStatus.StatusType.ALL_STAT_RATE : MapleStatus.StatusType.ALL_STAT,
                "공격력" => option[^1] == '%' ? MapleStatus.StatusType.ATTACK_RATE : MapleStatus.StatusType.ATTACK_POWER,
                "마력" => option[^1] == '%' ? MapleStatus.StatusType.MAGIC_RATE : MapleStatus.StatusType.MAGIC_POWER,
                "최대 HP" => MapleStatus.StatusType.HP,
                "최대 MP" => MapleStatus.StatusType.MP,
                "몬스터 방어율 무시" => MapleStatus.StatusType.IGNORE_DEF,
                "보스 몬스터 공격 시 데미지" => MapleStatus.StatusType.BOSS_DAMAGE,
                "크리티컬 확률" => MapleStatus.StatusType.CRITICAL_CHANCE,
                _ => MapleStatus.StatusType.OTHER,
            };

            UpgradeOption[soulStatusType] += optionValueInt;
        }

    }
    
    public int StarForce;
    // {
    //     get => sfv;
    //     set
    //     {
    //         sfv = value;
    //         StarforceOption = ParseStarforceOption();
    //     }
    // }
    
    public int ItemLevel { get; private set; }
    public int SpecialRingLevel { get; private set; }
    public string SoulName { get; private set; }
    public MapleItemPotential? Potential { get; private set; }

    private MapleStatContainer ExceptionalOption { get; set; }
    private MapleStatContainer BaseOption { get; set; }
    private MapleStatContainer AddOption { get; set; }
    private MapleStatContainer UpgradeOption { get; set; }
    private MapleStatContainer StarforceOption { get; set; }

    public new MapleStatContainer Status => ExceptionalOption + BaseOption + AddOption + UpgradeOption + StarforceOption;
    // {
    //     get
    //     {
    //         MapleStatContainer sum = ExceptionalOption + BaseOption + AddOption + EtcOption + StarforceOption;
    //         Console.WriteLine($"{Name} | {BaseOption[MapleStatus.StatusType.MAGIC_POWER]} + {AddOption[MapleStatus.StatusType.MAGIC_POWER]} + {EtcOption[MapleStatus.StatusType.MAGIC_POWER]} + {StarforceOption[MapleStatus.StatusType.MAGIC_POWER]}");
    //         return sum;
    //     }
    // }


    // #region 스타포스 시뮬레이트
    // private int GetStatIncreaseByStarforce(int starforce)
    // {
    //     if (starforce <= 5) return 2;
    //     if (starforce <= 15) return 3;
    //     if (starforce >= 23) return 0;
    //     if (ItemLevel >= 248) return 17;
    //     if (ItemLevel >= 198) return 15;
    //     if (ItemLevel >= 158) return 13;
    //     if (ItemLevel >= 148) return 11;
    //     if (ItemLevel >= 138) return 9;
    //     if (ItemLevel >= 128) return 7;
    //     return 0;
    // }
    //
    // private int GetWeaponAttackIncreaseByStarforce(int starforce, int increase)
    // {
    //     bool isMage = BaseOption[MapleStatus.StatusType.ATTACK_POWER] < BaseOption[MapleStatus.StatusType.MAGIC_POWER];
    //     if(starforce <= 15)
    //     {
    //         double refValue = isMage
    //             ? BaseOption[MapleStatus.StatusType.MAGIC_POWER] + EtcOption[MapleStatus.StatusType.MAGIC_POWER]
    //             : BaseOption[MapleStatus.StatusType.ATTACK_POWER] + EtcOption[MapleStatus.StatusType.MAGIC_POWER];
    //         refValue += increase;
    //         return (int) Math.Floor(refValue / 50.0) + 1;
    //     }
    //
    //     if (ItemLevel >= 198)
    //     {
    //         return starforce switch
    //         {
    //             16 => 13,
    //             17 => 13,
    //             18 => 14,
    //             19 => 14,
    //             20 => 15,
    //             21 => 16,
    //             22 => 17,
    //             23 => 34,
    //             24 => 35,
    //             25 => 36,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 158)
    //     {
    //         return starforce switch
    //         {
    //             16 => 9,
    //             17 => 9,
    //             18 => 10,
    //             19 => 11,
    //             20 => 12,
    //             21 => 13,
    //             22 => 14,
    //             23 => 32,
    //             24 => 33,
    //             25 => 34,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 148)
    //     {
    //         return starforce switch
    //         {
    //             16 => 8,
    //             17 => 9,
    //             18 => 9,
    //             19 => 10,
    //             20 => 11,
    //             21 => 12,
    //             22 => 13,
    //             23 => 31,
    //             24 => 32,
    //             25 => 33,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 138)
    //     {
    //         return starforce switch
    //         {
    //             16 => 7,
    //             17 => 8,
    //             18 => 8,
    //             19 => 9,
    //             20 => 10,
    //             21 => 11,
    //             22 => 12,
    //             23 => 30,
    //             24 => 31,
    //             25 => 32,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 128)
    //     {
    //         return starforce switch
    //         {
    //             16 => 6,
    //             17 => 7,
    //             18 => 7,
    //             19 => 8,
    //             20 => 9,
    //             _ => 0
    //         };
    //     }
    //     
    //     return 0;
    // }
    //
    // private int GetArmorAttackIncreaseByStarforce(int starforce)
    // {
    //     if (starforce <= 15)
    //     {
    //         if (EquipType != MapleEquipType.EquipType.GLOVE) return 0;
    //         return starforce is 5 or 7 or 9 or 11 or 13 or 14 or 15 ? 1 : 0;
    //     }
    //
    //     if (ItemLevel >= 248)
    //     {
    //         return starforce switch
    //         {
    //             16 => 14,
    //             17 => 15,
    //             18 => 16,
    //             19 => 17,
    //             20 => 18,
    //             21 => 19,
    //             22 => 21,
    //             23 => 23,
    //             24 => 25,
    //             25 => 27,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 198)
    //     {
    //         return starforce switch
    //         {
    //             16 => 12,
    //             17 => 13,
    //             18 => 14,
    //             19 => 15,
    //             20 => 16,
    //             21 => 17,
    //             22 => 19,
    //             23 => 21,
    //             24 => 23,
    //             25 => 25,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 158)
    //     {
    //         return starforce switch
    //         {
    //             16 => 10,
    //             17 => 11,
    //             18 => 12,
    //             19 => 13,
    //             20 => 14,
    //             21 => 15,
    //             22 => 17,
    //             23 => 19,
    //             24 => 21,
    //             25 => 23,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 148)
    //     {
    //         return starforce switch
    //         {
    //             16 => 9,
    //             17 => 10,
    //             18 => 11,
    //             19 => 12,
    //             20 => 13,
    //             21 => 14,
    //             22 => 16,
    //             23 => 18,
    //             24 => 20,
    //             25 => 22,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 138)
    //     {
    //         return starforce switch
    //         {
    //             16 => 8,
    //             17 => 9,
    //             18 => 10,
    //             19 => 11,
    //             20 => 12,
    //             21 => 13,
    //             22 => 15,
    //             23 => 17,
    //             24 => 19,
    //             25 => 21,
    //             _ => 0
    //         };
    //     }
    //     
    //     if (ItemLevel >= 128)
    //     {
    //         return starforce switch
    //         {
    //             16 => 7,
    //             17 => 8,
    //             18 => 9,
    //             19 => 10,
    //             20 => 11,
    //             _ => 0
    //         };
    //     }
    //     return 0;
    // }
    //
    // public MapleStatContainer ParseStarforceOption()
    // {
    //     if (CachedStarforce.TryGetValue(sfv, out var opt)) return opt;
    //     MapleStatContainer option = new MapleStatContainer();
    //     if (sfv == 0) return option;
    //
    //     for (int idx = 1; idx <= sfv; idx++)
    //     {
    //         option[MapleStatus.StatusType.ALL_STAT] += GetStatIncreaseByStarforce(idx);
    //         int apmp = EquipType == MapleEquipType.EquipType.WEAPON
    //             ? GetWeaponAttackIncreaseByStarforce(idx, (int) option[MapleStatus.StatusType.ATTACK_AND_MAGIC])
    //             : GetArmorAttackIncreaseByStarforce(idx);
    //         option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += apmp;
    //     }
    //     option.Flush();
    //     return option;
    // }
    // #endregion
}