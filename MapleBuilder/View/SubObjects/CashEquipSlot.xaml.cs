using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects;

public partial class CashEquipSlot : UserControl
{
    public CashEquipSlot()
    {
        InitializeComponent();

        selectBoxes = new List<ComboBox> {ctCashOptSelect1, ctCashOptSelect2, ctCashOptSelect3};
        labels = new List<Label> {ctCashLabel1, ctCashLabel2, ctCashLabel3};
        sliders = new List<Slider> {ctCashSlider1, ctCashSlider2, ctCashSlider3};

        foreach (MapleCashEquip.LabelType labelType in Enum.GetValues<MapleCashEquip.LabelType>())
        {
            ctCashLabelSelect.Items.Add(MapleCashEquip.GetLabelTypeString(labelType));
        }
        
        foreach (var box in selectBoxes)
        {
            box.Items.Add("STR");
            box.Items.Add("DEX");
            box.Items.Add("INT");
            box.Items.Add("LUK");
            box.Items.Add("없음");
        }
    }

    private List<Label> labels;
    private List<Slider> sliders;
    private List<ComboBox> selectBoxes;
    
    #region XAML Property
    
    public static readonly DependencyProperty EQUIP_TYPE_PROPERTY =
        DependencyProperty.Register("EquipType", typeof(MapleEquipType.EquipType), typeof(CashEquipSlot), new PropertyMetadata(MapleEquipType.EquipType.TOP_BOTTOM,
            (o, args) =>
            {
                var control = (CashEquipSlot) o;
                MapleEquipType.EquipType equipType = (MapleEquipType.EquipType) args.NewValue;
                string equipTypeStr = equipType switch
                {
                    MapleEquipType.EquipType.WEAPON => "무기",
                    MapleEquipType.EquipType.HELMET => "모자",
                    MapleEquipType.EquipType.GLOVE => "장갑",
                    MapleEquipType.EquipType.CAPE => "망토",
                    _ => ""
                };
                control.ctEquipTypelabel.Content = equipTypeStr;
                if (equipType == MapleEquipType.EquipType.WEAPON)
                {
                    control.ctCashSlider1.Width = 200;
                    control.ctCashLabel2.Visibility = Visibility.Collapsed;
                    control.ctCashLabel3.Visibility = Visibility.Collapsed;
                    control.ctCashSlider2.Visibility = Visibility.Collapsed;
                    control.ctCashSlider3.Visibility = Visibility.Collapsed;
                    control.ctCashOptSelect1.Text = "공격력/마력";
                    control.ctCashOptSelect1.Visibility = Visibility.Collapsed;
                    control.ctCashOptSelect2.Visibility = Visibility.Collapsed;
                    control.ctCashOptSelect3.Visibility = Visibility.Collapsed;
                }
            }));
    
    public MapleEquipType.EquipType EquipType
    {
        get => (MapleEquipType.EquipType) GetValue(EQUIP_TYPE_PROPERTY);
        set => SetValue(EQUIP_TYPE_PROPERTY, value);
    }

    #endregion

    #region Events
    
    private void OnOptionSelectChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        string text = e.AddedItems[0]!.ToString()!;
        
        for (int index = 0; index < 3; index++)
        {
            if (selectBoxes[index] != (ComboBox) sender) continue;
            if (text == "없음")
            {
                labels[index].Content = "-";
                break;
            }
            ApplySliderValue(index, text);


            if (BuilderDataContainer.PlayerStatus == null) return;
            int sliderValue = (int) sliders[index].Value;
            if (EquipType != MapleEquipType.EquipType.WEAPON)
            {
                if (Enum.TryParse(selectBoxes[index].Text, out MapleStatus.StatusType befStat))
                    BuilderDataContainer.PlayerStatus.PlayerStat[befStat] -= sliderValue;
                
                if (Enum.TryParse(text, out MapleStatus.StatusType newStat))
                    BuilderDataContainer.PlayerStatus.PlayerStat[newStat] += sliderValue;
                
                BuilderDataContainer.RefreshAll();
            }
            
            break;
        }
    }

    private void OnLabelSelectChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        MapleCashEquip.LabelType labelType = MapleCashEquip.GetLabelType(e.AddedItems[0]!.ToString()!);
        int[] cap = MapleCashEquip.GetLabelOptionCap(labelType, EquipType == MapleEquipType.EquipType.WEAPON);
        
        foreach (var slider in sliders.Where(slider => slider.Visibility != Visibility.Collapsed))
        {
            slider.Minimum = cap[0];
            slider.Maximum = cap[1];
            slider.Value = cap[0];
        }
    }

    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        for (int index = 0; index < 3; index++)
        {
            if (sliders[index] != (Slider) sender) continue;
            ApplySliderValue(index, selectBoxes[index].Text);

            if (BuilderDataContainer.PlayerStatus == null || selectBoxes[index].Text.Equals("없음")) break;
            int delta = (int)e.NewValue - (int)e.OldValue;
            if (EquipType == MapleEquipType.EquipType.WEAPON)
            {
                BuilderDataContainer.PlayerStatus.PlayerStat[MapleStatus.StatusType.ATTACK_POWER] += delta;
                BuilderDataContainer.PlayerStatus.PlayerStat[MapleStatus.StatusType.MAGIC_POWER] += delta;
            }
            else if(Enum.TryParse(selectBoxes[index].Text, out MapleStatus.StatusType statusType))
            {
                BuilderDataContainer.PlayerStatus.PlayerStat[statusType] += delta;
            }
            
            BuilderDataContainer.RefreshAll();
            break;
        }
    }

    private void ApplySliderValue(int idx, string text)
    {
        int sliderValue = (int) sliders[idx].Value;
        if (text == "없음") return;
        labels[idx].Content = $"{text} +{sliderValue}";
    }
    
    #endregion
}