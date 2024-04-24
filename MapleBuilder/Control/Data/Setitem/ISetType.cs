using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Setitem;

public interface ISetType
{
    public MapleEquipType.EquipType[] GetEquipmentTypesInSet();
    public MapleStatContainer GetSetEffect(MapleEquipType.EquipType[] sets, MapleEquipType.EquipType? luckyItemType, out int setCount);
    public string GetSignature();
}