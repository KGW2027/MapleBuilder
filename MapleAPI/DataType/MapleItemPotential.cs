using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleItemPotential
{
    public KeyValuePair<MaplePotentialOption.OptionType, int>[] Potentials { get; private set; }
    public KeyValuePair<MaplePotentialOption.OptionType, int>[] Additionals { get; private set; }
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
            KeyValuePair.Create(MaplePotentialOption.OptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOption.OptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOption.OptionType.OTHER, -1)
        };
        Additionals = new[]
        {
            KeyValuePair.Create(MaplePotentialOption.OptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOption.OptionType.OTHER, -1), 
            KeyValuePair.Create(MaplePotentialOption.OptionType.OTHER, -1)
        };
    }

    public void SetPotential(int slot, KeyValuePair<MaplePotentialOption.OptionType, int> value)
    {
        if (slot is < 0 or >= 3) return;
        Potentials[slot] = value;
    }
    
    public void SetAdditional(int slot, KeyValuePair<MaplePotentialOption.OptionType, int> value)
    {
        if (slot is < 0 or >= 3) return;
        Additionals[slot] = value;
    }
}