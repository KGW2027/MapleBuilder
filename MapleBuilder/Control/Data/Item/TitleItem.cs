using MapleAPI.DataType;
using MapleAPI.DataType.Item;

namespace MapleBuilder.Control.Data.Item;

public class TitleItem : ItemBase
{
    public TitleItem(MapleTitleItem itemBase) : base(itemBase)
    {
        titleStat = itemBase.Status;
    }

    private TitleItem()
    {
        titleStat = new MapleStatContainer();
    }

    private MapleStatContainer titleStat;

    public override MapleStatContainer GetItemStatus()
    {
        return titleStat;
    }

    public override MapleStatContainer GetUpStatus()
    {
        return titleStat;
    }

    public override ItemBase Clone()
    {
        var clone = new TitleItem();
        
        clone.UniqueName = UniqueName;
        clone.DisplayName = DisplayName;
        clone.EquipType = EquipType;
        clone.EquipData = EquipData;
        clone.ItemLevel = ItemLevel;
        clone.DefaultStats = new MapleStatContainer();
        clone.DefaultStats += DefaultStats;
        
        clone.titleStat += titleStat;
        return clone;
    }
}