using System;
using System.Collections.Generic;

namespace QuizApp.BL;

public class Question : AbstractViewModel
{
    private string _questionText = string.Empty;
    public string QuestionText
    {
        get => _questionText;
        set
        {
            _questionText = value;
            OnPropertyChanged();
        }
    }

    private List<string>? _possibleAnswers;
    public List<string>? PossibleAnswers
    {
        get => _possibleAnswers;
        set
        {
            _possibleAnswers = value;
            OnPropertyChanged();
        }
    }
    private string _correctAnswer = string.Empty;
    public string CorrectAnswer
    {
        get => _correctAnswer;
        set
        {
            _correctAnswer = value;
            OnPropertyChanged();
        }
    }

    public override int GetHashCode()
    {
        return _questionText.GetHashCode() + _correctAnswer.GetHashCode();
    }

    public Question(string questionText = "", List<string>? possibleAnswers = null, string correctAnswer = "")
    {
        QuestionText = questionText;
        PossibleAnswers = possibleAnswers;
        CorrectAnswer = correctAnswer;
    }

    public override string ToString()
    {
        return "Question: " + QuestionText + '\n' + " " +
               "Corr: " + CorrectAnswer;
    }
}