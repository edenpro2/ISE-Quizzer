namespace Quizzer;
using System.Collections.Generic;
using System.Linq;

public static class Database
{
    public static List<Quiz> loadQuizzes()
    {
        var quizzes = new List<Quiz>();

        for (var quiz_num = 1; quiz_num <= 12; quiz_num++)
        {
            var text = (List<string>)FileReader.LoadTxt($"/Quizzes/q_{quiz_num}.txt");
            if (text == default)
                continue;

            var joined = string.Join("", text.Select(o => o.ToString()).ToArray());
            var blocks = joined.Split('$').ToArray();
            var q = new List<string>();
            var question_text = "";
            var possible_ans = new List<string>();
            var answer = "";
            var trimmed = "";
            var quiz = new Quiz(quiz_num);
            Question question;

            // questions inside each quiz
            foreach (var block in blocks)
            {
                var index = block.IndexOf('~');
                // if open question 
                if (index != -1)
                {
                    question_text = new string(block.Take(index).ToArray()).Trim();
                    trimmed = block.Remove(0, index);
                    possible_ans = new List<string>(trimmed.Remove(0, 1).Split('~'));
                    answer = possible_ans[0];
                    question = new(question_text, possible_ans, answer);
                    quiz.AddQuestion(question);
                    continue;
                }

                // find answer symbol
                index = block.IndexOf('@');
                // question will be 
                question_text = new string(block.Take(index).ToArray()).Trim();

                if (question_text.Count() < 1)
                    continue;

                // remove text until first '@' 
                trimmed = block.Remove(0, index);
                possible_ans = new List<string>(trimmed.Remove(0, 1).Split('@'));
                possible_ans.ForEach(s => s.Trim());
                var possible_ans_edited = new List<string>();

                foreach (var ans in possible_ans)
                {
                    possible_ans_edited.Add(ans.Trim().Replace("*", " "));
                    if (ans.Contains('*'))
                        answer = ans.Replace("*", " ").Trim();
                }

                question = new(question_text, possible_ans_edited, answer);
                quiz.AddQuestion(question);
            }

            quizzes.Add(quiz);
        }

        return quizzes;
    }
}
