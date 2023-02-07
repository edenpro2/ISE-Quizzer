using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using QuizApp.BL;

namespace QuizApp.Presentation;

public partial class MainWindow : Window
{
    private List<Quiz> quizzes = Database.loadQuizzes();
    private const int DEFUALTMAX = 10;
    private int MaxQuestions = DEFUALTMAX;
    private bool fullQuizMode;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Quiz_BtnClick(object sender, RoutedEventArgs e)
    {
        // extract number from button content (Quiz * --> *) and load quiz
        var s = ((Button)sender).Content.ToString();
        LoadQuiz(int.Parse(s.Replace("Quiz Week", "")));
    }

    private void LoadQuiz(int quizNum)
    {
        var quiz = quizzes[quizNum - 1];
        var allQuestions = quiz.Questions.ToList();
        var questions = new List<Question>();
        var rand = new Random(DateTime.Now.Millisecond);
        var total = allQuestions.Count;
        int index;

        if (fullQuizMode)
            MaxQuestions = allQuestions.Count;

        for (var i = 0; i < MaxQuestions; i++)
        {
            index = rand.Next(total);
            questions.Add(allQuestions[index]);
            allQuestions.RemoveAt(index);
            total--;
        }

        new QuizWindow(questions, MaxQuestions).Show();
        // close this window
        Close(); 
    }

    private void Unimplemented_Click(object sender, RoutedEventArgs e) { /* noop */ }

    private void SearchBtn_Click(object sender, RoutedEventArgs e)
    {
        new SearchWindow(quizzes).Show();
    }

    private void MaxQuizToggle_OnClick(object sender, RoutedEventArgs e)
    {
        fullQuizMode = (bool)((ToggleButton)sender).IsChecked;
    }
}
