using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quizzer;

public partial class QuizWindow : Window, INotifyPropertyChanged
{
    // Public:
    public List<Question> _questions = new List<Question>();
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
    // Private:
    private List<Border> _border_refs = new List<Border>();
    private Question? _currentQuestion;
    private int current = 0;
    private Frame navFrame;
    // Green
    private SolidColorBrush? _correctColor = new BrushConverter().ConvertFrom("#6632CD32") as SolidColorBrush;

    // Red
    private SolidColorBrush? _wrongColor = new BrushConverter().ConvertFrom("#66DC143C") as SolidColorBrush;

    public QuizWindow(List<Question> questions, Frame nav_frame)
    {
        _questions = questions;
        CurrentQuestion = _questions[current];
        navFrame = nav_frame;
        InitializeComponent();
        SetQuestion(CurrentQuestion);
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
                if (answer.Contains(_questions[current]._answer))
                {
                    if (isFirstTry == true)
                    {
                        isFirstTry = false;
                        totalCorrect++;
                    }

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
        // check
        foreach (var bord in _border_refs) { bord.Visibility = Visibility.Collapsed; }

        if (current + 1 >= 10) //went passed the end
        {
            new ResultsWindow(totalCorrect).Show();
            Close();
            return;
        }

        CurrentQuestion = _questions[++current];
        SetQuestion(CurrentQuestion);
        InvalidateVisual();
    }
    
    //avi's addition
    private void PrevBtn_Click(object sender, RoutedEventArgs e)
    {
        if(current - 1 < 0)  //dont go further back than the 1st question
        {
            return;  //don't go to a negative index
        }

        else 
        {
            foreach (var bord in _border_refs) { bord.Visibility = Visibility.Collapsed; } //hide text
            CurrentQuestion = _questions[--current]; //go back 1 question
            SetQuestion(CurrentQuestion); //now show
            InvalidateVisual();
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
