using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.View.SubObjects;

public partial class OverviewTopbar : UserControl
{
    public OverviewTopbar()
    {
        InitializeComponent();
        
        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {
        Dispatcher.BeginInvoke(() =>
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = new MemoryStream(pdata.PlayerImage);
            bitmapImage.EndInit();

            PlayerThumbnail.Source = bitmapImage;
            PlayerGuild.Content = pdata.PlayerGuild;
            PlayerName.Content = pdata.PlayerName;
            PlayerLvAndClass.Content = $"LV {pdata.Level} {MapleClass.GetMapleClassString(pdata.Class)}";
            PlayerSymbol.Content =
                $"Arcane {pdata[PlayerData.StatSources.SYMBOL, MapleStatus.StatusType.ARCANE_FORCE]}\nAuthentic {pdata[PlayerData.StatSources.SYMBOL, MapleStatus.StatusType.AUTHENTIC_FORCE]}";
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