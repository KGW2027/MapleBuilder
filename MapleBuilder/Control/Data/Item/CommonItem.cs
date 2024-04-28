using System;
using System.Collections.Generic;
using System.Linq;
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

    private ItemFlag CheckEventRing(MapleCommonItem cItem, ItemFlag flag)
    {
        if (cItem.EquipType != MapleEquipType.EquipType.RING || EquipData == null) return flag;

        int[] noSfv = {
            // SS     벤젼스     결속    코스모스  카오스    오닉스    테네      글로리   어웨이크   딥다크    플레임   어비스
            1113231, 1114300, 1114302, 1114303, 1114306, 1114231, 1114307, 1114316, 1114318, 1114312, 1114324, 1114327
        };
        if ((flag & ItemFlag.STARFORCE) == ItemFlag.STARFORCE && noSfv.Contains(EquipData.Id))
        {
            flag -= ItemFlag.STARFORCE;

            if ((flag & ItemFlag.UPGRADE) == ItemFlag.UPGRADE && EquipData.MaxUpgrade == 1)
                flag -= ItemFlag.UPGRADE;
        }
        
        return flag;
    }

    public CommonItem(EquipmentData data) : base(data)
    {
        // 시드링 스킬 레벨
        SpecialSkillLevel = 0;
        if (EquipType == MapleEquipType.EquipType.RING &&
            (int) DefaultStats[MapleStatus.StatusType.ATTACK_POWER] == 4 &&
            (int) DefaultStats[MapleStatus.StatusType.MAGIC_POWER] == 4)
        {
            SpecialSkillLevel = 1;
        }

        if (SpecialSkillLevel > 0) return;
        
        // 잠재 부여 가능 여부 검사
        MapleEquipType.EquipType[] expectPot =
        {
            MapleEquipType.EquipType.POCKET, MapleEquipType.EquipType.EMBLEM, MapleEquipType.EquipType.MEDAL
        };
        if (!expectPot.Contains(EquipType))
        {
            TopGrade = MaplePotentialGrade.GradeType.NONE;
            BottomGrade = MaplePotentialGrade.GradeType.NONE;
            Potential = new KeyValuePair<MapleStatus.StatusType, int>[6];
        }
        
        // 업횟 가능 여부 검사
        if (data.MaxUpgrade > 1)
        {
            MaxUpgradeCount = RemainUpgradeCount = data.MaxUpgrade;
            Upgrades = new UpgradeOption.UpgradeType[data.MaxUpgrade];
            ChaosAverage = new Dictionary<MapleStatus.StatusType, double>();
        }
        
        // 스타포스 가능 여부 검사
        int[] noSfv = {
            // SS     벤젼스     결속    코스모스  카오스    오닉스    테네      글로리   어웨이크   딥다크    플레임   어비스
            1113231, 1114300, 1114302, 1114303, 1114306, 1114231, 1114307, 1114316, 1114318, 1114312, 1114324, 1114327
        };
        MapleEquipType.EquipType[] expectSfv =
        {
            MapleEquipType.EquipType.POCKET, MapleEquipType.EquipType.EMBLEM, MapleEquipType.EquipType.MEDAL
        };
        if (!expectSfv.Contains(EquipType)
            && (EquipType != MapleEquipType.EquipType.RING || !noSfv.Contains(data.Id))
            && (EquipType != MapleEquipType.EquipType.SUB_WEAPON || data.AfterImage.Equals("swordOL") || data.IconPath!.Contains("Shield"))
           )
        {
            Starforce = 0;
        }
        
        // 추가옵션 가능 여부 검사
        MapleEquipType.EquipType[] expectAddOpt =
        {
            MapleEquipType.EquipType.EMBLEM, MapleEquipType.EquipType.SUB_WEAPON, MapleEquipType.EquipType.BADGE,
            MapleEquipType.EquipType.MEDAL, MapleEquipType.EquipType.HEART, MapleEquipType.EquipType.RING,
        };
        if (!expectAddOpt.Contains(EquipType))
        {
            AddOptions = new Dictionary<AddOptions.AddOptionType, int>();
        }
        
        // 소울 가능 여부 검사
        if (EquipType == MapleEquipType.EquipType.WEAPON)
        {
            SoulName = "없음";
            SoulOption = KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1);
        }
    }
    
    public CommonItem(MapleCommonItem itemBase, ItemFlag flag = 0) : base(itemBase)
    {
        SpecialSkillLevel = itemBase.SpecialRingLevel; 
        if (SpecialSkillLevel > 0) flag = 0;
        flag = CheckEventRing(itemBase, flag);
        
        if ((flag & ItemFlag.POTENTIAL) == ItemFlag.POTENTIAL)
        {
            Potential = new KeyValuePair<MapleStatus.StatusType, int>[6];
            if (itemBase.Potential != null)
            {
                TopGrade = itemBase.Potential.PotentialGrade;
                BottomGrade = itemBase.Potential.AdditionalGrade;
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
    public MaplePotentialGrade.GradeType TopGrade;
    public MaplePotentialGrade.GradeType BottomGrade;
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
    
    /* 시드링 */
    public int SpecialSkillLevel;

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

    public override ItemBase Clone()
    {
        var clone = new CommonItem
        {
            UniqueName = UniqueName,
            DisplayName = DisplayName,
            EquipType = EquipType,
            EquipData = EquipData,
            ItemLevel = ItemLevel,
            DefaultStats = new MapleStatContainer()
        };
        clone.DefaultStats += DefaultStats;

        if (Potential != null)
        {
            clone.TopGrade = TopGrade;
            clone.BottomGrade = BottomGrade;
            clone.Potential = new KeyValuePair<MapleStatus.StatusType, int>[Potential.Length];
            for (int idx = 0; idx < Potential.Length; idx++)
                clone.Potential[idx] = KeyValuePair.Create(Potential[idx].Key, Potential[idx].Value);
        }

        if (Starforce != null) clone.Starforce = Starforce;

        if (AddOptions != null)
        {
            clone.AddOptions = new Dictionary<AddOptions.AddOptionType, int>();
            foreach (var pair in AddOptions) clone.AddOptions.TryAdd(pair.Key, pair.Value);
        }

        if (SoulOption != null)
        {
            clone.SoulName = SoulName;
            clone.SoulOption = KeyValuePair.Create(SoulOption.Value.Key, SoulOption.Value.Value);
        }

        if (Upgrades != null && ChaosAverage != null)
        {
            clone.MaxUpgradeCount = MaxUpgradeCount;
            clone.RemainUpgradeCount = RemainUpgradeCount;
            clone.Upgrades = new UpgradeOption.UpgradeType[Upgrades.Length];
            for (int idx = 0; idx < Upgrades.Length; idx++) clone.Upgrades[idx] = Upgrades[idx];
            clone.ChaosAverage = new Dictionary<MapleStatus.StatusType, double>();
            foreach (var pair in ChaosAverage) clone.ChaosAverage.TryAdd(pair.Key, pair.Value);
        }

        return clone;
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
        newItem.TopGrade = TopGrade;
        newItem.BottomGrade = BottomGrade;
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