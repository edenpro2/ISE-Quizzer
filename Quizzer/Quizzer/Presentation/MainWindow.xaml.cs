using QuizApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace QuizApp.Presentation;

public partial class MainWindow  
{
    private readonly List<Quiz> _quizzes = Database.LoadQuizzes();
    private const int DefaultMax = 10;
    private int _maxQuestions = DefaultMax;
    private bool _fullQuizMode;
    private bool _isTimed;

    public MainWindow() => InitializeComponent();

    private void Quiz_BtnClick(object sender, RoutedEventArgs e)
    {
        // extract number from button content (Quiz * --> *) and load quiz
        var s = (sender as Button).Content.ToString();
        LoadQuiz(int.Parse(s?.Replace("Quiz Week", "") ?? string.Empty));
    }

    private void LoadQuiz(int quizNum)
    {
        var quiz = _quizzes[quizNum - 1];
        var allQuestions = quiz.Questions.ToList();
        var questions = new List<Question>();
        var rand = new Random(DateTime.Now.Millisecond);
        var total = allQuestions.Count;

        if (_fullQuizMode)
            _maxQuestions = allQuestions.Count;

        for (var i = 0; i < _maxQuestions; i++)
        {
            var index = rand.Next(total);

            questions.Add(allQuestions[index]);
            allQuestions.RemoveAt(index);
            total--;
        }

        new QuizWindow(questions, _maxQuestions, _isTimed).Show();
        // close this window
        Close(); 
    }

    private void SearchBtn_Click(object sender, RoutedEventArgs e)
    {
        new SearchWindow(_quizzes).Show();
    }

    private void MaxQuizToggle_OnClick(object sender, RoutedEventArgs e)
    {
        _fullQuizMode = (bool)(sender as ToggleButton).IsChecked; //isChecked from bool? to bool
    }

    private void TimerToggle_Click(object sender, RoutedEventArgs e)
    {
         _isTimed = (bool)(sender as ToggleButton).IsChecked; //isChecked from bool? to bool
    }
}
