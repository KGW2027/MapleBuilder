using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleBuilder.Control.Data.Item.Interface;

namespace MapleBuilder.Control.Data.Item;

public class ArmorItem : ItemBase, IPotentialSupport, IStarforceSupport, IUpgradeSupport, IAddOptionSupport
{
    
    protected internal ArmorItem(MapleCommonItem itemBase) : base(itemBase)
    {
        MaxUpgradeCount = EquipData!.MaxUpgrade;
        RemainUpgradeCount = MaxUpgradeCount - itemBase.UpgradeCount;
        Starforce = itemBase.StarForce;
    }

    public int RemainUpgradeCount;
    public int MaxUpgradeCount;
    public int Starforce;
    
    
    public MapleStatContainer GetPotentialStatus()
    {
        throw new System.NotImplementedException();
    }

    public MapleStatContainer GetStarforceStatus()
    {
        throw new System.NotImplementedException();
    }

    public MapleStatContainer GetUpgradeStatus()
    {
        throw new System.NotImplementedException();
    }

    public MapleStatContainer GetAddOptionStatus()
    {
        throw new System.NotImplementedException();
    }

    public override MapleStatContainer GetItemStatus()
    {
        throw new System.NotImplementedException();
    }
}