using System.Windows;

namespace FreewriteWindows
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set default font
            var fontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            this.Resources["DefaultFont"] = fontFamily;
        }
    }
}