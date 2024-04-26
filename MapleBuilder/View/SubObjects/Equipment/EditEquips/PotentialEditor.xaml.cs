using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class PotentialEditor : UserControl
{
    private readonly ComboBox[] comboBoxes;
    private const string EmptyString = "옵션 없음 (또는 잡옵)";
    private List<PotentialOptions.Details> topOptions;
    private List<PotentialOptions.Details> subOptions;

    public PotentialEditor()
    {
        InitializeComponent();
        topOptions = new List<PotentialOptions.Details>();
        subOptions = new List<PotentialOptions.Details>();

        GradeBox.Items.Clear();
        foreach (MaplePotentialGrade.GradeType gradeType in Enum.GetValues(typeof(MaplePotentialGrade.GradeType)))
        {
            GradeBox.Items.Add(gradeType == MaplePotentialGrade.GradeType.NONE
                ? "없음"
                : MaplePotentialGrade.GetPotentialGradeString(gradeType));
        }

        GradeBox.SelectedItem = "없음";

        comboBoxes = new ComboBox[3];
        int idx = 0;
        foreach (var box in Grid.GetChildren<ComboBox>())
        {
            if (box == GradeBox) continue;
            comboBoxes[idx++] = box;
            box.SelectionChanged += OnSelectedOptionChanged;
        }
        
        Update();
    }

    private void Update()
    {
        MaplePotentialGrade.GradeType gradeType =
            MaplePotentialGrade.GetPotentialGrade(GradeBox.SelectedItem.ToString());
        topOptions.Clear();
        subOptions.Clear();

        foreach (var box in comboBoxes)
        {
            box.Items.Clear();
            box.Items.Add(EmptyString);
            box.SelectedItem = EmptyString;
        }

        if (TargetItem == null || TargetItem.Potential == null && gradeType == MaplePotentialGrade.GradeType.NONE ) return;
        GradeBox.SelectedItem = MaplePotentialGrade.GetPotentialGradeString(IsAdditional ? TargetItem.BottomGrade : TargetItem.TopGrade);
        
        // 옵션 목록 불러오기 & 콤보 박스에 옵션 입력하기
        foreach (var detail in PotentialOptions.Options)
        {
            if (!detail.AvailableTypes.Contains(TargetItem.EquipType)) continue; // 적용 타입이 아닌 경우 Continue
            var tgtDict = IsAdditional ? detail.AdditionalValues : detail.Values;
            if (tgtDict.TryGetValue(gradeType, out var topValue))
            {
                topOptions.Add(detail);
                subOptions.Add(detail);
                
                int[] opts = ParseOptions(topValue.Values);
                foreach (var opt in opts)
                    AddComboBox(detail.Description.Replace("%d", opt.ToString()));
            }

            var subGrade = gradeType - 1;
            if (tgtDict.TryGetValue(subGrade, out var subValue))
            {
                subOptions.Add(detail);
                
                int[] opts = ParseOptions(subValue.Values);
                foreach (var opt in opts)
                    AddComboBox(detail.Description.Replace("%d", opt.ToString()), true);
            }
        }
        
        for (int idx = 0; idx < 3; idx++)
        {
            int ridx = idx + (IsAdditional ? 3 : 0);
            var option = TargetItem.Potential![ridx];

            foreach (var text in from detail in subOptions where detail.StatusType == option.Key select detail.Description.Replace("%d", option.Value.ToString()))
            {
                ApplyComboBox(idx, text);
                break;
            }
        }
        
    }

    private int[] ParseOptions(Dictionary<int, int[]> values)
    {
        int highest = -1;
        int[]? highestVal = null;
        foreach (var pair in values.Where(pair => pair.Key >= highest && pair.Key <= TargetItem!.ItemLevel))
        {
            highest = pair.Key;
            highestVal = pair.Value;
        }

        return highestVal ?? Array.Empty<int>();
    }

    private void AddComboBox(string text, bool expectMain = false)
    {
        foreach (var box in comboBoxes)
        {
            if (expectMain && box.Tag != null && box.Tag.Equals("0")) continue;
            box.Items.Add(text);
        }
    }

    private void ApplyComboBox(int tag, string text)
    {
        foreach (var box in comboBoxes)
        {
            if (!int.TryParse(box.Tag.ToString(), out var numTag) || numTag != tag) continue;
            box.SelectedItem = text;
        }
    }
    
    #region XAML Property

    public static DependencyProperty IsAdditionalProperty = DependencyProperty.Register(
        nameof(IsAdditional), typeof(bool), typeof(PotentialEditor)
    );

    public bool IsAdditional
    {
        get => (bool) GetValue(IsAdditionalProperty);
        set => SetValue(IsAdditionalProperty, value);
    }
    
    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(CommonItem), typeof(PotentialEditor), new PropertyMetadata(null, OnPotentialUpdated)
        );

    private static void OnPotentialUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PotentialEditor) d;
        control.Update();
    }

    public CommonItem? TargetItem
    {
        get => (CommonItem?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }
    #endregion

    private void OnGradeUpdated(object sender, SelectionChangedEventArgs e)
    {
        if (TargetItem == null || e.AddedItems[0] is not string newGrade) return;
        MaplePotentialGrade.GradeType newType = MaplePotentialGrade.GetPotentialGrade(newGrade);
        if (IsAdditional) TargetItem.BottomGrade = newType;
        else TargetItem.TopGrade = newType;
        Update();
    }
    

    private void OnSelectedOptionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TargetItem == null || e.AddedItems.Count < 1 || e.AddedItems[0] is not string newOption || sender is not ComboBox box || !int.TryParse(box.Tag.ToString(), out int boxTag)) return;

        // Find StatusType
        var findList = boxTag == 0 ? topOptions : subOptions;
        if (findList.Count < 1) return;
        
        MapleStatus.StatusType statusType = MapleStatus.StatusType.OTHER;
        int val = -1;
        foreach (var detail in findList)
        {
            string[] split = detail.Description.Split("%d");
            int num = 0, idx = split[0].Length;
            while (idx < newOption.Length && '0' <= newOption[idx] && newOption[idx] <= '9') num = num * 10 + (newOption[idx++] - '0');
            
            if($"{split[0]}{num}{split[1]}".Equals(newOption))
            {
                statusType = detail.StatusType;
                val = num;
                break;
            }
        }
        
        int ridx = boxTag + (IsAdditional ? 3 : 0);
        TargetItem.Potential![ridx] = KeyValuePair.Create(statusType, val);
    }
}