using System;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control;

namespace MapleBuilder.View.SubObjects.StatSymbol;

public partial class HyperStatSlot : UserControl
{
    public int Level {
        set => InputLevel.Text = value.ToString();
    }
    
    public HyperStatSlot()
    {
        InitializeComponent();
        DisplayLabel.Content = Text;
        InputLevel.Text = "0";
        DisplayDamageInc.Content = "00.00%";

        InputLevel.PreviewTextInput += StaticFunctions.CheckTextNumberOnly;
    }

    #region XAML Properties
    
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(HyperStatSlot), new PropertyMetadata(null, OnTextChanged));
    
    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (HyperStatSlot)d;
        control.DisplayLabel.Content = e.NewValue as string;
    }
    
    public static readonly DependencyProperty StatProperty =
        DependencyProperty.Register("StatType", typeof(MapleStatus.StatusType), typeof(HyperStatSlot));

    public MapleStatus.StatusType StatType
    {
        get => (MapleStatus.StatusType) GetValue(StatProperty);
        set => SetValue(StatProperty, value);
    }
    
    #endregion

    private void OnLevelTextChanged(object sender, TextChangedEventArgs e)
    {
        if (GlobalDataController.Instance.PlayerInstance == null) return;
        if(!int.TryParse(((TextBox) sender).Text, out int level)) return;
        level = Math.Clamp(level, 0, 15);
        GlobalDataController.Instance.PlayerInstance.HyperStat[StatType] = level;
    }
}