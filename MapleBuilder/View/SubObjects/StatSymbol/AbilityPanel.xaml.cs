using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.StatSymbol;

public partial class AbilityPanel : UserControl
{
    private MaplePotentialGrade.GradeType abilityGrade;
    private readonly List<AbilityOptionSelect> optionSelects;
    private bool isInitialized;
    
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
        

        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {
        if (isInitialized) return;
        isInitialized = true;
        
        Dictionary<MaplePotentialGrade.GradeType, List<KeyValuePair<MapleStatus.StatusType, double>>> parseGrade = new();
        MaplePotentialGrade.GradeType topGrade = MaplePotentialGrade.GradeType.EPIC;
        foreach (var pair in pdata.Ability)
        {
            bool found = false;
            foreach (MaplePotentialGrade.GradeType gradeType in Enum.GetValues<MaplePotentialGrade.GradeType>())
            {
                if (gradeType == MaplePotentialGrade.GradeType.NONE) continue;

                int[] options = MapleAbility.GetMinMax(pair.Key, gradeType);
                if (options[0] <= pair.Value && pair.Value <= options[1])
                {
                    parseGrade.TryAdd(gradeType, new List<KeyValuePair<MapleStatus.StatusType, double>>());
                    parseGrade[gradeType].Add(pair);
                    topGrade = (MaplePotentialGrade.GradeType) Math.Max((int) topGrade, (int) gradeType);
                    found = true;
                    break;
                }
            }

            if (found) continue;
            parseGrade.TryAdd(MaplePotentialGrade.GradeType.RARE,
                new List<KeyValuePair<MapleStatus.StatusType, double>>());
            parseGrade[MaplePotentialGrade.GradeType.RARE].Add(pair);
        }

        Dispatcher.BeginInvoke(() =>
        {
            optionSelects[0].OnGradeChanged(topGrade);
            optionSelects[1].OnGradeChanged(topGrade);
            optionSelects[2].OnGradeChanged(topGrade);

            int idx = 0;
            for (int grade = (int) topGrade; grade >= (int) MaplePotentialGrade.GradeType.RARE; grade--)
            {
                MaplePotentialGrade.GradeType gradeEnum = (MaplePotentialGrade.GradeType) grade;
                if (parseGrade!.GetValueOrDefault(gradeEnum, null) == null) continue;

                bool isOverGrade = topGrade >= MaplePotentialGrade.GradeType.UNIQUE && (topGrade - gradeEnum) == 1;
                foreach (var pair in parseGrade[gradeEnum])
                {
                    var targetSelector = optionSelects[idx++];
                    if (isOverGrade)
                        targetSelector.IsUpValue.IsChecked = true;

                    targetSelector.Option = pair.Key;
                    targetSelector.Value = pair.Value;
                }
            }
        });
    }

    private void OnAbilityGradeChanged(object sender, SelectionChangedEventArgs e)
    {
        abilityGrade = MaplePotentialGrade.GetPotentialGrade(AbilityGradeSelector.Text);
        foreach (AbilityOptionSelect element in optionSelects)
            element.OnGradeChanged(abilityGrade);
        
    }
}