using System.Linq;
using System.Windows;
using QuizApp.BL;

namespace QuizApp.Presentation
{
    public partial class App
    {
        public App()
        {
            Startup += App_Startup;
        }

        private static void App_Startup(object sender, StartupEventArgs e)
        {
            var q = Database.LoadQuizzes();
            new ResultsWindow(5, 10, q[0].Take(10).ToList()).Show();
            // new MainWindow().Show();
        }
    }
}

