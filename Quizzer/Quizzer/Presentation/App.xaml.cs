using System.Windows;

namespace QuizApp.Presentation;

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

