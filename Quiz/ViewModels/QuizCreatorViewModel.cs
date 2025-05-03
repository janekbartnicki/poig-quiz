using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Quiz.Commands;
using Quiz.Models;

namespace Quiz.ViewModels
{
    public class QuizCreatorViewModel : ViewModelBase
    {
        // Właściwości
        private string _quizTitle;
        public string QuizTitle
        {
            get { return _quizTitle; }
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
            get { return _questionText; }
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
            get { return _currentQuestionIndex; }
            set
            {
                if (_currentQuestionIndex != value)
                {
                    _currentQuestionIndex = value;
                    OnPropertyChanged(nameof(CurrentQuestionIndex));
                }
            }
        }

        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        // Komendy
        public ICommand NextQuestionCommand { get; set; }
        public ICommand FinishQuizCommand { get; set; }

        // Właściwości odpowiedzialne za widoczność paneli
        private bool _isTitlePanelVisible = true;
        public bool IsTitlePanelVisible
        {
            get { return _isTitlePanelVisible; }
            set
            {
                if (_isTitlePanelVisible != value)
                {
                    _isTitlePanelVisible = value;
                    OnPropertyChanged(nameof(IsTitlePanelVisible));
                }
            }
        }

        private bool _isQuestionPanelVisible = false;
        public bool IsQuestionPanelVisible
        {
            get { return _isQuestionPanelVisible; }
            set
            {
                if (_isQuestionPanelVisible != value)
                {
                    _isQuestionPanelVisible = value;
                    OnPropertyChanged(nameof(IsQuestionPanelVisible));
                }
            }
        }

        // Konstruktor
        public QuizCreatorViewModel()
        {
            NextQuestionCommand = new RelayCommand(_ => NextQuestion());
            FinishQuizCommand = new RelayCommand(_ => FinishQuiz());
        }

        // Metoda do przejścia do kolejnego pytania
        private void NextQuestion()
        {
            if (string.IsNullOrEmpty(QuizTitle))
            {
                // Komunikat o błędzie
                return;
            }

            // Dodanie nowego pytania
            var correctAnswersList = new List<string>();
            for (int i = 0; i < CorrectAnswers.Count; i++)
            {
                if (CorrectAnswers[i])
                {
                    correctAnswersList.Add(Answers[i]);
                }
            }

            var newQuestion = new Question
            {
                Text = QuestionText,
                Questions = new List<string>(Answers),
                CorrectAnswers = correctAnswersList
            };

            Questions.Add(newQuestion);

            // Resetowanie pól
            QuestionText = "";
            Answers = new ObservableCollection<string> { "", "", "", "" };
            CorrectAnswers = new ObservableCollection<bool> { false, false, false, false };

            // Zwiększenie indeksu pytania
            CurrentQuestionIndex++;

            // Zmiana widoczności panelu
            IsTitlePanelVisible = false;
            IsQuestionPanelVisible = true;
        }

        // Metoda do zakończenia tworzenia quizu
        private void FinishQuiz()
        {
            // Tu dodaj kod do zapisania quizu np. do pliku
        }
    }
}
