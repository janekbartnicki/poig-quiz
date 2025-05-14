using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Quiz.Commands;
using Quiz.Models;
using Quiz.ViewModels;

namespace Quiz.ViewModels
{
    public class QuizViewModel : ViewModelBase
    {
        private Models.Quiz _currentQuiz;
        private int _currentQuestionIndex;
        private ObservableCollection<bool> _selectedAnswers;
        private List<List<bool>> _allUserAnswers = new List<List<bool>>();
        private DispatcherTimer _timer;
        private int _timeLeft = 30;
        private bool _isQuizLoaded = false;
        private bool _isQuizFinished = false;
        private bool _isTimerRunning = false;
        private int _correctAnswersCount = 0;

        // Properties for binding
        public Models.Quiz CurrentQuiz
        {
            get { return _currentQuiz; }
            set
            {
                if (_currentQuiz != value)
                {
                    _currentQuiz = value;
                    OnPropertyChanged(nameof(CurrentQuiz));
                    OnPropertyChanged(nameof(TotalQuestions));
                }
            }
        }

        public Question CurrentQuestion
        {
            get
            {
                return _currentQuestionIndex < CurrentQuiz?.Questions.Count ?
                CurrentQuiz.Questions[_currentQuestionIndex] : null;
            }
        }

        public int CurrentQuestionIndex
        {
            get { return _currentQuestionIndex; }
            set
            {
                if (_currentQuestionIndex != value)
                {
                    _currentQuestionIndex = value;
                    OnPropertyChanged(nameof(CurrentQuestionIndex));
                    OnPropertyChanged(nameof(CurrentQuestion));
                    OnPropertyChanged(nameof(QuestionNumber));
                }
            }
        }

        public int QuestionNumber
        {
            get { return _currentQuestionIndex + 1; }
        }


        public int TotalQuestions => CurrentQuiz?.Questions.Count ?? 0;

        public ObservableCollection<bool> SelectedAnswers
        {
            get { return _selectedAnswers; }
            set
            {
                _selectedAnswers = value;
                OnPropertyChanged(nameof(SelectedAnswers));
            }
        }

        public int TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                if (_timeLeft != value)
                {
                    _timeLeft = value;
                    OnPropertyChanged(nameof(TimeLeft));
                }
            }
        }

        public bool IsQuizLoaded
        {
            get { return _isQuizLoaded; }
            set
            {
                if (_isQuizLoaded != value)
                {
                    _isQuizLoaded = value;
                    OnPropertyChanged(nameof(IsQuizLoaded));
                }
            }
        }

        public bool IsQuizFinished
        {
            get { return _isQuizFinished; }
            set
            {
                if (_isQuizFinished != value)
                {
                    _isQuizFinished = value;
                    OnPropertyChanged(nameof(IsQuizFinished));
                }
            }
        }

        public int CorrectAnswersCount
        {
            get { return _correctAnswersCount; }
            set
            {
                if (_correctAnswersCount != value)
                {
                    _correctAnswersCount = value;
                    OnPropertyChanged(nameof(CorrectAnswersCount));
                    OnPropertyChanged(nameof(ScorePercentage));
                }
            }
        }

        public int ScorePercentage => TotalQuestions > 0 ? (CorrectAnswersCount * 100) / TotalQuestions : 0;

        public ObservableCollection<QuestionResultViewModel> QuizResults { get; } = new ObservableCollection<QuestionResultViewModel>();

        // Commands
        public ICommand LoadQuizCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand RestartQuizCommand { get; }

        public QuizViewModel()
        {
            LoadQuizCommand = new RelayCommand(_ => LoadQuiz());
            NextQuestionCommand = new RelayCommand(_ => NextQuestion(), _ => CanGoToNextQuestion());
            RestartQuizCommand = new RelayCommand(_ => RestartQuiz());

            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;

            if (TimeLeft <= 0)
            {
                StopTimer();
                // Auto-submit the current question when time is up
                NextQuestion();
            }
        }

        private void StartTimer()
        {
            if (!_isTimerRunning)
            {
                TimeLeft = 30; // Reset to 30 seconds
                _timer.Start();
                _isTimerRunning = true;
            }
        }

        private void StopTimer()
        {
            if (_isTimerRunning)
            {
                _timer.Stop();
                _isTimerRunning = false;
            }
        }

        private bool CanGoToNextQuestion()
        {
            return IsQuizLoaded && !IsQuizFinished;
        }

        private void LoadQuiz()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Quiz Files (*.json, *.txt)|*.json;*.txt|JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Wybierz plik z quizem"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".json")
                    {
                        LoadQuizFromJson(filePath);
                    }
                    else if (extension == ".txt")
                    {
                        LoadQuizFromText(filePath);
                    }
                    else
                    {
                        MessageBox.Show("Nieobsługiwany format pliku. Wybierz plik .json lub .txt.", "Błąd",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Initialize quiz state
                    CurrentQuestionIndex = 0;
                    _allUserAnswers.Clear();
                    QuizResults.Clear();
                    CorrectAnswersCount = 0;
                    IsQuizLoaded = true;
                    IsQuizFinished = false;

                    // Initialize answers for the first question
                    InitializeAnswersForCurrentQuestion();

                    // Upewnij się, że widok powitalny jest ukryty
                    OnPropertyChanged(nameof(IsQuizLoaded));
                    OnPropertyChanged(nameof(CurrentQuiz));
                    OnPropertyChanged(nameof(CurrentQuestion));

                    // Start the timer
                    StartTimer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas wczytywania pliku: {ex.Message}", "Błąd",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadQuizFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);
            CurrentQuiz = JsonSerializer.Deserialize<Models.Quiz>(json);
        }

        private void LoadQuizFromText(string filePath)
        {
            // Przygotuj dane z pliku tekstowego
            string[] lines = File.ReadAllLines(filePath);
            string title = lines.Length > 0 ? lines[0] : "Quiz"; // First line is the quiz title

            // Utwórz obiekt Quiz w taki sam sposób jak w metodzie LoadQuizFromJson
            Models.Quiz quiz = new Models.Quiz(title, new List<Question>());

            Question currentQuestion = null;
            List<string> options = new List<string>();
            List<string> correctAnswers = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Check if this is a question line
                if (!line.StartsWith("[") && currentQuestion == null)
                {
                    // This is a new question
                    currentQuestion = new Question();
                    currentQuestion.Text = line;
                    options = new List<string>();
                    correctAnswers = new List<string>();
                }
                // Check if this is an answer option
                else if (line.StartsWith("[") && currentQuestion != null)
                {
                    string option = line.Substring(line.IndexOf(']') + 1).Trim();
                    options.Add(option);

                    // Check if this option is marked as correct
                    if (line.StartsWith("[X]") || line.StartsWith("[x]"))
                    {
                        correctAnswers.Add(option);
                    }

                    // If this is the last option for the current question
                    if (i == lines.Length - 1 || (!string.IsNullOrWhiteSpace(lines[i + 1]) && !lines[i + 1].Trim().StartsWith("[")))
                    {
                        // Finalize the current question
                        currentQuestion.Questions = options;
                        currentQuestion.CorrectAnswers = correctAnswers;
                        quiz.Questions.Add(currentQuestion);
                        currentQuestion = null;
                    }
                }
            }

            CurrentQuiz = quiz;
        }

        private void InitializeAnswersForCurrentQuestion()
        {
            if (CurrentQuestion != null)
            {
                SelectedAnswers = new ObservableCollection<bool>(
                    Enumerable.Repeat(false, CurrentQuestion.Questions.Count));
                OnPropertyChanged(nameof(CurrentQuestion));
            }
        }

        private void NextQuestion()
        {
            // Save current answers
            if (CurrentQuestion != null && SelectedAnswers != null)
            {
                List<bool> currentAnswers = SelectedAnswers.ToList();
                _allUserAnswers.Add(currentAnswers);

                // Calculate if answer is correct
                List<string> selectedAnswerTexts = new List<string>();
                for (int i = 0; i < CurrentQuestion.Questions.Count; i++)
                {
                    if (SelectedAnswers[i])
                    {
                        selectedAnswerTexts.Add(CurrentQuestion.Questions[i]);
                    }
                }

                bool isCorrect = CurrentQuestion.IsAnswerCorrect(selectedAnswerTexts);
                if (isCorrect)
                {
                    CorrectAnswersCount++;
                }
            }

            StopTimer();

            // Move to next question or finish quiz
            if (CurrentQuestionIndex < TotalQuestions - 1)
            {
                CurrentQuestionIndex++;
                InitializeAnswersForCurrentQuestion();
                StartTimer();
            }
            else
            {
                FinishQuiz();
            }
        }

        private void FinishQuiz()
        {
            IsQuizFinished = true;
            StopTimer();
            PrepareResults();
        }

        private void PrepareResults()
        {
            QuizResults.Clear();

            for (int i = 0; i < CurrentQuiz.Questions.Count; i++)
            {
                var question = CurrentQuiz.Questions[i];
                List<bool> userAnswers = i < _allUserAnswers.Count ? _allUserAnswers[i] : new List<bool>();

                List<string> selectedAnswers = new List<string>();
                for (int j = 0; j < question.Questions.Count && j < userAnswers.Count; j++)
                {
                    if (userAnswers[j])
                    {
                        selectedAnswers.Add(question.Questions[j]);
                    }
                }

                bool isCorrect = question.IsAnswerCorrect(selectedAnswers);

                QuizResults.Add(new QuestionResultViewModel
                {
                    QuestionText = question.Text,
                    UserAnswers = selectedAnswers,
                    CorrectAnswers = question.CorrectAnswers,
                    IsCorrect = isCorrect
                });
            }
        }

        private void RestartQuiz()
        {
            if (CurrentQuiz != null)
            {
                // Reset quiz state
                CurrentQuestionIndex = 0;
                _allUserAnswers.Clear();
                QuizResults.Clear();
                CorrectAnswersCount = 0;
                IsQuizFinished = false;

                // Initialize answers for the first question
                InitializeAnswersForCurrentQuestion();

                // Start the timer
                StartTimer();
            }
        }
    }

    public class QuestionResultViewModel : ViewModelBase
    {
        public string QuestionText { get; set; }
        public List<string> UserAnswers { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public bool IsCorrect { get; set; }
    }
}