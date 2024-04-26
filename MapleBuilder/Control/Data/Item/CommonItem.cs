using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.Control.Data.Item;

public class CommonItem : ItemBase
{
    private CommonItem()
    {
        Potential = new KeyValuePair<MapleStatus.StatusType, int>[6];
        
    }
    public CommonItem(MapleCommonItem itemBase, ItemFlag flag = 0) : base(itemBase)
    {
        if ((flag & ItemFlag.POTENTIAL) == ItemFlag.POTENTIAL)
        {
            Potential = new KeyValuePair<MapleStatus.StatusType, int>[6];
            if (itemBase.Potential != null)
            {
                for (int idx = 0; idx < itemBase.Potential!.Potentials.Length; idx++)
                    Potential[idx] = itemBase.Potential!.Potentials[idx];
                for (int idx = 0; idx < itemBase.Potential!.Additionals.Length; idx++)
                    Potential[idx+3] = itemBase.Potential!.Additionals[idx];
            }
        }

        if ((flag & ItemFlag.UPGRADE) == ItemFlag.UPGRADE)
        {
            MaxUpgradeCount = EquipData!.MaxUpgrade;
            RemainUpgradeCount = MaxUpgradeCount - itemBase.UpgradeCount;
            Upgrades = this.ParseUpgrades(itemBase.UpgradeOption, out ChaosAverage);
        }

        if ((flag & ItemFlag.STARFORCE) == ItemFlag.STARFORCE)
        {
            Starforce = itemBase.StarForce;
        }

        if ((flag & ItemFlag.ADD_OPTION) == ItemFlag.ADD_OPTION)
        {
            AddOptions = this.ParseAddOption(itemBase.AddOption);
        }

        if ((flag & ItemFlag.SOUL_ENCHANT) == ItemFlag.SOUL_ENCHANT)
        {
            SoulName = itemBase.SoulName;
            SoulOption = itemBase.SoulOption;
        }
    }

    /* 잠재능력 */
    public KeyValuePair<MapleStatus.StatusType, int>[]? Potential;
    
    /* 스타포스 */
    public int? Starforce;
    
    /* 추가옵션 */
    public Dictionary<AddOptions.AddOptionType, int>? AddOptions;
    
    /* 소울 인챈트 */
    public string? SoulName;
    public KeyValuePair<MapleStatus.StatusType, int>? SoulOption;
    
    /* 업그레이드 */
    public int? RemainUpgradeCount;
    public int? MaxUpgradeCount;
    public UpgradeOption.UpgradeType[]? Upgrades;
    public Dictionary<MapleStatus.StatusType, double>? ChaosAverage;

    private MapleStatContainer GetAddStatus()
    {
        MapleStatContainer msc = new MapleStatContainer();
        if (AddOptions == null) return msc;

        foreach (var pair in AddOptions)
        {
            int val = pair.Key.GetOptionStatus(this, pair.Value);
            switch (pair.Key)
            {
                case Option.AddOptions.AddOptionType.STR:
                    msc[MapleStatus.StatusType.STR] += val;
                    break;
                case Option.AddOptions.AddOptionType.DEX:
                    msc[MapleStatus.StatusType.DEX] += val;
                    break;
                case Option.AddOptions.AddOptionType.INT:
                    msc[MapleStatus.StatusType.INT] += val;
                    break;
                case Option.AddOptions.AddOptionType.LUK:
                    msc[MapleStatus.StatusType.LUK] += val;
                    break;
                case Option.AddOptions.AddOptionType.STR_DEX:
                    msc[MapleStatus.StatusType.DEX] += val;
                    msc[MapleStatus.StatusType.STR] += val;
                    break;
                case Option.AddOptions.AddOptionType.STR_INT:
                    msc[MapleStatus.StatusType.STR] += val;
                    msc[MapleStatus.StatusType.INT] += val;
                    break;
                case Option.AddOptions.AddOptionType.STR_LUK:
                    msc[MapleStatus.StatusType.STR] += val;
                    msc[MapleStatus.StatusType.LUK] += val;
                    break;
                case Option.AddOptions.AddOptionType.DEX_INT:
                    msc[MapleStatus.StatusType.DEX] += val;
                    msc[MapleStatus.StatusType.INT] += val;
                    break;
                case Option.AddOptions.AddOptionType.DEX_LUK:
                    msc[MapleStatus.StatusType.DEX] += val;
                    msc[MapleStatus.StatusType.LUK] += val;
                    break;
                case Option.AddOptions.AddOptionType.INT_LUK:
                    msc[MapleStatus.StatusType.INT] += val;
                    msc[MapleStatus.StatusType.LUK] += val;
                    break;
                case Option.AddOptions.AddOptionType.HP:
                    msc[MapleStatus.StatusType.HP] += val;
                    break;
                case Option.AddOptions.AddOptionType.MP:
                    msc[MapleStatus.StatusType.MP] += val;
                    break;
                case Option.AddOptions.AddOptionType.ATTACK:
                    msc[MapleStatus.StatusType.ATTACK_POWER] += val;
                    break;
                case Option.AddOptions.AddOptionType.MAGIC:
                    msc[MapleStatus.StatusType.MAGIC_POWER] += val;
                    break;
                case Option.AddOptions.AddOptionType.BOSS_DAMAGE:
                    msc[MapleStatus.StatusType.BOSS_DAMAGE] += val;
                    break;
                case Option.AddOptions.AddOptionType.DAMAGE:
                    msc[MapleStatus.StatusType.DAMAGE] += val;
                    break;
                case Option.AddOptions.AddOptionType.ALL_STAT:
                    msc[MapleStatus.StatusType.ALL_STAT_RATE] += val;
                    break;
            }
        }

        return msc;
    }

    private MapleStatContainer GetPotentialStatus()
    {
        MapleStatContainer msc = new MapleStatContainer();
        if (Potential == null) return msc;

        foreach (var pair in Potential)
            msc[pair.Key] += pair.Value;

        return msc;
    }
    
    public override MapleStatContainer GetItemStatus()
    {
        MapleStatContainer msc = new MapleStatContainer();
        msc += EquipData!.GetStatus(); // 기본 정보
        msc += GetAddStatus();         // 추옵 정보
        msc += GetPotentialStatus();   // 잠재 정보

        MapleStatContainer upgStatus = Upgrades.ConvertUpgrades(this, ChaosAverage);
        msc += upgStatus; // 작 정보
        // 스타포스 정보
        MapleStatContainer sfStatus = this.ParseStarforceOption(upgStatus[MapleStatus.StatusType.ATTACK_POWER], upgStatus[MapleStatus.StatusType.MAGIC_POWER]);
        msc += sfStatus;

        if (SoulOption != null)
            msc[SoulOption.Value.Key] += SoulOption.Value.Value;
        
        return msc;
    }

    public override MapleStatContainer GetUpStatus()
    {
        MapleStatContainer msc = new MapleStatContainer();
        msc += EquipData!.GetStatus(); // 기본 정보
        msc += GetAddStatus();         // 추옵 정보

        MapleStatContainer upgStatus = Upgrades.ConvertUpgrades(this, ChaosAverage);
        msc += upgStatus; // 작 정보
        
        // 스타포스 정보
        MapleStatContainer sfStatus = this.ParseStarforceOption(upgStatus[MapleStatus.StatusType.ATTACK_POWER], upgStatus[MapleStatus.StatusType.MAGIC_POWER]);
        msc += sfStatus;
        
        return msc;
    }

    public CommonItem CopyItem(EquipmentData overrideData, bool isPowerCalc = true)
    {
        var newItem = new CommonItem();
        newItem.UniqueName = overrideData.Name;
        newItem.DisplayName = overrideData.Name;
        newItem.EquipType = EquipType;
        newItem.EquipData = overrideData;
        newItem.ItemLevel = overrideData.Level;
        newItem.DefaultStats = overrideData.GetStatus();

        // Potential Copy
        newItem.Potential = Potential;
        // Starforce Copy
        newItem.Starforce = Starforce;
        // Upgrade Copy
        newItem.MaxUpgradeCount = MaxUpgradeCount;
        newItem.RemainUpgradeCount = RemainUpgradeCount;

        if (Upgrades != null && isPowerCalc)
        {
            newItem.Upgrades = new UpgradeOption.UpgradeType[Upgrades.Length];
            int idx = 0;
            foreach (var upg in Upgrades)
            {
                if (upg.ToString().Contains("MAG"))
                {
                    string newName = upg.ToString().Replace("MAG", "ATK");
                    if (Enum.TryParse(typeof(UpgradeOption.UpgradeType), newName, out var newType) && newType != null)
                    {
                        newItem.Upgrades[idx++] = (UpgradeOption.UpgradeType) newType;
                        continue;
                    }
                }

                if (upg.ToString().StartsWith("SPELL_TRACE"))
                {
                    string last = upg.ToString().Split("_")[3];
                    if (Enum.TryParse(typeof(UpgradeOption.UpgradeType), $"SPELL_TRACE_DEX_{last}", out var newType) &&
                        newType != null)
                    {
                        newItem.Upgrades[idx++] = (UpgradeOption.UpgradeType) newType;
                        continue;
                    }
                }

                newItem.Upgrades[idx++] = upg;
            }
            
            newItem.ChaosAverage = ChaosAverage;
            if (newItem.ChaosAverage != null)
            {
                (newItem.ChaosAverage[MapleStatus.StatusType.ATTACK_POWER],
                        newItem.ChaosAverage[MapleStatus.StatusType.MAGIC_POWER])
                    = (newItem.ChaosAverage[MapleStatus.StatusType.MAGIC_POWER],
                        newItem.ChaosAverage[MapleStatus.StatusType.ATTACK_POWER]);
            }
        }
        else
        {
            newItem.Upgrades = Upgrades;
            newItem.ChaosAverage = ChaosAverage;
        }

        // Soul Copy
        newItem.SoulName = SoulName;
        newItem.SoulOption = SoulOption;
        
        // Add Option Copy
        if (isPowerCalc && AddOptions != null)
        {
            newItem.AddOptions = new Dictionary<AddOptions.AddOptionType, int>();
            foreach (var origAddOpt in AddOptions)
            {
                int grade = origAddOpt.Value;
                if (origAddOpt.Key == Option.AddOptions.AddOptionType.MAGIC)
                {
                    grade = Math.Max(grade, newItem.AddOptions.GetValueOrDefault(Option.AddOptions.AddOptionType.ATTACK, 0));
                    newItem.AddOptions.TryAdd(Option.AddOptions.AddOptionType.ATTACK, 0);
                    newItem.AddOptions[Option.AddOptions.AddOptionType.ATTACK] = Math.Max(grade, origAddOpt.Value);
                    continue;
                }

                if (newItem.AddOptions.ContainsKey(origAddOpt.Key))
                    newItem.AddOptions[origAddOpt.Key] = Math.Max(grade, newItem.AddOptions[origAddOpt.Key]);
                else newItem.AddOptions.Add(origAddOpt.Key, origAddOpt.Value);

            }
        } else newItem.AddOptions = AddOptions;
        
        return newItem;
    }
}