using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleItemPotential
{
    public KeyValuePair<MapleStatus.StatusType, int>[] Potentials { get; private set; }
    public KeyValuePair<MapleStatus.StatusType, int>[] Additionals { get; private set; }
    public MaplePotentialGrade.GradeType TopGrade { get; private set; }
    public MaplePotentialGrade.GradeType BottomGrade { get; private set; }
    private int level;
    private MapleEquipType.EquipType equipType;
    
    public MapleItemPotential(int itemLevel, 
        MapleEquipType.EquipType equipType, 
        MaplePotentialGrade.GradeType potentialGrade = MaplePotentialGrade.GradeType.NONE, 
        MaplePotentialGrade.GradeType additionalGrade = MaplePotentialGrade.GradeType.NONE)
    {
        TopGrade = potentialGrade;
        BottomGrade = potentialGrade;
        level = itemLevel;
        this.equipType = equipType;
        Potentials = new[]
        {
            KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
            KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
            KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1)
        };
        Additionals = new[]
        {
            KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
            KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1), 
            KeyValuePair.Create(MapleStatus.StatusType.OTHER, -1)
        };
    }

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