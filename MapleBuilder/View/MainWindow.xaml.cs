using System.Reflection;
using System.Threading;
using System.Windows;
using MapleBuilder.Control;

namespace MapleBuilder.View
{
    public partial class MainWindow : Window
    {
        public static Window? Window { get; private set; }

        // TODO[Priority High] : MVC 패턴에 대해 공부하기
        public MainWindow()
        {
            Window = this;
            
            InitializeComponent();
            SetWindowTitle();
            new Thread(() =>
            {
                ResourceManager.GetItemIcon("카오스 벨룸의 헬름"); // Pre load Iconlist
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