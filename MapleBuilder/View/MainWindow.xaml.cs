using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View
{
    public partial class MainWindow : Window
    {
        public static Window? Window { get; private set; }

        public MainWindow()
        {
            Window = this;
            
            InitializeComponent();
            SetWindowTitle();
            new Thread(() =>
            {
                if (!File.Exists("./ItemExtractorResult.json")) return;
                WzDatabase.Instance.ToString();
            }).Start();
        }

        /// <summary>
        /// 윈도우의 이름을 Maple Builder [Version] 으로 변경합니다.
        /// Version은 csproj의 InformationVersion에서 설정됩니다.
        /// </summary>
        private void SetWindowTitle()
        {
            object[] asmVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            string infoVersion = asmVersion.Length <= 0 ? "unknown" : ((AssemblyInformationalVersionAttribute) asmVersion[0]).InformationalVersion;
            WindowSelf.Title = $"Maple Builder {infoVersion}";
        }
    }
}