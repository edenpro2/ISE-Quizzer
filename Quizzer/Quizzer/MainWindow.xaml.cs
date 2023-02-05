using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Quizzer;

public partial class MainWindow : Window
{
    List<Quiz> quizzes = Database.loadQuizzes();
    QuizPage QuizPage;
    const int MAXQUESTIONS = 10;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Quiz_0_Click(object sender, RoutedEventArgs e) => LoadQuiz(0);
    private void Quiz_1_Click(object sender, RoutedEventArgs e) => LoadQuiz(1);
    private void Quiz_2_Click(object sender, RoutedEventArgs e) => LoadQuiz(2);
    private void Quiz_3_Click(object sender, RoutedEventArgs e) => LoadQuiz(3);

    private void LoadQuiz(int quiz_num)
    {
        var quiz = quizzes[quiz_num];
        var all_questions = quiz.Questions.ToList();
        var questions = new List<Question>(MAXQUESTIONS);
        var rand = new Random(DateTime.Now.Millisecond);
        var total = all_questions.Count;
        int index;

        for (int i = 0; i < MAXQUESTIONS; i++)
        {
            index = rand.Next(total);
            questions.Add(all_questions[index]);
            all_questions.RemoveAt(index);
            total--;
        }

        QuizPage = new QuizPage(questions);
        _NavigationFrame.Navigate(QuizPage);
    }
}
