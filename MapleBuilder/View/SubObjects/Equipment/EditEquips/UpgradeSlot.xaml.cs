using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control.Data;
using MapleBuilder.Control.Data.Item.Option;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class UpgradeSlot : UserControl
{
    public UpgradeSlot()
    {
        InitializeComponent();
    }
    
    #region XAML Property

    public static readonly DependencyProperty ChaosAverageProperty = DependencyProperty.Register(
        nameof(ChaosAverage), typeof(Dictionary<MapleStatus.StatusType, double>), typeof(UpgradeSlot));

    public Dictionary<MapleStatus.StatusType, double>? ChaosAverage
    {
        get => (Dictionary<MapleStatus.StatusType, double>?) GetValue(ChaosAverageProperty);
        set => SetValue(ChaosAverageProperty, value);
    } 
    
    
    public static readonly DependencyProperty UpgradeTypeProperty = DependencyProperty.Register(
        nameof(UpgradeType), typeof(UpgradeOption.UpgradeType), typeof(UpgradeSlot), new PropertyMetadata(UpgradeOption.UpgradeType.NONE, OnUpgradeTypeChanged)
    );

    private static void OnUpgradeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (UpgradeSlot) d;
        control.Update();
    }

    public UpgradeOption.UpgradeType UpgradeType
    {
        get => (UpgradeOption.UpgradeType) GetValue(UpgradeTypeProperty);
        set => SetValue(UpgradeTypeProperty, value);
    }

    #endregion

    private void Update()
    {
        if (UpgradeType == UpgradeOption.UpgradeType.NONE)
            return;

        string itemText = "";
        switch (UpgradeType)
        {
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_15:
                itemText = "주문의 흔적";
                break;
            case UpgradeOption.UpgradeType.DOMINATOR_FRAGMENT:
                itemText = "비틀린 시간의 파편";
                break;
            case UpgradeOption.UpgradeType.CHAOS:
                itemText = "혼돈의 주문서 60%";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_5:
                itemText = "프리미엄 악세서리 공격력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_4:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_5:
                itemText = "프리미엄 악세서리 마력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_2:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_3:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_4:
                itemText = "악세서리 공격력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_2:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_3:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_4:
                itemText = "악세서리 마력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.MAGICAL_ATK_9:
            case UpgradeOption.UpgradeType.MAGICAL_ATK_10:
            case UpgradeOption.UpgradeType.MAGICAL_ATK_11:
                itemText = "매지컬 한손무기 공격력 주문서";
                break;
            case UpgradeOption.UpgradeType.MAGICAL_MAG_9:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_10:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_11:
                itemText = "매지컬 한손무기 마력 주문서";
                break;
            case UpgradeOption.UpgradeType.MIRACLE_50_ATK_2:
            case UpgradeOption.UpgradeType.MIRACLE_50_ATK_3:
            case UpgradeOption.UpgradeType.MIRACLE_50_MAG_2:
            case UpgradeOption.UpgradeType.MIRACLE_50_MAG_3:
            case UpgradeOption.UpgradeType.EARRING_INT_10:
                itemText = "투구 방어력 주문서 10%";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_PET_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_PET_ATK_5:
                itemText = "프리미엄 펫장비 공격력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_PET_MAG_4:
            case UpgradeOption.UpgradeType.PREMIUM_PET_MAG_5:
                itemText = "프리미엄 펫장비 마력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.PET_ATK_2:
            case UpgradeOption.UpgradeType.PET_ATK_3:
            case UpgradeOption.UpgradeType.PET_ATK_4:
                itemText = "펫장비 공격력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.PET_MAG_2:
            case UpgradeOption.UpgradeType.PET_MAG_3:
            case UpgradeOption.UpgradeType.PET_MAG_4:
                itemText = "펫장비 마력 스크롤 100%";
                break;
            case UpgradeOption.UpgradeType.ARMOR_70_ATK_1:
            case UpgradeOption.UpgradeType.ARMOR_70_ATK_2:
            case UpgradeOption.UpgradeType.ARMOR_70_MAG_1:
            case UpgradeOption.UpgradeType.ARMOR_70_MAG_2:
            case UpgradeOption.UpgradeType.PET_70_ATK_1:
            case UpgradeOption.UpgradeType.PET_70_ATK_2:
            case UpgradeOption.UpgradeType.PET_70_MAG_1:
            case UpgradeOption.UpgradeType.PET_70_MAG_2:
            case UpgradeOption.UpgradeType.ACCESSORY_70_ATK_1:
            case UpgradeOption.UpgradeType.ACCESSORY_70_ATK_2:
            case UpgradeOption.UpgradeType.ACCESSORY_70_MAG_1:
            case UpgradeOption.UpgradeType.ACCESSORY_70_MAG_2:
                itemText = "투구 방어력 주문서 60%";
                break;
            case UpgradeOption.UpgradeType.HEART_ATK:
                itemText = "카르마 스페셜 하트 공격력 주문서 100%";
                break;
            case UpgradeOption.UpgradeType.HEART_MAG:
                itemText = "카르마 스페셜 하트 마력 주문서 100%";
                break;
        }

        if (itemText.Equals("")) return;
        if (!WzDatabase.Instance.EquipmentDataList.TryGetValue(itemText, out var data)) return;
        UpgradeImage.Source = data.Image;

        switch (UpgradeType)
        {
            case UpgradeOption.UpgradeType.NONE:
                UpgradeLabel.Content = "";
                break;
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_100:
                UpgradeLabel.Content = "100%";
                break;
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_70:
                UpgradeLabel.Content = "70%";
                break;
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_30:
                UpgradeLabel.Content = "30%";
                break;
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_15:
                UpgradeLabel.Content = "15%";
                break;
            case UpgradeOption.UpgradeType.DOMINATOR_FRAGMENT:
                UpgradeLabel.Content = "3 3 3";
                break;
            case UpgradeOption.UpgradeType.EARRING_INT_10:
                UpgradeLabel.Content = "5 3";
                break;
            case UpgradeOption.UpgradeType.ARMOR_70_ATK_1:
            case UpgradeOption.UpgradeType.ARMOR_70_MAG_1:
            case UpgradeOption.UpgradeType.PET_70_ATK_1:
            case UpgradeOption.UpgradeType.PET_70_MAG_1:
            case UpgradeOption.UpgradeType.ACCESSORY_70_ATK_1:
            case UpgradeOption.UpgradeType.ACCESSORY_70_MAG_1:
                UpgradeLabel.Content = "+1";
                break;
            case UpgradeOption.UpgradeType.PET_ATK_2:
            case UpgradeOption.UpgradeType.PET_MAG_2:
            case UpgradeOption.UpgradeType.PET_70_ATK_2:
            case UpgradeOption.UpgradeType.PET_70_MAG_2:
            case UpgradeOption.UpgradeType.MIRACLE_50_MAG_2:
            case UpgradeOption.UpgradeType.MIRACLE_50_ATK_2:
            case UpgradeOption.UpgradeType.ARMOR_70_ATK_2:
            case UpgradeOption.UpgradeType.ARMOR_70_MAG_2:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_2:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_2:
            case UpgradeOption.UpgradeType.ACCESSORY_70_ATK_2:
            case UpgradeOption.UpgradeType.ACCESSORY_70_MAG_2:
                UpgradeLabel.Content = "+2";
                break;
            case UpgradeOption.UpgradeType.PET_ATK_3:
            case UpgradeOption.UpgradeType.PET_MAG_3:
            case UpgradeOption.UpgradeType.MIRACLE_50_ATK_3:
            case UpgradeOption.UpgradeType.MIRACLE_50_MAG_3:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_3:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_3:
                UpgradeLabel.Content = "+3";
                break;
            case UpgradeOption.UpgradeType.PET_MAG_4:
            case UpgradeOption.UpgradeType.PET_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_PET_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_PET_MAG_4:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_4:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_4:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_4:
                UpgradeLabel.Content = "+4";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_PET_MAG_5:
            case UpgradeOption.UpgradeType.PREMIUM_PET_ATK_5:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_5:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_5:
                UpgradeLabel.Content = "+5";
                break;
            case UpgradeOption.UpgradeType.MAGICAL_ATK_9:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_9:
            case UpgradeOption.UpgradeType.HEART_ATK:
            case UpgradeOption.UpgradeType.HEART_MAG:
                UpgradeLabel.Content = "+9";
                break;
            case UpgradeOption.UpgradeType.MAGICAL_ATK_10:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_10:
                UpgradeLabel.Content = "+10";
                break;
            case UpgradeOption.UpgradeType.MAGICAL_MAG_11:
            case UpgradeOption.UpgradeType.MAGICAL_ATK_11:
                UpgradeLabel.Content = "+11";
                break;
            case UpgradeOption.UpgradeType.CHAOS:
                UpgradeLabel.Content = "C";
                break;
        }

        string[] split = UpgradeType.ToString().Split("_");
        string atkType = "", atkValue = "";
        if (UpgradeType > UpgradeOption.UpgradeType.SPELL_TRACE_ALL_15 && split.Length >= 2)
        {
            atkType = split[^2].Equals("ATK") ? "공격력" : "마력";
            atkValue = split[^1];
        }

        switch (UpgradeType)
        {
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_100:
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_70:
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_30:
            case UpgradeOption.UpgradeType.SPELL_TRACE_STR_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_DEX_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_INT_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_LUK_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_HP_15:
            case UpgradeOption.UpgradeType.SPELL_TRACE_ALL_15:
                TooltipManager.ToolTip = $"주문의 흔적 ({split[2]}) {split[3]}%";
                break;
            case UpgradeOption.UpgradeType.DOMINATOR_FRAGMENT:
                TooltipManager.ToolTip = "파편작";
                break;
            case UpgradeOption.UpgradeType.EARRING_INT_10:
                TooltipManager.ToolTip = "귀장식 지력 주문서 10% (지력 5, 마력 3)";
                break;
            case UpgradeOption.UpgradeType.ARMOR_70_ATK_1:
            case UpgradeOption.UpgradeType.ARMOR_70_MAG_1:
            case UpgradeOption.UpgradeType.ARMOR_70_ATK_2:
            case UpgradeOption.UpgradeType.ARMOR_70_MAG_2:
                TooltipManager.ToolTip = $"방어구 {atkType} 주문서 70% ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.PET_70_ATK_1:
            case UpgradeOption.UpgradeType.PET_70_MAG_1:
            case UpgradeOption.UpgradeType.PET_70_ATK_2:
            case UpgradeOption.UpgradeType.PET_70_MAG_2:
                TooltipManager.ToolTip = $"펫 장비 {atkType} 주문서 70% ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.ACCESSORY_70_ATK_1:
            case UpgradeOption.UpgradeType.ACCESSORY_70_MAG_1:
            case UpgradeOption.UpgradeType.ACCESSORY_70_ATK_2:
            case UpgradeOption.UpgradeType.ACCESSORY_70_MAG_2:
                TooltipManager.ToolTip = $"악세서리 {atkType} 주문서 70% ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.MIRACLE_50_MAG_2:
            case UpgradeOption.UpgradeType.MIRACLE_50_ATK_2:
            case UpgradeOption.UpgradeType.MIRACLE_50_ATK_3:
            case UpgradeOption.UpgradeType.MIRACLE_50_MAG_3:
                TooltipManager.ToolTip = $"미라클 방어구 {atkType} 주문서 50% ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.PET_ATK_2:
            case UpgradeOption.UpgradeType.PET_MAG_2:
            case UpgradeOption.UpgradeType.PET_ATK_3:
            case UpgradeOption.UpgradeType.PET_MAG_3:
            case UpgradeOption.UpgradeType.PET_MAG_4:
            case UpgradeOption.UpgradeType.PET_ATK_4:
                TooltipManager.ToolTip = $"펫장비 {atkType} 스크롤 ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_2:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_2:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_3:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_3:
            case UpgradeOption.UpgradeType.ACCESSORY_ATK_4:
            case UpgradeOption.UpgradeType.ACCESSORY_MAG_4:
                TooltipManager.ToolTip = $"악세서리 {atkType} 스크롤 ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_PET_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_PET_MAG_4:
            case UpgradeOption.UpgradeType.PREMIUM_PET_MAG_5:
            case UpgradeOption.UpgradeType.PREMIUM_PET_ATK_5:
                TooltipManager.ToolTip = $"프리미엄 펫장비 {atkType} 스크롤 ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_4:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_4:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_MAG_5:
            case UpgradeOption.UpgradeType.PREMIUM_ACCESSORY_ATK_5:
                TooltipManager.ToolTip = $"프리미엄 악세서리 {atkType} 스크롤 ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.MAGICAL_ATK_9:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_9:
            case UpgradeOption.UpgradeType.MAGICAL_ATK_10:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_10:
            case UpgradeOption.UpgradeType.MAGICAL_MAG_11:
            case UpgradeOption.UpgradeType.MAGICAL_ATK_11:
                TooltipManager.ToolTip = $"매지컬 {atkType} 스크롤 ({atkType} +{atkValue})";
                break;
            case UpgradeOption.UpgradeType.HEART_ATK:
            case UpgradeOption.UpgradeType.HEART_MAG:
                atkType = split[^1].Equals("ATK") ? "공격력" : "마력";
                TooltipManager.ToolTip = $"카르마 스페셜 하트 {atkType} 주문서 100% ({atkType} +9)";
                break;
            case UpgradeOption.UpgradeType.CHAOS:
                if (ChaosAverage == null) ChaosAverage = new Dictionary<MapleStatus.StatusType, double>();
                string tooltip = "혼돈의 주문서 작 : ";
                foreach (var pair in ChaosAverage)
                {
                    string key = pair.Key == MapleStatus.StatusType.ATTACK_POWER ? "공격력" :
                        pair.Key == MapleStatus.StatusType.MAGIC_POWER ? "마력" : pair.Key.ToString();
                    if (pair.Value > 0.0) tooltip += $"{key} +{pair.Value:F1}, ";
                }

                TooltipManager.ToolTip = tooltip[..^2];
                break;
            default:
                TooltipManager.ToolTip = "빈 슬롯";
                break;
        }
    }
}