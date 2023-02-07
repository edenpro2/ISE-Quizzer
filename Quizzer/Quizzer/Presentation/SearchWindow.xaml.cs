using BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Presentation;

public partial class SearchWindow : Window, INotifyPropertyChanged
{
    private BackgroundWorker Worker;
    public List<Question> _questions;

    private List<Question> _possibleQuestions;
    public List<Question> PossibleQuestions
    {
        get => _possibleQuestions;
        set
        {
            _possibleQuestions = value;
            OnPropertyChanged();
        }
    }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
    }

    private void searchChangedEventHandler(object sender, TextChangedEventArgs args)
    {
        SearchText = (sender as TextBox).Text;
        StartSearch();
    }

    private void StartSearch()
    {
        Worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        
        Worker.DoWork += Worker_DoWork!;
        Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        Worker.RunWorkerAsync();
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        PossibleQuestions = Search(SearchText);
        Dispatcher.Invoke(() => { /* main ui */; });
        Worker_RunWorkerCompleted(sender, new RunWorkerCompletedEventArgs(sender, null, true));
    }

    private void Worker_RunWorkerCompleted(object? o, RunWorkerCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            throw new Exception("Fatal Error - stopping thread");
        }

        Worker?.CancelAsync();
    }

    private List<Question>? Search(string searchText)
    {
        return _questions.Where(que => que._question.ToLower().Contains(searchText.ToLower())).ToList();
    }

    public SearchWindow(List<Quiz> quizzes)
    {
        InitializeComponent();
        _questions = quizzes.SelectMany(q => q.Questions).ToList();
    }

    #region INotify
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

}
