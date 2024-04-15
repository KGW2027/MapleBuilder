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

    public MaplePetType.PetType PetType;
    public readonly MapleStatContainer InitStats;
    
    protected override MapleStatContainer GetItemStatus()
    {
        return InitStats;
    }
}