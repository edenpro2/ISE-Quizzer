using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizApp.BL;

public class Quiz : IEnumerable<Question>, INotifyPropertyChanged
{
    public int Quiz_Num { get; set; } = -1;
    public List<Question> Questions;

    public Quiz(int Quiz_Num)
    {
        Questions = new List<Question>();
    }

    public void AddQuestion(Question question)
    {
        Questions.Add(question);
    }

    public IEnumerator<Question> GetEnumerator()
    {
        return Questions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
