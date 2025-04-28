using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Model
{
    class Question
    {
        //public int Id { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public List<string> Options { get; set; }
        public Question(string text, string answer, List<string> options)
        {
            //Id = id;
            Text = text;
            Answer = answer;
            Options = options;
        }
    }
}
