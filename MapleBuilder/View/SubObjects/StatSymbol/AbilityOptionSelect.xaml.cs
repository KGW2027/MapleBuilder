using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects.StatSymbol;

public partial class AbilityOptionSelect : UserControl
{
    private static readonly Dictionary<string, MapleStatus.StatusType> CACHED_OPTION = new();
    
    private MaplePotentialGrade.GradeType optionGrade;
    private bool IsUp => IsUpValue.IsChecked!.Value;
    private string ComboBoxStr => MapleAbility.GetAbilityString(selectedOption).Replace("%d", ((int) ValueSlider.Value).ToString());
    private MapleStatus.StatusType selectedOption;
    public MapleStatus.StatusType Option
    {
        get => selectedOption;
        set
        {
            selectedOption = value;
            ChangedOptionAction();
        }
    }

    public double Value
    {
        set => ValueSlider.Value = Math.Clamp(value, ValueSlider.Minimum, ValueSlider.Maximum);
    }
    
    public AbilityOptionSelect()
    {
        InitializeComponent();
        optionGrade = MaplePotentialGrade.GradeType.RARE;
        selectedOption = MapleStatus.StatusType.OTHER;
        GradeChangedAction();
    }

    public void OnGradeChanged(MaplePotentialGrade.GradeType newGrade)
    {
        optionGrade = newGrade;
        if (!IsTop)
            optionGrade = (MaplePotentialGrade.GradeType) Math.Max((int) optionGrade - 2, 1);
        GradeChangedAction();
    }

    #region XAML Property

    public static readonly DependencyProperty IsTopProperty =
        DependencyProperty.Register("IsTop", typeof(bool), typeof(AbilityOptionSelect), new PropertyMetadata(false,
            (o, args) =>
            {
                var control = (AbilityOptionSelect) o;
                bool isTop = (bool) args.NewValue;
                control.IsUpValue.Visibility = isTop ? Visibility.Collapsed : Visibility.Visible;
            }));

    public bool IsTop
    {
        get => (bool) GetValue(IsTopProperty);
        set => SetValue(IsTopProperty, value);
    }
    #endregion

    private void OnAbilitySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || !IsInitialized) return;
        
        // 선택 해제
        if (selectedOption != MapleStatus.StatusType.OTHER && GlobalDataController.Instance.PlayerInstance != null)
            GlobalDataController.Instance.PlayerInstance.Ability[selectedOption] = 0;
        
        string newSelect = e.AddedItems[0]!.ToString()!;
        selectedOption = CACHED_OPTION.GetValueOrDefault(newSelect, MapleStatus.StatusType.OTHER);
        ChangedOptionAction();
    }

    private void OnSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        DisplayAbility.Text = ComboBoxStr;

        if (selectedOption != MapleStatus.StatusType.OTHER && GlobalDataController.Instance.PlayerInstance != null)
            GlobalDataController.Instance.PlayerInstance.Ability[selectedOption] = (int) e.NewValue;
    }

    private void ChangedOptionAction()
    {
        if (selectedOption == MapleStatus.StatusType.OTHER)
        {
            ValueSlider.Minimum = 1;
            ValueSlider.Maximum = 1;
            ValueSlider.SmallChange = 1;
            ValueSlider.Value = 1;
            return;
        }
        
        MaplePotentialGrade.GradeType targetGrade = optionGrade;
        if (IsUp) targetGrade += 1;

        int[] options = MapleAbility.GetMinMax(selectedOption, targetGrade);
        
        if (options[0] == 0 && options[1] == 0)
        {
            selectedOption = MapleStatus.StatusType.OTHER;
            GradeChangedAction();
            return;
        }
        
        ValueSlider.Minimum = options[0];
        ValueSlider.Maximum = options[1];
        ValueSlider.SmallChange = options.Length == 3 ? options[2] : 1;
        ValueSlider.Value = options[0];
    }

    private void GradeChangedAction()
    {
        MaplePotentialGrade.GradeType targetGrade = optionGrade;
        if (IsUp) targetGrade += 1;
        
        DisplayAbility.Items.Clear();
        foreach (MapleStatus.StatusType status in Enum.GetValues<MapleStatus.StatusType>())
        {
            string display = MapleAbility.GetAbilityString(status);
            if (display.Equals("잡옵")) continue;
            int[] options = MapleAbility.GetMinMax(status, targetGrade);
            if (options[0] == 0 && options[1] == 0) continue;
            
            display = display.Replace("%d", "[ ]");
            CACHED_OPTION.TryAdd(display, status);
            DisplayAbility.Items.Add(display);
        }
        DisplayAbility.Items.Add("잡옵");
        
        ChangedOptionAction();
    }

    private void OnCheckboxChecked(object sender, RoutedEventArgs e)
    {
        GradeChangedAction();
    }

    private void OnCheckboxUnchecked(object sender, RoutedEventArgs e)
    {
        GradeChangedAction();
    }

    private void OnDropdownClosed(object? sender, EventArgs e)
    {
        DisplayAbility.Text = ComboBoxStr;
    }
}