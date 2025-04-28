using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.Model;
using QuizModel = Quiz.Model.Quiz;

namespace Quiz.ViewModel
{
    class QuizGeneratorViewModel : INotifyPropertyChanged
    {
        private QuizModel _selectedQuiz;

        public QuizModel SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged(nameof(SelectedQuiz));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public QuizGeneratorViewModel()
        {
        }

        public void CreateNewQuiz(string quizName, List<Question>? questions = null)
        {
            var quiz = new QuizModel
            {
                Title = quizName,
                Questions = questions ?? new List<Question>()
            };

            SelectedQuiz = quiz;
        }

        public void AddQuestion(string questionText, string answer, List<string> options)
        {
            var question = new Question(questionText, answer, options);

            SelectedQuiz?.Questions.Add(question);
        }
    }
}
