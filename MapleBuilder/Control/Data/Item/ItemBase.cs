using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Item;

public class ItemBase
{
    public string UniqueName;
    public string DisplayName;
    public MapleEquipType.EquipType EquipType;
    public MapleStatContainer StatContainer;

    public ItemBase(MapleEquipType.EquipType equipType, string name)
    {
        UniqueName = name;
        DisplayName = name;
        EquipType = equipType;
        StatContainer = new MapleStatContainer();
    }
}