using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Quizzer;

public partial class ResultsPage : Page, INotifyPropertyChanged
{
    private int _totalCorrect;
    public int totalCorrect 
    { 
        get => _totalCorrect;
        set
        {
            _totalCorrect = value;
            OnPropertyChanged();
        }
    }

    public ResultsPage(int total_correct)
    {
        totalCorrect = total_correct;
        InitializeComponent();
    }

    private void MainBtn_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    #region INotify
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
