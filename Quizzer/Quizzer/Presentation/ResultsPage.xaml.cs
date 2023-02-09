using System;
using QuizApp.BL;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuizApp.Presentation
{
    public partial class ResultsWindow
    {
        public int TotalCorrect { get; }
        public int TotalQuestions { get; }
        public int Score { get; }
        public List<Question> incorrectAnswered { get; }
 

        public ResultsWindow(int totalCorrect, int totalQuestions, List<Question> incorrect)
        {
            TotalCorrect = totalCorrect;
            TotalQuestions = totalQuestions;
            incorrectAnswered = incorrect;
            Score = TotalCorrect / TotalQuestions;
 
            InitializeComponent();

        }

        private void MainBtn_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

    }
}
