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
    
    public override MapleStatContainer GetItemStatus()
    {
        throw new System.NotImplementedException();
    }
}