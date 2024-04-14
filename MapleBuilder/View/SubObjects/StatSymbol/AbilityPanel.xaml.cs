using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MapleAPI.Enum;

namespace MapleBuilder.View.SubObjects.StatSymbol;

public partial class AbilityPanel : UserControl
{
    private MaplePotentialGrade.GradeType abilityGrade;
    private readonly List<AbilityOptionSelect> optionSelects;
    
    public AbilityPanel()
    {
        InitializeComponent();

        abilityGrade = MaplePotentialGrade.GradeType.LEGENDARY;
        
        optionSelects = new List<AbilityOptionSelect>();
        foreach (var element in MainGrid.GetChildren<AbilityOptionSelect>())
        {
            optionSelects.Add(element);
            element.OnGradeChanged(abilityGrade);
        }

        AbilityGradeSelector.Items.Clear();
        AbilityGradeSelector.Items.Add(MaplePotentialGrade.GetPotentialGradeString(MaplePotentialGrade.GradeType.EPIC));
        AbilityGradeSelector.Items.Add(MaplePotentialGrade.GetPotentialGradeString(MaplePotentialGrade.GradeType.UNIQUE));
        AbilityGradeSelector.Items.Add(MaplePotentialGrade.GetPotentialGradeString(MaplePotentialGrade.GradeType.LEGENDARY));
    }

    private void OnAbilityGradeChanged(object sender, SelectionChangedEventArgs e)
    {
        abilityGrade = MaplePotentialGrade.GetPotentialGrade(AbilityGradeSelector.Text);
        foreach (AbilityOptionSelect element in optionSelects)
            element.OnGradeChanged(abilityGrade);
        
    }
}