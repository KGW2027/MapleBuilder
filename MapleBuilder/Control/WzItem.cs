using System;
using System.Windows.Media.Imaging;

namespace MapleBuilder.Control;

public class WzItem
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public BitmapImage IconRaw { get; }

    protected internal WzItem(string name, string iconPath, string desc = "")
    {
        Name = name;
        Description = desc;
        IconRaw = new BitmapImage();
        IconRaw.BeginInit();
        string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iconPath);
        IconRaw.UriSource = new Uri(fullPath);
        IconRaw.EndInit();
    }
}