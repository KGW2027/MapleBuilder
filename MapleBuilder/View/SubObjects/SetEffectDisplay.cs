using System.Windows;

namespace MapleBuilder.View.SubObjects;

public class SetEffectDisplay : System.Windows.Controls.Control
{
    static SetEffectDisplay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SetEffectDisplay), new FrameworkPropertyMetadata(typeof(SetEffectDisplay)));
    }

    public static readonly DependencyProperty SetNameProperty =
        DependencyProperty.Register(nameof(SetName), typeof(string), typeof(SetEffectDisplay), new PropertyMetadata(string.Empty));

    public string SetName
    {
        get => (string)GetValue(SetNameProperty);
        set => SetValue(SetNameProperty, value);
    }

    public static readonly DependencyProperty SetCountProperty =
        DependencyProperty.Register(nameof(SetCount), typeof(string), typeof(SetEffectDisplay), new PropertyMetadata(string.Empty));

    public string SetCount
    {
        get => (string)GetValue(SetCountProperty);
        set => SetValue(SetCountProperty, value);
    }
}