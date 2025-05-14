using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class Question
    {
        public string Text { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public List<string> Questions { get; set; }

        public bool IsAnswerCorrect(List<string> answers)
        {
            if (answers == null || CorrectAnswers == null)
                return false;

            return answers.Count == CorrectAnswers.Count &&
                   !answers.Except(CorrectAnswers).Any() &&
                   !CorrectAnswers.Except(answers).Any();
        }
    }
}