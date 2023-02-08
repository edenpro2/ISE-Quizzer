using System.Collections;
using System.Collections.Generic;

namespace QuizApp.BL
{
    public class Quiz : IEnumerable<Question>
    {
        public List<Question> Questions { get; set; }

        public Quiz()
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
    }
}
