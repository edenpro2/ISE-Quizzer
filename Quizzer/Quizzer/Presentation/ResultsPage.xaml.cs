using System.Windows;

namespace QuizApp.Presentation;

public partial class ResultsWindow
{
    public int TotalCorrect { get; set; }
    public int TotalQuestions { get; set; }

    public ResultsWindow(int totalCorrect, int totalQuestions)
    {
        TotalCorrect = totalCorrect;
        TotalQuestions = totalQuestions;
        InitializeComponent();
    }

    private void MainBtn_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}
