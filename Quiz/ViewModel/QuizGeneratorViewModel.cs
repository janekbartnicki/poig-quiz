using System.ComponentModel;
using Newtonsoft.Json;
using Quiz.Model;
using QuizModel = Quiz.Model.Quiz;
using System.IO;

namespace Quiz.ViewModel
{
    class QuizGeneratorViewModel : INotifyPropertyChanged
    {
        private readonly string _pathToJSON = "../Data/quiz_db.json";

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

            if(SelectedQuiz != null)
            {
                SelectedQuiz.Questions.Add(question);
                _updateQuizQuestions(SelectedQuiz);
            }
        }

        private void _updateQuizQuestions(QuizModel quiz)
        {
            var json = File.ReadAllText(_pathToJSON);
            var quizList = JsonConvert.DeserializeObject<List<QuizModel>>(json);

            var foundQuiz = quizList?.Find(q => q.Title == quiz.Title);
            
            if(foundQuiz != null)
            {
                foundQuiz.Questions = quiz.Questions;
            }
            else
            {
                quizList.Add(quiz);
            }

            var updatedJSON = JsonConvert.SerializeObject(quizList, Formatting.Indented);
            File.WriteAllText(_pathToJSON, updatedJSON);
        }
    }
}
