using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleBuilder.Control;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.View.SubObjects;

public partial class OverviewTopbar : UserControl
{
    public OverviewTopbar()
    {
        InitializeComponent();
    }

    public async void UpdateProfileImage()
    {
        while (BuilderDataContainer.CharacterInfo == null || BuilderDataContainer.CharacterInfo.PlayerImage.Length == 0) await Task.Delay(100);
        
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = new MemoryStream(BuilderDataContainer.CharacterInfo.PlayerImage);
        bitmapImage.EndInit();
        
        await Dispatcher.BeginInvoke(() =>
        {
            ctCharacterImage.Source = bitmapImage;
        });
    }

    private void OnChangeScreenToEquip(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.OVERVIEW_EQUIPMENT;
    }

    private void OnChangeScreenToStatSymbol(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.STAT_SYMBOL;
    }

    private void OnChangeScreenToSpecialEquip(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.SPECIAL_EQUIPS;
    }

    private void OnChangeScreenToCashEquip(object sender, RoutedEventArgs e)
    {
        RenderFrame.RenderType = RenderFrame.RenderScreenType.CASH_EQUIPS;
    }
}