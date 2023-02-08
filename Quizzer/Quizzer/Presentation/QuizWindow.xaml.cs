using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuizApp.BL;

namespace QuizApp.Presentation;

public partial class QuizWindow : Window, INotifyPropertyChanged
{
    // Ui/Ux Related
    private readonly SolidColorBrush? _correctColorGreen = new BrushConverter().ConvertFrom("#6632CD32") as SolidColorBrush;
    private readonly SolidColorBrush? _wrongColorRed = new BrushConverter().ConvertFrom("#66DC143C") as SolidColorBrush;
    private readonly SolidColorBrush? _transparent = new BrushConverter().ConvertFrom("#00000000") as SolidColorBrush;
    private static readonly SoundPlayer CorrectSoundPlayer = new(FileReader.GetFilePath("right_answer.wav"));
    private static readonly SoundPlayer WrongSoundPlayer = new(FileReader.GetFilePath("wrong_answer.wav"));
    private bool _soundToggled;
    public string CorrectAns { get; set; } = "";
    private readonly Thickness _highlightThickness = new(2);

    // Backend Related
    private readonly List<Border> _borders = new();
    private readonly List<Question> _questions;

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

    public int MaxQuestions { get; }
    private int _totalCorrect;
    private int _currentQuestionNum;
    public int CurrentQuestionNum
    {
        get => _currentQuestionNum;
        set
        {
            _currentQuestionNum = value;
            OnPropertyChanged();
        }
    }

    // Timer logic
    private bool _isTimed;
    private Clock _clock;
    public Clock Clock
    {
        get => _clock;
        set
        {
            _clock = value;
            OnPropertyChanged();
        }
    }

    private bool[,] correctAnswers;

    const int firstTimeRow = 0;
    const int questionRow = 1;

    private const int DefaultAllottedSec = 30;

    public QuizWindow(List<Question> questions, int maxQuestions, bool isTimed)
    {
        _questions = questions;
        MaxQuestions = maxQuestions;
        CurrentQuestion = _questions[CurrentQuestionNum];
        correctAnswers = new bool[2, MaxQuestions];

        for (var i = 0; i < correctAnswers.GetLength(0); i++)
        {
            correctAnswers[firstTimeRow, i] = true;
        }

        _isTimed = isTimed;
        if (_isTimed)
            _clock = new Clock();
        InitializeComponent();
        SetQuestion(CurrentQuestion);
    }

    private void SetClock(Question question)
    {
        if (question.QuestionText.Count() > 130)
        {
            _clock.SetAllotted(40);

            if (question.PossibleAnswers.Count > 2)
            {
                _clock.SetAllotted(50);
            }
        }
        else _clock.SetAllotted(DefaultAllottedSec);

        if (question.PossibleAnswers.Count == 1)
        {
            _clock.SetAllotted(80);
        }
    }

    public void SetQuestion(Question? question)
    {
        if (question == null)
            return;

        if (_isTimed)
        {
            SetClock(question);
            Clock.Start();
        }

        CorrectAns = _questions[CurrentQuestionNum].CorrectAnswer;
        
        // reorder multiple choice questions
        if (question.PossibleAnswers.Count > 2)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            question.PossibleAnswers = question.PossibleAnswers.OrderBy(_ => rand.Next()).ToList();
        }

        // Need to dynamically add RadioButtons. Every RB will be a child of Border so that UI
        // can highlight it when right or wrong answer
        foreach (var possibleAnswer in question.PossibleAnswers)
        {
            var txt = new TextBlock
            {
                Text = possibleAnswer,
                TextWrapping = TextWrapping.Wrap
            };

            var rb = new RadioButton
            {
                Content = txt,
                FontSize = 26,
                VerticalContentAlignment = VerticalAlignment.Center,
                GroupName = question.GetHashCode().ToString()
            };

            var border = new Border
            {
                Child = rb
            };
            _borders.Add(border);

            var isFirstTry = true;

            rb.Checked += (_, _) =>
            {
                rb.FontWeight = FontWeights.Bold;

                // answer is correct
                if (possibleAnswer.Contains(CorrectAns))
                {
                    // first row will be 
                    if (correctAnswers[firstTimeRow, CurrentQuestionNum])
                    {
                        correctAnswers[firstTimeRow, CurrentQuestionNum] = false;
                        correctAnswers[questionRow, CurrentQuestionNum] = true;
                    }

                    if (_soundToggled)
                        CorrectSoundPlayer.Play();

                    if (_clock is { IsRunning: true})
                        _clock.Pause();

                    border.Background = _correctColorGreen;
                    rb.BorderBrush = _correctColorGreen;
                    rb.BorderThickness = _highlightThickness;
                }
                // answer incorrect
                else
                {
                    correctAnswers[firstTimeRow, CurrentQuestionNum] = false;

                    if (_soundToggled)
                        WrongSoundPlayer.Play();

                    border.Background = _wrongColorRed;
                    rb.BorderBrush = _wrongColorRed;
                    rb.BorderThickness = _highlightThickness;
                }
            };

            rb.Unchecked += (_, _) =>
            {
                rb.FontWeight = FontWeights.Normal;
                border.Background = _transparent;
                border.BorderBrush = _transparent;
            };

            // add to stack-panel
            RadioStackPanel.Children.Add(border);
        }
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_clock is { IsRunning: true })
        {
            _clock.Pause();
            _clock.Rewind();
        }

        // if done
        if (CurrentQuestionNum + 1 >= MaxQuestions)
        {
            var totalCorrect = 0;
            for(var i = 0; i < MaxQuestions; i++)
            {
                if (correctAnswers[questionRow, i])
                    totalCorrect++;
            }
           
            new ResultsWindow(totalCorrect, _questions.Count).Show();
            Close();
            return;
        }

        foreach (var border in _borders) border.Visibility = Visibility.Collapsed;

        CurrentQuestion = _questions[++CurrentQuestionNum];
        SetQuestion(CurrentQuestion);

        if (_isTimed)
        {
            Clock.Start();
        }
    }

    // Avi's addition
    private void PrevBtn_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentQuestionNum - 1 < 0)
            return;

        if (Clock is { IsRunning:true})
        {
            _clock.Pause();
            _clock.Rewind();
        }

        foreach (var border in _borders) border.Visibility = Visibility.Collapsed;
        CurrentQuestion = _questions[--CurrentQuestionNum]; //go back 1 question
        SetQuestion(CurrentQuestion);

        if (_isTimed)
        {
            Clock.Start();
        }
    }

    private void MainBtn_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    private void ToggleSound_Click(object sender, RoutedEventArgs e)
    {
        _soundToggled = (bool)SoundToggleBtn.IsChecked;
    }
    
    #region INotify

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}