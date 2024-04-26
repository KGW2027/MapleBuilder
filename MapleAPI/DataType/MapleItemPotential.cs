using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleItemPotential
{
    public MaplePotentialGrade.GradeType PotentialGrade = MaplePotentialGrade.GradeType.NONE;
    public MaplePotentialGrade.GradeType AdditionalGrade = MaplePotentialGrade.GradeType.NONE;
    
    public KeyValuePair<MapleStatus.StatusType, int>[] Potentials { get; private set; } = {
        KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
        KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
        KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1)
    };

    public KeyValuePair<MapleStatus.StatusType, int>[] Additionals { get; private set; } = {
        KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
        KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
        KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1)
    };

    public void SetPotential(int slot, KeyValuePair<MapleStatus.StatusType, int> value)
    {
        if (slot is < 0 or >= 3) return;
        Potentials[slot] = value;
    }
    
    public void SetAdditional(int slot, KeyValuePair<MapleStatus.StatusType, int> value)
    {
        if (slot is < 0 or >= 3) return;
        Additionals[slot] = value;
    }
}