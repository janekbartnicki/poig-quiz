using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quiz.Commands;
using Quiz.Models;
using Quiz.Repositories;
using Quiz.Stores;

namespace Quiz.ViewModels
{
    public class QuizEditorViewModel : ViewModelBase
    {
        public ICommand SaveCommand { get; }
        public ICommand NavigateQuizGeneratorCommand { get; }
        public ICommand AddQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }

        private readonly NavigationStore _navigationStore;
        private readonly QuizRepository _quizRepository;

        private Quiz.Models.Quiz _originalQuiz;

        private string _quizTitle;
        public string QuizTitle
        {
            get => _quizTitle;
            set
            {
                if (_quizTitle != value)
                {
                    _quizTitle = value;
                    OnPropertyChanged(nameof(QuizTitle));
                }
            }
        }

        private string _questionText;
        public string QuestionText
        {
            get => _questionText;
            set
            {
                if (_questionText != value)
                {
                    _questionText = value;
                    OnPropertyChanged(nameof(QuestionText));
                }
            }
        }

        public ObservableCollection<string> Answers { get; set; } = new ObservableCollection<string> { "", "", "", "" };
        public ObservableCollection<bool> CorrectAnswers { get; set; } = new ObservableCollection<bool> { false, false, false, false };

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                if (_currentQuestionIndex != value && value >= 0 && value < Questions.Count)
                {
                    _currentQuestionIndex = value;
                    OnPropertyChanged(nameof(CurrentQuestionIndex));
                    LoadQuestionData();
                }
            }
        }

        public ObservableCollection<Question> Questions { get; set; }

        public QuizEditorViewModel(NavigationStore navigationStore, Quiz.Models.Quiz quizToEdit)
        {
            _navigationStore = navigationStore;
            _quizRepository = new QuizRepository();
            _originalQuiz = quizToEdit;

            NavigateQuizGeneratorCommand = new NavigateQuizGeneratorCommand(navigationStore);
            SaveCommand = new RelayCommand(_ => SaveQuiz());

            AddQuestionCommand = new RelayCommand(_ => AddNewQuestion());
            DeleteQuestionCommand = new RelayCommand(_ => DeleteCurrentQuestion(), _ => Questions.Count > 0);

            QuizTitle = quizToEdit.Title;
            Questions = new ObservableCollection<Question>(quizToEdit.Questions);

            if (Questions.Count > 0)
            {
                CurrentQuestionIndex = 0;
                LoadCurrentQuestion();
            }
        }

        private void LoadQuestionData()
        {
            var selectedQuestion = Questions[CurrentQuestionIndex];

            QuestionText = selectedQuestion.Text;
            Answers = new ObservableCollection<string>(selectedQuestion.Questions);
            CorrectAnswers = new ObservableCollection<bool>(
                selectedQuestion.Questions.Select(q => selectedQuestion.CorrectAnswers.Contains(q))
            );

            OnPropertyChanged(nameof(Answers));
            OnPropertyChanged(nameof(CorrectAnswers));
        }


        private void LoadCurrentQuestion()
        {
            var question = Questions[CurrentQuestionIndex];

            QuestionText = question.Text;
            Answers = new ObservableCollection<string>(question.Questions);
            CorrectAnswers = new ObservableCollection<bool>(
                question.Questions.Select(a => question.CorrectAnswers.Contains(a))
            );

            OnPropertyChanged(nameof(Answers));
            OnPropertyChanged(nameof(CorrectAnswers));
        }

        private void SaveCurrentQuestionToList()
        {
            if (CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Questions.Count)
            {
                var updatedCorrectAnswers = new List<string>();
                for (int i = 0; i < CorrectAnswers.Count; i++)
                {
                    if (CorrectAnswers[i])
                        updatedCorrectAnswers.Add(Answers[i]);
                }

                Questions[CurrentQuestionIndex] = new Question
                {
                    Text = QuestionText,
                    Questions = new List<string>(Answers),
                    CorrectAnswers = updatedCorrectAnswers
                };
            }
        }

        private void SaveQuiz()
        {
            SaveCurrentQuestionToList();

            if (string.IsNullOrEmpty(QuizTitle) || Questions.Count == 0)
            {
                MessageBox.Show("Quiz musi mieć tytuł i co najmniej jedno pytanie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var updatedQuiz = new Quiz.Models.Quiz(QuizTitle, Questions.ToList());
                _quizRepository.AddOrUpdate(updatedQuiz);

                MessageBox.Show("Quiz został zapisany pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigateQuizGeneratorCommand.Execute(null);
            }
            catch
            {
                MessageBox.Show("Wystąpił błąd przy zapisie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewQuestion()
        {
            var newQuestion = new Question
            {
                Text = "Nowe pytanie",
                Questions = new List<string> { "", "", "", "" },
                CorrectAnswers = new List<string>()
            };

            Questions.Add(newQuestion);
            CurrentQuestionIndex = Questions.Count - 1;
            LoadCurrentQuestion();
        }

        private void DeleteCurrentQuestion()
        {
            if (CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Questions.Count)
            {
                Questions.RemoveAt(CurrentQuestionIndex);
                if (Questions.Count > 0)
                {
                    CurrentQuestionIndex = Math.Max(0, CurrentQuestionIndex - 1);
                    LoadCurrentQuestion();
                }
                else
                {
                    QuestionText = "";
                    Answers = new ObservableCollection<string> { "", "", "", "" };
                    CorrectAnswers = new ObservableCollection<bool> { false, false, false, false };
                    OnPropertyChanged(nameof(Answers));
                    OnPropertyChanged(nameof(CorrectAnswers));
                }
            }
        }
    }
}
