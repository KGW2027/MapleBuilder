using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item;

public class PetItem : ItemBase
{
    public PetItem(MaplePetItem itemBase) : base(itemBase)
    {
        PetType = itemBase.PetType;
        InitStats = itemBase.Status;
    }

    private PetItem()
    {
        PetType = MaplePetType.PetType.OTHER;
        InitStats = new MapleStatContainer();
    }

    public MaplePetType.PetType PetType;
    public MapleStatContainer InitStats;
    
    public override MapleStatContainer GetItemStatus()
    {
        return InitStats;
    }

    public override MapleStatContainer GetUpStatus()
    {
        return InitStats;
    }

    public override ItemBase Clone()
    {
        var clone = new PetItem {PetType = this.PetType};
        clone.InitStats += InitStats;

        clone.UniqueName = UniqueName;
        clone.DisplayName = DisplayName;
        clone.EquipType = EquipType;
        clone.EquipData = EquipData;
        clone.ItemLevel = ItemLevel;
        clone.DefaultStats = new MapleStatContainer();
        clone.DefaultStats += DefaultStats;
        
        return clone;
    }
}