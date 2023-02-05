using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quizzer;

public partial class QuizPage : Page, INotifyPropertyChanged
{
    public List<Question> _questions = new List<Question>();
    private List<RadioButton> _rb_refs = new List<RadioButton>();
    private List<Border> _border_refs = new List<Border>();
    private Question _currentQuestion;
    public Question CurrentQuestion
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            OnPropertyChanged();
        }
    }

    int current = 0;

    public QuizPage(List<Question> questions)
    {
        _questions = questions;
        CurrentQuestion = _questions[current];
        InitializeComponent();
        SetQuestion(CurrentQuestion);
    }

    public void SetQuestion(Question question)
    {
        foreach (var ans in question._possibleAnswers)
        {
            var rb = new RadioButton() { Content = ans, IsChecked = false };
            var border = new Border();
            border.Child = rb;
            _rb_refs.Add(rb);
            _border_refs.Add(border);
            rb.Checked += (sender, args) =>
            {
                Console.WriteLine("Pressed " + (sender as RadioButton).Tag);
                rb.FontWeight = FontWeights.Bold;
                var answer = (string)rb.Content;
                if (answer.Contains(_questions[current]._answer))
                {
                    border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF32CD32");
                    rb.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF32CD32");
                    rb.BorderThickness = new Thickness(2);
                }
                else
                {
                    border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDC143C");
                    rb.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDC143C");
                    rb.BorderThickness = new Thickness(2);
                }
            }; 

            rb.Unchecked += (sender, args) => {
                rb.FontWeight = FontWeights.Normal;
                border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff00");
                rb.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#222222"); 
                rb.BorderThickness = new Thickness(1); };

            RadioStackPanel.Children.Add(border);
        }
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
        foreach (var rb in _rb_refs) { rb.Visibility = Visibility.Collapsed; }
        foreach (var bord in _border_refs) { bord.Visibility = Visibility.Collapsed; }

        if (current + 1 >= 10)
            return;

        CurrentQuestion = _questions[++current];
        SetQuestion(CurrentQuestion);
        InvalidateVisual();
    }

    private void MainBtn_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
