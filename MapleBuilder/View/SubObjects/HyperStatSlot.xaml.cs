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
    private int BeforeLevel => befLevel;
    public int Level {
        get => int.Parse(ctLevelInput.Text);
        set => ctLevelInput.Text = value.ToString();
    }

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
        DependencyProperty.Register("StatType", typeof(MapleStatus.StatusType), typeof(HyperStatSlot));

    public MapleStatus.StatusType StatType
    {
        get => (MapleStatus.StatusType) GetValue(STAT_PROPERTY);
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

    private double GetIncrease(MapleStatus.StatusType type, int level)
    {
        return type switch
        {
            MapleStatus.StatusType.STR_FLAT => 30,
            MapleStatus.StatusType.DEX_FLAT => 30,
            MapleStatus.StatusType.INT_FLAT => 30,
            MapleStatus.StatusType.LUK_FLAT => 30,
            MapleStatus.StatusType.HP_RATE => 2,
            MapleStatus.StatusType.MP_RATE => 2,
            MapleStatus.StatusType.DF_TT_PP => level <= 10 ? 10 : 0,
            MapleStatus.StatusType.CRITICAL_CHANCE => level > 5 ? 2 : 1,
            MapleStatus.StatusType.CRITICAL_DAMAGE => 1,
            MapleStatus.StatusType.IGNORE_DEF => 3,
            MapleStatus.StatusType.DAMAGE => 3,
            MapleStatus.StatusType.BOSS_DAMAGE => level > 5 ? 4 : 3,
            MapleStatus.StatusType.COMMON_DAMAGE => level > 5 ? 4 : 3,
            MapleStatus.StatusType.ABN_STATUS_RESIS => level > 5 ? 2 : 1,
            MapleStatus.StatusType.ATTACK_AND_MAGIC => 3,
            MapleStatus.StatusType.EXP_INCREASE => level > 10 ? 1 : 0.5,
            MapleStatus.StatusType.ARCANE_FORCE => level > 10 ? 10 : 5,
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