using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleAPI.Enum;

namespace MapleBuilder.View.SubObjects;

public partial class HyperStatSlot : UserControl
{
    public HyperStatSlot()
    {
        InitializeComponent();
        ctLabel.Content = Text;
        ctLevelInput.Text = "0";
        ctDmgIncrease.Content = "00.00%";
        befLevel = 0;
    }
    
    #region Public Properties

    private int befLevel;
    private int Level => int.Parse(ctLevelInput.Text);
    private int BeforeLevel => befLevel;

    public double Delta => GetDelta();
    #endregion

    #region XAML Properties
    
    public static readonly DependencyProperty TEXT_PROPERTY =
        DependencyProperty.Register("Text", typeof(string), typeof(HyperStatSlot), new PropertyMetadata(null, OnTextChanged));
    
    public string Text
    {
        get => (string) GetValue(TEXT_PROPERTY);
        set => SetValue(TEXT_PROPERTY, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (HyperStatSlot)d;
        control.ctLabel.Content = e.NewValue as string;
    }
    
    public static readonly DependencyProperty STAT_PROPERTY =
        DependencyProperty.Register("StatType", typeof(MapleHyperStat.StatType), typeof(HyperStatSlot));

    public MapleHyperStat.StatType StatType
    {
        get => (MapleHyperStat.StatType) GetValue(STAT_PROPERTY);
        set => SetValue(STAT_PROPERTY, value);
    }

    private void CheckTextIsNumber(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.Length < 1 || e.Text[0] < '0' || e.Text[0] > '9';
    }

    private static readonly RoutedEvent LEVEL_CHANGED_EVENT = EventManager.RegisterRoutedEvent("LevelChanged",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HyperStatSlot));
    
    public event RoutedEventHandler LevelChanged
    {
        add => AddHandler(LEVEL_CHANGED_EVENT, value);
        remove => RemoveHandler(LEVEL_CHANGED_EVENT, value);
    }
    
    #endregion

    private void OnLevelTextChanged(object sender, TextChangedEventArgs e)
    {
        if(!int.TryParse(((TextBox) sender).Text, out int level)) return;
        level = Math.Clamp(level, 0, 15);
        ((TextBox) sender).Text = level.ToString();
        RoutedEventArgs newEventArgs = new RoutedEventArgs(LEVEL_CHANGED_EVENT);
        RaiseEvent(newEventArgs);

        befLevel = level;
    }

    private double GetIncrease(MapleHyperStat.StatType type, int level)
    {
        return type switch
        {
            MapleHyperStat.StatType.STR => 30,
            MapleHyperStat.StatType.DEX => 30,
            MapleHyperStat.StatType.INT => 30,
            MapleHyperStat.StatType.LUK => 30,
            MapleHyperStat.StatType.HP => 2,
            MapleHyperStat.StatType.MP => 2,
            MapleHyperStat.StatType.DF_TF_PP => level <= 10 ? 10 : 0,
            MapleHyperStat.StatType.CRITCIAL_CHANCE => level > 5 ? 2 : 1,
            MapleHyperStat.StatType.CRITICAL_DAMAGE => 1,
            MapleHyperStat.StatType.IGNORE_ARMOR => 3,
            MapleHyperStat.StatType.DAMAGE => 3,
            MapleHyperStat.StatType.BOSS_DAMAGE => level > 5 ? 4 : 3,
            MapleHyperStat.StatType.COMMON_DAMAGE => level > 5 ? 4 : 3,
            MapleHyperStat.StatType.IMMUNE => level > 5 ? 2 : 1,
            MapleHyperStat.StatType.ATTACK_POWER => 3,
            MapleHyperStat.StatType.EXP_UP => level > 10 ? 1 : 0.5,
            MapleHyperStat.StatType.ARCANE_FORCE => level > 10 ? 10 : 5,
            _ => 0
        };
    }

    private double GetDelta()
    {
        double bv = 0, nv = 0;
        for (int idx = 1; idx <= Math.Max(BeforeLevel, Level); idx++)
        {
            double increase = GetIncrease(StatType, idx);
            if (idx <= BeforeLevel) bv += increase;
            if (idx <= Level) nv += increase;
        }
        return nv - bv;
    }
}