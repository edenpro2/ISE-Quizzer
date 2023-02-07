using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BL;

namespace Presentation;

public partial class QuizWindow : Window, INotifyPropertyChanged
{
    public List<Question> _questions = new();
    public Question CurrentQuestion
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            OnPropertyChanged();
        }
    }
    public int totalCorrect = 0;
    private const int ALLOTED_SEC = 30;
    

    // Timer logic
    private Clock clock;
    private int _remainingSeconds = ALLOTED_SEC;
    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set
        {
            _remainingSeconds = value;
            OnPropertyChanged();
            if (_remainingSeconds == 0)
                NextBtn_Click(this, null);
        }
    }

    private List<Border> _border_refs = new List<Border>();
    private Question? _currentQuestion;
    private int currentQuestionNumber = 0;

    // Green
    private SolidColorBrush? _correctColor = new BrushConverter().ConvertFrom("#6632CD32") as SolidColorBrush;

    // Red
    private SolidColorBrush? _wrongColor = new BrushConverter().ConvertFrom("#66DC143C") as SolidColorBrush;

    public QuizWindow(List<Question> questions)
    {
        _questions = questions;
        CurrentQuestion = _questions[currentQuestionNumber];
        clock = new Clock(ALLOTED_SEC, this);
        InitializeComponent();
        SetQuestion(CurrentQuestion);
        clock.Start();
    }

    public void SetQuestion(Question question)
    {
        // reorder multiple choice questions
        if (question._possibleAnswers.Count > 2)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            question._possibleAnswers = question._possibleAnswers.OrderBy(_ => rand.Next()).ToList();
        }

        foreach (var ans in question._possibleAnswers)
        {
            var txt = new TextBlock() { Text = ans, TextWrapping = TextWrapping.Wrap };
            var rb = new RadioButton() { Content = txt, IsChecked = false, FontSize = 22, VerticalContentAlignment = VerticalAlignment.Center };
            var border = new Border();
            bool isFirstTry = true;
            border.Child = rb;
            _border_refs.Add(border);
            rb.Checked += (sender, args) =>
            {
                rb.FontWeight = FontWeights.Bold;
                var answer = ((TextBlock)rb.Content).Text;

                // answer is correct
                if (answer.Contains(_questions[currentQuestionNumber]._answer))
                {
                    if (isFirstTry == true)
                    {
                        isFirstTry = false;
                        totalCorrect++;
                    }

                    clock.Stop();
                    border.Background = _correctColor;
                    rb.BorderBrush = _correctColor;
                    rb.BorderThickness = new Thickness(2);
                }
                else // answer incorrect
                {
                    if (isFirstTry == true)
                        isFirstTry = false;

                    border.Background = _wrongColor;
                    rb.BorderBrush = _wrongColor;
                    rb.BorderThickness = new Thickness(2);
                }
            };

            rb.Unchecked += (sender, args) =>
            {
                rb.FontWeight = FontWeights.Normal;
                border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff00");
                rb.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#222222");
                rb.BorderThickness = new Thickness(1);
            };

            RadioStackPanel.Children.Add(border);
        }
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
        if (clock.isRunning)
            clock.Stop();

        clock = new Clock(ALLOTED_SEC, this); //reset
        RemainingSeconds = ALLOTED_SEC;

        // if 10 questions answered
        if (currentQuestionNumber + 1 >= 10)
        {
            new ResultsWindow(totalCorrect).Show();
            Close();
            return;
        }

        // hide the radio buttons 
        // TODO: needs to be reimplemented in a way that doesn't require the loop
        foreach (var bord in _border_refs) { bord.Visibility = Visibility.Collapsed; }

        CurrentQuestion = _questions[++currentQuestionNumber];
        SetQuestion(CurrentQuestion);
        clock.Start();
    }

    //avi's addition
    private void PrevBtn_Click(object sender, RoutedEventArgs e)
    {
        if (clock.isRunning)
            clock.Stop();

        if (currentQuestionNumber - 1 < 0)
            return;

        else //dont go further back than the 1st question
        {
            foreach (var bord in _border_refs) { bord.Visibility = Visibility.Collapsed; }
            CurrentQuestion = _questions[--currentQuestionNumber]; //go back 1 question
            SetQuestion(CurrentQuestion);
        }
    }
    private void MainBtn_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    #region INotify
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

}
