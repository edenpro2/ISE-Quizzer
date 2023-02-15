﻿using QuizApp.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace QuizApp.Presentation
{
    public partial class SearchWindow : INotifyPropertyChanged
    {
        private BackgroundWorker _worker;
        private readonly List<Question> _questions;

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
            _worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _worker.DoWork += Worker_DoWork!;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.RunWorkerAsync();
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

            _worker.CancelAsync();
        }

        //the condition met to show in the results (either text matches question or answer
        private bool Found(Question que, string searchText)
        {

            return que.QuestionText.ToLower().Contains(searchText) || que.CorrectAnswer.ToLower().Contains(searchText);
        }

        private List<Question>? Search(string searchText)
        {
            return _questions.Where(que => Found(que, searchText.ToLower().Trim())).ToList();
        }

        public SearchWindow(IEnumerable<Quiz> quizzes)
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
}
