using QuizApp.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuizApp.Presentation;

public partial class QuizWindow : INotifyPropertyChanged
{
    // Ui/Ux Related
    private readonly SolidColorBrush? _correctColorGreen = new BrushConverter().ConvertFrom("#6632CD32") as SolidColorBrush;
    private readonly SolidColorBrush? _wrongColorRed = new BrushConverter().ConvertFrom("#66DC143C") as SolidColorBrush;
    private readonly SolidColorBrush? _transparent = new BrushConverter().ConvertFrom("#00000000") as SolidColorBrush;
    private static readonly SoundPlayer CorrectSoundPlayer = new(FileReader.GetFilePath("right_answer.wav"));
    private static readonly SoundPlayer WrongSoundPlayer = new(FileReader.GetFilePath("wrong_answer.wav"));
    private static readonly SoundPlayer LowScorePlayer = new(FileReader.GetFilePath("scoreunder10.wav"));
    private static readonly SoundPlayer MediumScorePlayer = new(FileReader.GetFilePath("scorebetween10and23.wav"));
    private static readonly SoundPlayer PassedScorePlayer = new(FileReader.GetFilePath("passedscoreabove24.wav"));
    private bool _soundToggled;
    private readonly Thickness _highlightThickness = new(2);

    // Backend Related
    private readonly List<Question> _questions;

    private Question _currentQuestion;
    public Question CurrentQuestion
    {
        get => _currentQuestion;
        private set
        {
            _currentQuestion = value;
            OnPropertyChanged();
        }
    }

    public int MaxQuestions { get; }
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
    private readonly bool _isTimed;
    public Clock Clock { get; }
    private const int DefaultAllottedSec = 30;


    public QuizWindow(List<Question> questions, int maxQuestions, bool isTimed)
    {
        _questions = questions;
        MaxQuestions = maxQuestions;
        CurrentQuestion = _questions[CurrentQuestionNum];

        _isTimed = isTimed;

        if (_isTimed)
            Clock = new Clock();

        InitializeComponent();
        ConfigureQuestion(CurrentQuestion);
    }

    private void SetClock(Question question)
    {
        if (question.QuestionText.Count() > 130)
        {
            Clock.SetAllotted(40);

            if (question.PossibleAnswers.Count > 2)
            {
                Clock.SetAllotted(50);
            }
        }
        else Clock.SetAllotted(DefaultAllottedSec);

        if (question.PossibleAnswers.Count == 1)
        {
            Clock.SetAllotted(80);
        }
    }

    private void ConfigureQuestion(Question question)
    {
        if (_isTimed)
        {
            SetClock(question);
            Clock.Start();
        }

        var correctAns = _questions[CurrentQuestionNum].CorrectAnswer;

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
            var answerTextBlock = new TextBlock
            {
                Text = possibleAnswer,
                TextWrapping = TextWrapping.Wrap
            };

            var answerRadioButton = new RadioButton
            {
                Content = answerTextBlock,
                Margin = new Thickness(0, 3, 0, 0),
                FontSize = 26,
                VerticalContentAlignment = VerticalAlignment.Center,
                GroupName = question.GetHashCode().ToString()
            };

            var radioButtonDecoration = new Border
            {
                Child = answerRadioButton
            };

            answerRadioButton.Checked += (_, _) =>
            {
                answerRadioButton.FontWeight = FontWeights.Bold;

                // answer is correct
                if (possibleAnswer.Contains(correctAns))
                {
                    // first row will be 
                    if (question.IsFirstTry)
                    {
                        question.IsFirstTry = false;
                        question.AnsweredCorrectly = 1;
                    }

                    if (_soundToggled)
                        CorrectSoundPlayer.Play();

                    if (Clock is { IsRunning: true })
                        Clock.Pause();

                    radioButtonDecoration.Background = _correctColorGreen;
                    answerRadioButton.BorderBrush = _correctColorGreen;
                    answerRadioButton.BorderThickness = _highlightThickness;
                }
                // answer incorrect
                else
                {
                    question.IsFirstTry = false;

                    if (_soundToggled)
                        WrongSoundPlayer.Play();

                    radioButtonDecoration.Background = _wrongColorRed;
                    answerRadioButton.BorderBrush = _wrongColorRed;
                    answerRadioButton.BorderThickness = _highlightThickness;
                }
            };

            answerRadioButton.Unchecked += (_, _) =>
            {
                answerRadioButton.FontWeight = FontWeights.Normal;
                radioButtonDecoration.Background = _transparent;
                radioButtonDecoration.BorderBrush = _transparent;
            };

            // add to stack-panel
            RadioStackPanel.Children.Add(radioButtonDecoration);
        }
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
        if (Clock is { IsRunning: true })
        {
            Clock.Pause();
            Clock.Rewind();
        }

        // if done
        if (CurrentQuestionNum + 1 >= MaxQuestions)
        {
            var totalCorrect = _questions.Sum(q => q.AnsweredCorrectly);

            if (_soundToggled && MaxQuestions == 35)
            {
                if (totalCorrect < 10)
                    LowScorePlayer.Play();
                else if (totalCorrect < 24)
                    MediumScorePlayer.Play();
                else 
                    PassedScorePlayer.Play();
            }
            new ResultsWindow(totalCorrect, _questions.Count).Show();
            Close();
            return;
        }

        RadioStackPanel.Children.Clear();

        CurrentQuestion = _questions[++CurrentQuestionNum];
        ConfigureQuestion(CurrentQuestion);

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

        if (Clock is { IsRunning: true })
        {
            Clock.Pause();
            Clock.Rewind();
        }

        RadioStackPanel.Children.Clear();
        CurrentQuestion = _questions[--CurrentQuestionNum]; //go back 1 question
        ConfigureQuestion(CurrentQuestion);

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
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}