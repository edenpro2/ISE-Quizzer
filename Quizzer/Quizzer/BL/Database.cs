using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;

namespace QuizApp.BL;

public static class Database
{
    public static List<Quiz> LoadQuizzes()
    {
        var quizzes = new List<Quiz>();
        string questionText;
        Question question;
        string trimmed;
        List<string> possibleAns;

        for (var quizNum = 1; quizNum <= 12; quizNum++)
        {
            var text = (List<string>)FileReader.LoadTxt($"/Quizzes/q_{quizNum}.txt");
            var joined = string.Join("", text.Select(o => o.ToString()).ToArray());
            var blocks = joined.Split('$').ToArray();

            var answer = "";
            var quiz = new Quiz();

            // questions inside each quiz
            foreach (var block in blocks)
            {
                var index = block.IndexOf('~');
                // if open question 
                if (index != -1)
                {
                    questionText = new string(block.Take(index).ToArray()).Trim();
                    trimmed = block.Remove(0, index);
                    possibleAns = new List<string>(trimmed.Remove(0, 1).Split('~'));
                    answer = possibleAns[0];
                    question = new Question(questionText, possibleAns, answer);
                    quiz.AddQuestion(question);
                    continue;
                }

                // find answer symbol
                index = block.IndexOf('@');
                // question will be 
                questionText = new string(block.Take(index).ToArray()).Trim();

                if (!questionText.Any())
                    continue;

                // remove text until first '@' 
                trimmed = block.Remove(0, index);
                possibleAns = new List<string>(trimmed.Remove(0, 1).Split('@'));
                possibleAns = possibleAns.Select(s => s.Trim()).ToList();
                var possibleAnsEdited = new List<string>();

                foreach (var ans in possibleAns)
                {
                    possibleAnsEdited.Add(ans.Trim().Replace("*", " "));
                    if (ans.Contains('*'))
                        answer = ans.Replace("*", " ").Trim();
                }

                question = new Question(questionText, possibleAnsEdited, answer);
                quiz.AddQuestion(question);
            }

            quizzes.Add(quiz);
        }

        return quizzes;
    }
}
