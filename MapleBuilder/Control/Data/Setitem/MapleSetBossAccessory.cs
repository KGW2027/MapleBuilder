using System;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleBuilder.Control.Data.Setitem;

public class MapleSetBossAccessory : ISetType
{
    public MapleEquipType.EquipType[] GetEquipmentTypesInSet()
    {
        return new[]
        {
            MapleEquipType.EquipType.FACE,
            MapleEquipType.EquipType.EYE,
            MapleEquipType.EquipType.EARRING,
            MapleEquipType.EquipType.RING,
            MapleEquipType.EquipType.PENDANT,
            MapleEquipType.EquipType.BELT,
            MapleEquipType.EquipType.SHOULDER,
            MapleEquipType.EquipType.POCKET,
            MapleEquipType.EquipType.BADGE
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
        switch (Math.Min(setCount, 9))
        {
            case 9:
                option[MapleStatus.StatusType.BOSS_DAMAGE] += 10;
                option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                option[MapleStatus.StatusType.ALL_STAT] += 15;
                goto case 7;
            case 8:
            case 7:
                option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
                option[MapleStatus.StatusType.ALL_STAT] += 10;
                option[MapleStatus.StatusType.IGNORE_DEF] += 10;
                goto case 5;
            case 6:
            case 5:
                option[MapleStatus.StatusType.ALL_STAT] += 10;
                option[MapleStatus.StatusType.HP_AND_MP_RATE] += 5;
                option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 5;
                goto case 3;
            case 4:
            case 3:
                option[MapleStatus.StatusType.ALL_STAT] += 10;
                option[MapleStatus.StatusType.HP_AND_MP_RATE] += 5;
                option[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 5;
                break;
        }
        return option;
    }

    public string GetSignature()
    {
        return "보장";
    }
}