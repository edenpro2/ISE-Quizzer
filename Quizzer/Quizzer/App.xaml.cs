using System.Windows;
using QuizApp.Presentation;

namespace QuizApp
{
    public partial class App
    {
        public App()
        {
            Startup += App_Startup;
        }

        private static void App_Startup(object sender, StartupEventArgs e)
        {
            new MainWindow().Show();
        }
    }
}
