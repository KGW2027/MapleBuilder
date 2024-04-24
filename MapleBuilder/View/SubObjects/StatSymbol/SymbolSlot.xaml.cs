using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.StatSymbol;

public partial class SymbolSlot : UserControl
{
    
    private static readonly Brush NON_HOVER = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
    private static readonly Brush HOVER = new SolidColorBrush(Color.FromArgb(0xA0, 0x00, 0x00, 0x00));
    
    public SymbolSlot()
    {
        InitializeComponent();
    }
    
    #region XAML Property
    
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register("Symbol", typeof(MapleSymbol.SymbolType), typeof(SymbolSlot), new PropertyMetadata(MapleSymbol.SymbolType.UNKNOWN, OnSymbolChanged));

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var symbol = (MapleSymbol.SymbolType) e.NewValue;
        if (symbol == MapleSymbol.SymbolType.UNKNOWN) return;
        
        var control = (SymbolSlot) d;
        string name = MapleSymbol.GetSymbolTypeString(symbol);
        control.HoverText.Content = name.Split(":")[1].Trim();
        
        if (!WzDatabase.Instance.EquipmentDataList.TryGetValue(name, out var item)) return;
        control.ItemImage.Source = item.Image;
    }

    public MapleSymbol.SymbolType Symbol
    {
        get => (MapleSymbol.SymbolType) GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    
    public static readonly DependencyProperty LevelProperty =
        DependencyProperty.Register(nameof(Level), typeof(int), typeof(SymbolSlot), new PropertyMetadata(-1, OnLevelChanged));

    private static void OnLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SymbolSlot) d;
        if (control.Symbol == MapleSymbol.SymbolType.UNKNOWN) return;
        int max = (int) control.Symbol <= 0x10 ? 20 : 11;
        control.LevelInput.Text = Math.Clamp((int) e.NewValue, 0, max).ToString();
    }

    public int Level
    {
        get => (int) GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }
    
    #endregion

    private void OnHover(object sender, MouseEventArgs e)
    {
        Border.Background = HOVER;
        HoverText.Visibility = Visibility.Visible;
    }

    private void OnHoverEnd(object sender, MouseEventArgs e)
    {
        Border.Background = NON_HOVER;
        HoverText.Visibility = Visibility.Collapsed;
    }

    private void CheckIsNumber(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _);
    }

    private void LevelChanged(object sender, TextChangedEventArgs e)
    {
        if (GlobalDataController.Instance.PlayerInstance == null) return;
        int max = (int) Symbol <= 0x10 ? 20 : 11;
        GlobalDataController.Instance.PlayerInstance[Symbol] = Math.Clamp(int.Parse(LevelInput.Text), 0, max);
    }
}