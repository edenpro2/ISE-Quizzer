using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BL;

namespace Presentation;

public partial class MainWindow : Window
{
    List<Quiz> quizzes = Database.loadQuizzes();
    const int MAXQUESTIONS = 10;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Quiz_BtnClick(object sender, RoutedEventArgs e)
    {
        // extract number from button content (Quiz * --> *) and load quiz
        string s = ((Button)sender).Content.ToString();
        LoadQuiz(int.Parse(s.Replace("Quiz Week", "")));
    }

    private void LoadQuiz(int quiz_num)
    {
        var quiz = quizzes[quiz_num - 1];
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

        new QuizWindow(questions).Show();
        // close this window
        Close(); 
    }

    private void Unimp_Click(object sender, RoutedEventArgs e) { /* noop */ }

}
