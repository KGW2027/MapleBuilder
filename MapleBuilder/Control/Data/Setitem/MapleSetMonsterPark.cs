using System;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Setitem;

public class MapleSetMonsterPark : ISetType
{
    public MapleEquipType.EquipType[] GetEquipmentTypesInSet()
    {
        return new[]
        {
            MapleEquipType.EquipType.BADGE,
            MapleEquipType.EquipType.MEDAL
        };
    }

    public MapleStatContainer GetSetEffect(MapleEquipType.EquipType[] sets, MapleEquipType.EquipType? luckyItemType, out int setCount)
    {
        MapleStatContainer option = new MapleStatContainer();
        setCount = sets.Length;
        if (luckyItemType != null // 럭키 아이템을 착용 중
            && !sets.Contains(luckyItemType.Value) // 럭키 아이템이 현재 세트 효과에 포함되지 않아야 함. (제네-에테 등)
            && sets.Length >= 3 // 현재 세트 아이템을 3 부위 이상 착용 중
            && GetEquipmentTypesInSet().Contains(luckyItemType.Value)) // 럭키 아이템 부위가 세트 아이템에 포함되는 부위인 경우
            setCount += 1;
        switch (Math.Min(setCount, 2))
        {
            case 2:
                option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                break;
        }
        return option;
    }

    public string GetSignature()
    {
        return "칠요";
    }
}