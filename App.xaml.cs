using System.Windows;
using Twitify.Windows;
using Forms = System.Windows.Forms;

namespace Twitify
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var screen = Forms.Screen.PrimaryScreen;
            var area = screen.WorkingArea;
            var window = new MainWindow
            {
                Left = area.Right - 430,
                Top = area.Bottom - 107
            };

            window.Show();
        }
    }
}