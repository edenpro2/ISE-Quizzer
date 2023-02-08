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
    private const int TrueOrFalsePart1 = 10;
    private const int MultipleChoicePar1 = 14;
    private const int TrueOrFalsePart2 = 5;
    private const int MultipleChoicePar2 = 6;

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

    private void PracticeTestBtn_Click(object sender, RoutedEventArgs e)
    {
        /* quiz 0 - 10
           10 true/false
           14 multiple
           
           quiz 11 - 12
           5 true/false
           6 multiple   */

        Quiz quiz;
        var rand = new Random(DateTime.Now.Millisecond);
        var questions = new List<Question>();

        var questionSet0to10 = new List<Question>();
        _quizzes.ForEach(qz => qz.Questions.ForEach(q => questionSet0to10.Add(q)));
        var bool1to10 = questionSet0to10.Where(q => q.PossibleAnswers.Count == 2).OrderBy(_ => rand.Next()).Take(TrueOrFalsePart1).ToList();
        var multiple1to10 = questionSet0to10.Where(q => q.PossibleAnswers.Count > 2).OrderBy(_ => rand.Next()).Take(MultipleChoicePar1).ToList();

        questions.AddRange(bool1to10);
        questions.AddRange(multiple1to10);


        var questionSet11to12 = new List<Question>();
        _quizzes.ForEach(qz => qz.Questions.ForEach(q => questionSet11to12.Add(q)));
        var bool11to12 = questionSet0to10.Where(q => q.PossibleAnswers.Count == 2).OrderBy(_ => rand.Next()).Take(TrueOrFalsePart2).ToList();
        var multiple11to12 = questionSet0to10.Where(q => q.PossibleAnswers.Count > 2).OrderBy(_ => rand.Next()).Take(MultipleChoicePar2).ToList();

        questions.AddRange(bool11to12);
        questions.AddRange(multiple11to12);

        new QuizWindow(questions,
            maxQuestions: TrueOrFalsePart1 + TrueOrFalsePart2 + MultipleChoicePar1 + MultipleChoicePar2,
            _isTimed).Show();

        Close();

    }
}
