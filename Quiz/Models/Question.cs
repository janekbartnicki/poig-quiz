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
        public string CorrectAnswer { get; set; }
        public List<string> Questions { get; set; }

        public bool IsAnswerCorrect(string answer)
        {
            return answer == CorrectAnswer;
        }
    }
}
