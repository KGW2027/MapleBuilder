using System.Windows;
using MapleBuilder.Control;

namespace MapleBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ResourceManager initResourceManager = ResourceManager.Instance;
        }
    }
}