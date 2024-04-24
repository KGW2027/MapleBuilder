using MapleAPI.DataType;
using MapleAPI.DataType.Item;

namespace MapleBuilder.Control.Data.Item;

public class TitleItem : ItemBase
{
    public TitleItem(MapleTitleItem itemBase) : base(itemBase)
    {
        titleStat = itemBase.Status;
    }

    private readonly MapleStatContainer titleStat;

    public override MapleStatContainer GetItemStatus()
    {
        return titleStat;
    }
}