//using Quiz.Commands;
using Quiz.Models;
using Quiz.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quiz.ViewModels
{
    public class QuizViewModel : ViewModelBase
    {
        private readonly Models.Quiz _quiz;
        private readonly DispatcherTimer _questionTimer;
        private int _currentQuestionIndex;

        // Właściwości widoczności
        private bool _isQuizVisible;
        public bool IsQuizVisible
        {
            get { return _isQuizVisible; }
            set { SetProperty(ref _isQuizVisible, value); }
        }

        private bool _isSummaryVisible;
        public bool IsSummaryVisible
        {
            get { return _isSummaryVisible; }
            set { SetProperty(ref _isSummaryVisible, value); }
        }

        // Właściwości dla ekranu pytania
        private string _questionNumberText;
        public string QuestionNumberText
        {
            get { return _questionNumberText; }
            set { SetProperty(ref _questionNumberText, value); }
        }

        private string _questionText;
        public string QuestionText
        {
            get { return _questionText; }
            set { SetProperty(ref _questionText, value); }
        }

        private ObservableCollection<Answer> _currentAnswers;
        public ObservableCollection<Answer> CurrentAnswers
        {
            get { return _currentAnswers; }
            set { SetProperty(ref _currentAnswers, value); }
        }

        private int _secondsRemaining;
        public int SecondsRemaining
        {
            get { return _secondsRemaining; }
            set { SetProperty(ref _secondsRemaining, value); }
        }

        // Właściwości dla ekranu podsumowania
        private string _scoreText;
        public string ScoreText
        {
            get { return _scoreText; }
            set { SetProperty(ref _scoreText, value); }
        }

        private string _maxScoreText;
        public string MaxScoreText
        {
            get { return _maxScoreText; }
            set { SetProperty(ref _maxScoreText, value); }
        }

        private ObservableCollection<Question> _summaryQuestions;
        public ObservableCollection<Question> SummaryQuestions
        {
            get { return _summaryQuestions; }
            set { SetProperty(ref _summaryQuestions, value); }
        }

        // Komendy
        public ICommand NextQuestionCommand { get; }
        public ICommand BackToMainCommand { get; }

        // Zdarzenie informujące o zakończeniu quizu
        public event EventHandler QuizCompleted;

        public QuizViewModel(Models.Quiz quiz)
        {
            _quiz = quiz;
            _currentQuestionIndex = 0;

            // Inicjalizacja timera
            _questionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _questionTimer.Tick += QuestionTimer_Tick;

            // Inicjalizacja komend
            NextQuestionCommand = new RelayCommand(() => NextQuestion(null);
            // Update the RelayCommand initialization to match the expected Action signature
            NextQuestionCommand = new RelayCommand(() => NextQuestion(null));
            BackToMainCommand = new RelayCommand(() => BackToMain(null));
            BackToMainCommand = new RelayCommand(() => BackToMain(null);

            // Inicjalizacja kolekcji
            CurrentAnswers = new ObservableCollection<Answer>();
            SummaryQuestions = new ObservableCollection<Question>();
        }

        public void StartQuiz()
        {
            IsQuizVisible = true;
            IsSummaryVisible = false;
            _currentQuestionIndex = 0;

            // Resetowanie punktów
            foreach (var question in _quiz.Questions)
            {
                question.UserScore = 0;
            }

            _quiz.TotalScore = 0;

            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            if (_currentQuestionIndex < _quiz.Questions.Count)
            {
                // Pobierz aktualne pytanie
                Question currentQuestion = _quiz.Questions[_currentQuestionIndex];

                // Ustawienie właściwości
                QuestionNumberText = $"Pytanie {_currentQuestionIndex + 1}/{_quiz.Questions.Count}";
                QuestionText = currentQuestion.Text;

                // Ustawienie odpowiedzi
                foreach (var answer in currentQuestion.Answers)
                {
                    answer.IsSelected = false;
                }

                CurrentAnswers = new ObservableCollection<Answer>(currentQuestion.Answers);

                // Uruchomienie timera
                SecondsRemaining = 30;
                _questionTimer.Start();
            }
        }

        private void QuestionTimer_Tick(object sender, EventArgs e)
        {
            SecondsRemaining--;

            if (SecondsRemaining <= 0)
            {
                _questionTimer.Stop();
                ProcessAnswers();
                MoveToNextQuestion();
            }
        }

        private void ProcessAnswers()
        {
            Question currentQuestion = _quiz.Questions[_currentQuestionIndex];
            List<Answer> currentAnswers = currentQuestion.Answers;

            int correctAnswersCount = currentAnswers.Count(a => a.IsCorrect);
            int userCorrectAnswersCount = 0;
            int userWrongAnswersCount = 0;

            foreach (var answer in currentAnswers)
            {
                if (answer.IsCorrect && answer.IsSelected)
                {
                    userCorrectAnswersCount++;
                }
                if (!answer.IsCorrect && answer.IsSelected)
                {
                    userWrongAnswersCount++;
                }

                // Ustawienie statusu dla podsumowania
                if (answer.IsCorrect)
                {
                    answer.Status = "✓";
                    answer.StatusColor = Brushes.Green;
                }
                else if (answer.IsSelected)
                {
                    answer.Status = "✗";
                    answer.StatusColor = Brushes.Red;
                }
                else
                {
                    answer.Status = " ";
                    answer.StatusColor = Brushes.Black;
                }
            }

            // Obliczenie punktacji
            double pointsForQuestion = 0;

            if (userCorrectAnswersCount > 0 && userWrongAnswersCount == 0)
            {
                // Wszystkie zaznaczone odpowiedzi są poprawne
                pointsForQuestion = (double)userCorrectAnswersCount / correctAnswersCount;

                // Pełny punkt za wszystkie poprawne odpowiedzi
                if (userCorrectAnswersCount == correctAnswersCount)
                {
                    pointsForQuestion = 1.0;
                }
            }

            // Zapisanie punktacji
            currentQuestion.UserScore = pointsForQuestion;
            _quiz.TotalScore += pointsForQuestion;

            // Informacja o punktach dla podsumowania
            currentQuestion.PointsInfo = $"Zdobyte punkty: {pointsForQuestion:F1}";
            currentQuestion.PointsColor = pointsForQuestion >= 0.5 ? Brushes.Green : Brushes.Red;
        }

        private void NextQuestion(object parameter)
        {
            _questionTimer.Stop();
            ProcessAnswers();
            MoveToNextQuestion();
        }

        private void MoveToNextQuestion()
        {
            _currentQuestionIndex++;

            if (_currentQuestionIndex < _quiz.Questions.Count)
            {
                DisplayCurrentQuestion();
            }
            else
            {
                ShowSummary();
            }
        }

        private void ShowSummary()
        {
            IsQuizVisible = false;
            IsSummaryVisible = true;

            // Ustawienie wyniku
            ScoreText = _quiz.TotalScore.ToString("F1");
            MaxScoreText = $"/{_quiz.Questions.Count}";

            // Ustawienie podsumowania pytań
            SummaryQuestions = new ObservableCollection<Question>(_quiz.Questions);

            // Wywołanie zdarzenia zakończenia quizu
            QuizCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void BackToMain(object parameter)
        {
            _questionTimer.Stop();
            IsQuizVisible = false;
            IsSummaryVisible = false;
        }
    }
}