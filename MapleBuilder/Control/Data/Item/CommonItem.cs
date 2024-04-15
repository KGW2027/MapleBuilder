using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.Control.Data.Item;

public class CommonItem : ItemBase
{
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
    public readonly KeyValuePair<MapleStatus.StatusType, int>[]? Potential;
    
    /* 스타포스 */
    public int? Starforce;
    
    /* 추가옵션 */
    public readonly Dictionary<AddOptions.AddOptionType, int>? AddOptions;
    
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
    
    protected override MapleStatContainer GetItemStatus()
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

        // Console.WriteLine($" ===== [ {UniqueName}'s AddStatus INFO ] =====");
        // foreach (var pair in GetAddStatus())
        //     Console.WriteLine($"\t{pair.Key} : {pair.Value}");
        // if (UniqueName.Equals("아케인셰이드 메이지글러브"))
        // {
        //     
        //     
        //     Console.WriteLine($" ===== [ {UniqueName}'s UPGRADE INFO ] =====");
        //     foreach (var pair in upgStatus)
        //         Console.WriteLine($"\t{pair.Key} : {pair.Value}");
        //     
        //     Console.WriteLine($" ===== [ {UniqueName}'s STARFORCE INFO ] =====");
        //     foreach (var pair in sfStatus)
        //         Console.WriteLine($"\t{pair.Key} : {pair.Value}");
        // }

        if (SoulOption != null)
            msc[SoulOption.Value.Key] += SoulOption.Value.Value;

        string s = $"[Equipment {UniqueName}] Status = {{";
        foreach (var pair in msc) s += $"{pair.Key}={pair.Value}, ";
        Console.WriteLine($"{s[..^2]}}}");
        
        return msc;
    }
}