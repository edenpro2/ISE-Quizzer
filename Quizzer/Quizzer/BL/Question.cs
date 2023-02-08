using System.Collections.Generic;
using System.Linq;

namespace QuizApp.BL
{
    public class Question : AbstractViewModel
    {
        private readonly string _questionText = string.Empty;
        public string QuestionText
        {
            get => _questionText;
            private init
            {
                _questionText = value;
                OnPropertyChanged();
            }
        }

        private List<string> _possibleAnswers = new();
        public List<string> PossibleAnswers
        {
            get => _possibleAnswers;
            set
            {
                _possibleAnswers = value;
                OnPropertyChanged();
            }
        }
        private readonly string _correctAnswer = string.Empty;
        public string CorrectAnswer
        {
            get => _correctAnswer;
            private init
            {
                _correctAnswer = value;
                OnPropertyChanged();
            }
        }

        public int QuizNum { get; }

        public int AnsweredCorrectly { get; set; } 
        public bool IsFirstTry { get; set; } = true;

        public Question(string questionText, IEnumerable<string> possibleAnswers, string correctAnswer, int quizNum)
        {
            QuestionText = questionText;
            CorrectAnswer = correctAnswer;
            PossibleAnswers = possibleAnswers.ToList();
            QuizNum = quizNum;
        }

        public override string ToString()
        {
            return "Question: " + QuestionText + '\n' + " " +
                   "Corr: " + CorrectAnswer;
        }
    }
}