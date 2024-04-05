using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapleAPI.DataType;

namespace MapleBuilder.View.SubFrames;

public partial class RenderOverview : UserControl
{
    private static RenderOverview? selfInstance;

    public static void Update(CharacterInfo cInfo)
    {
        if (selfInstance == null) return;

        UpdateProfileImage(cInfo);
        selfInstance.ctCharacterName.Content = cInfo.UserName;
        selfInstance.ctCharacterGuild.Content = $"길드 {cInfo.GuildName}";
        selfInstance.ctCharacterLevelAndClass.Content = $"Lv. {cInfo.Level} {cInfo.ClassString}";

        int arcane = 1350, authentic = 660;
        selfInstance.ctCharacterSymbol.Content = $"아케인 {arcane:N0}\n어센틱 {authentic:N0}";
        selfInstance.ctEquips.Clear();
        selfInstance.ctEquips.UpdateEquipments(cInfo);
        foreach (var tem in cInfo.Items)
        {
            Console.WriteLine(tem.EquipType.ToString());
        }
    }

    private static async void UpdateProfileImage(CharacterInfo cInfo)
    {
        while (cInfo.PlayerImage.Length == 0) await Task.Delay(100);
        
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = new MemoryStream(cInfo.PlayerImage);
        bitmapImage.EndInit();
        selfInstance!.Dispatcher.Invoke(() =>
        {
            selfInstance.ctCharacterImage.Source = bitmapImage;
        });
    }
    
    
    public RenderOverview()
    {
        selfInstance = this;
        
        InitializeComponent();
    }
    
    
}