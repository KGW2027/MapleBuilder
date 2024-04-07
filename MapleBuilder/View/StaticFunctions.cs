using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MapleBuilder.View;

public static class StaticFunctions
{
    public static IEnumerable<T> GetChildren<T>(this DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) yield break;

        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                yield return typedChild;
            }

            foreach (var descendant in GetChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }
}