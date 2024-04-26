using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapleBuilder.View.SubObjects.Equipment.EditEquips;

public partial class StarforceEditor : UserControl
{
    public class StarforceChangedArgs : RoutedEventArgs
    {
        public int NewStarforce;

        internal StarforceChangedArgs(RoutedEvent e, int sfv) : base(e)
        {
            NewStarforce = sfv;
        }
    }
    
    private static readonly double[][] STAR_VECTORS =
    {
        new[]{0.0, 1.0}, 
        new[]{0.22456, 0.30898},
        new[]{0.95, 0.30898},
        new[]{0.364, -0.1183},
        new[]{0.5878, -0.809},
        new[]{0.0, -0.38},
        new[]{-0.5878, -0.809},
        new[]{-0.364, -0.1183},
        new[]{-0.95, 0.30898},
        new[]{-0.22456, 0.30898},
        new[]{0.0, 1.0}
    };

    private static readonly Brush DISABLED = new SolidColorBrush(Color.FromRgb(0x31, 0x31, 0x31));
    private static readonly Brush ENABLED = new SolidColorBrush(Color.FromRgb(0xB2, 0x7D, 0x00));
    
    public StarforceEditor()
    {
        InitializeComponent();

        int offsetX = 0;

        PointCollection collection = new PointCollection();
        foreach (var pos in STAR_VECTORS)
            collection.Add(new Point
            {
                X = (pos[0] + 1) * 7.5,
                Y = 15 - (pos[1] + 1) * 7.5
            });
        
        for (int draw = 1; draw <= 25; draw++)
        {
            Polygon polygon = new Polygon
            {
                Width = 15,
                Height = 15,
                Fill = DISABLED,
                Stroke = Brushes.Black,
                StrokeThickness = 0.4,
                Margin = new Thickness(offsetX, 0, 0, 0),
                Points = collection,
                Tag = draw,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            polygon.MouseLeftButtonDown += PolygonSelected;
            polygon.MouseRightButtonDown += PolygonDeselected;
            
            offsetX += draw % 5 == 0 ? 19 : 11;
            Grid.Children.Add(polygon);
        }
    }

    private void PolygonDeselected(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Polygon {Tag: int sfv}) return;
        Starforce = Math.Max(sfv-1, 0);

        StarforceChangedArgs args = new StarforceChangedArgs(StarforceChangedEvent, Starforce);
        RaiseEvent(args);
    }

    private void PolygonSelected(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Polygon {Tag: int sfv}) return;
        Starforce = sfv;

        StarforceChangedArgs args = new StarforceChangedArgs(StarforceChangedEvent, Starforce);
        RaiseEvent(args);
    }

    private void Update()
    {
        for (int idx = 1; idx <= 25; idx++)
        {
            var visiblity = idx <= MaxStarforce ? Visibility.Visible : Visibility.Collapsed;
            var brush = idx <= Starforce ? ENABLED : DISABLED;

            var polygon = (Polygon) Grid.Children[idx - 1];
            polygon.Visibility = visiblity;
            polygon.Fill = brush;
        }
    }
    
    #region XAML Property
    
    private static void OnSfvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (StarforceEditor) d;
        control.Update();
    }

    public static readonly DependencyProperty StarforceProperty = DependencyProperty.Register(
        nameof(Starforce), typeof(int), typeof(StarforceEditor), new PropertyMetadata(-1, OnSfvChanged));
    
    public int Starforce
    {
        get => (int) GetValue(StarforceProperty);
        set => SetValue(StarforceProperty, value);
    }

    public static readonly DependencyProperty MaxStarforceProperty = DependencyProperty.Register(
        nameof(MaxStarforce), typeof(int), typeof(StarforceEditor), new PropertyMetadata(-1, OnSfvChanged));

    public int MaxStarforce
    {
        get => (int) GetValue(MaxStarforceProperty);
        set => SetValue(MaxStarforceProperty, value);
    }

    public static readonly RoutedEvent StarforceChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(StarforceChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StarforceEditor));

    public event RoutedEventHandler StarforceChanged
    {
        add => AddHandler(StarforceChangedEvent, value);
        remove => RemoveHandler(StarforceChangedEvent, value);
    }

    #endregion
}