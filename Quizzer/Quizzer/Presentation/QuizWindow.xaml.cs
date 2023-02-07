using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using QuizApp.BL;

namespace QuizApp.Presentation;

public partial class QuizWindow : Window, INotifyPropertyChanged
{
    private static SoundPlayer correctSoundPlayer = new();
    private static SoundPlayer wrongSoundPlayer = new();
    private readonly string wrongSound = FileReader.GetFilePath("wrong_answer.wav", new() { ".wav" });
    private readonly string correctSound = FileReader.GetFilePath("right_answer.wav", new() { ".wav" });
    private bool _soundToggled;

    private readonly List<Question> _questions;
    private Question? _currentQuestion;
    public Question CurrentQuestion 
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            OnPropertyChanged();
        }
    }
    
    private readonly int _maxQuestions;
    private int _totalCorrect;
    private int _currentQuestionNum;

    // Timer logic
    private Clock _clock;
    private const int AllottedSec = 30;
    private int _remainingSeconds = AllottedSec;
    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set
        {
            _remainingSeconds = value;
            OnPropertyChanged();
            if (_remainingSeconds == 0)
                NextBtn_Click(this, null!);
        }
    }

    private readonly List<Border> _borders = new();
    // Green
    private readonly SolidColorBrush? _correctColor = new BrushConverter().ConvertFrom("#6632CD32") as SolidColorBrush;
    // Red
    private readonly SolidColorBrush? _wrongColor = new BrushConverter().ConvertFrom("#66DC143C") as SolidColorBrush;

    public QuizWindow(List<Question> questions, int maxQuestions)
    {
        _questions = questions;
        _maxQuestions = maxQuestions;
        correctSoundPlayer.SoundLocation = correctSound;
        wrongSoundPlayer.SoundLocation = wrongSound;
        CurrentQuestion = _questions[_currentQuestionNum];
        _clock = new Clock(AllottedSec, this);
        InitializeComponent();
        SetQuestion(CurrentQuestion);
        _clock.Start();
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
            var txt = new TextBlock { Text = ans, TextWrapping = TextWrapping.Wrap };
            var rb = new RadioButton { Content = txt, IsChecked = false, FontSize = 22, VerticalContentAlignment = VerticalAlignment.Center };
            var border = new Border();
            var isFirstTry = true;
            border.Child = rb;
            _borders.Add(border);

            rb.Checked += (_,_) =>
            {
                rb.FontWeight = FontWeights.Bold;
                var answer = ((TextBlock)rb.Content).Text;

                // answer is correct
                if (answer.Contains(_questions[_currentQuestionNum]._answer))
                {
                    if (isFirstTry)
                    {
                        isFirstTry = false;
                        _totalCorrect++;
                    }

                    if (_soundToggled)
                        correctSoundPlayer.Play();

                    _clock.Stop();
                    border.Background = _correctColor;
                    rb.BorderBrush = _correctColor;
                    rb.BorderThickness = new Thickness(2);
                }
                else // answer incorrect
                {
                    if (isFirstTry)
                        isFirstTry = false;

                    if (_soundToggled)
                        wrongSoundPlayer.Play();

                    border.Background = _wrongColor;
                    rb.BorderBrush = _wrongColor;
                    rb.BorderThickness = new Thickness(2);
                }
            };

            rb.Unchecked += (_,_) =>
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
        if (_clock.isRunning)
            _clock.Stop();

        _clock = new Clock(AllottedSec, this); //reset
        RemainingSeconds = AllottedSec;

        // if done
        if (_currentQuestionNum + 1 >= _maxQuestions)
        {
            new ResultsWindow(_totalCorrect, _questions.Count).Show();
            Close();
            return;
        }

        // hide the radio buttons 
        // TODO: needs to be reimplemented in a way that doesn't require the loop
        foreach (var border in _borders) { border.Visibility = Visibility.Collapsed; }

        CurrentQuestion = _questions[++_currentQuestionNum];
        SetQuestion(CurrentQuestion);
        _clock.Start();
    }

    // Avi's addition
    private void PrevBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_clock.isRunning)
            _clock.Stop();

        if (_currentQuestionNum - 1 < 0)
            return;

        foreach (var border in _borders) { border.Visibility = Visibility.Collapsed; }
        CurrentQuestion = _questions[--_currentQuestionNum]; //go back 1 question
        SetQuestion(CurrentQuestion);
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

    private void ToggleSound_Click(object sender, RoutedEventArgs e)
    {
        _soundToggled = (bool)((ToggleButton)sender).IsChecked;
    }
}
