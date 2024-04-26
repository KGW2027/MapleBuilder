using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class UpgradePreviewer : UserControl
{
    public UpgradePreviewer()
    {
        InitializeComponent();
        ClearPreviewer();
    }

    #region XAML Property

    public static readonly RoutedEvent OpenUpgradeEditorEvent = EventManager.RegisterRoutedEvent(
        nameof(OpenUpgradeEditor), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UpgradePreviewer)
    );

    public event RoutedEventHandler OpenUpgradeEditor
    {
        add => AddHandler(OpenUpgradeEditorEvent, value);
        remove => RemoveHandler(OpenUpgradeEditorEvent, value);
    }

    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(CommonItem), typeof(UpgradePreviewer), new PropertyMetadata(null, OnTargetItemChanged)
    );

    private static void OnTargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (UpgradePreviewer) d;
        control.Update();
    }

    public CommonItem? TargetItem
    {
        get => (CommonItem?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }
    
    #endregion

    private void ClearPreviewer()
    {
        Previewer.Children.Clear();

        Polygon plus = new Polygon
        {
            Stroke = Brushes.White,
            StrokeThickness = 1.0,
            Margin = new Thickness(-1, -1, 0, 0),
            Points = new PointCollection
            {
                new(14, 10), new(18, 10), new(18, 14), new(22, 14), new(22, 18),
                new(18, 18), new(18, 22), new(14, 22), new(14, 18), new(10, 18),
                new(10, 14), new(14, 14)
            }
        };
        Border border = new Border
        {
            Width = 32,
            Height = 32,
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(1),
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = plus
        };
        Previewer.Children.Add(border);
    }
    
    private void Update()
    {
        if (TargetItem?.Upgrades == null) return;
        
        ClearPreviewer();
        foreach (var upg in TargetItem.Upgrades)
        {
            UpgradeSlot slot = new UpgradeSlot
            {
                ChaosAverage = TargetItem.ChaosAverage,
                UpgradeType = upg,
                Margin = new Thickness(2, 0, 0, 0)
            };
            Previewer.Children.Add(slot);
        }
        
    }
    
    private void OnEditorOpenButtonClicked(object sender, MouseButtonEventArgs e)
    {
        var newEvent = new RoutedEventArgs(OpenUpgradeEditorEvent)
        {
            Handled = true
        };
        RaiseEvent(newEvent);
    }
}