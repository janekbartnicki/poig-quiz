using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class Quiz
    {
        public string Title { get; set; }
        public List<Question> questions { get; set; }
    }
}
