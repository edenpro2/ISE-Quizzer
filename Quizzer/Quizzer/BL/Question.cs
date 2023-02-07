using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizApp.BL;

public class Question : INotifyPropertyChanged
{
    public string _question { get; set; }
    public List<string> _possibleAnswers { get; set; }
    public string _answer { get; set; }
    int identifier = -1;

    public Question(string question, List<string> poss_ans, string answer)
    {
        _question = question;
        _possibleAnswers = poss_ans;
        _answer = answer;

        identifier = _question.GetHashCode() % _possibleAnswers.GetHashCode() % _answer.GetHashCode();
    }

    public override string ToString()
    {
        return _question + '\n' +
            string.Join(" ", _possibleAnswers.ToArray()) + '\n' +
            _answer;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}