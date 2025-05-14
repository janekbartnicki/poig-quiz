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
using System.Windows.Controls;
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
        private ObservableCollection<AnswerAdapter> _currentVisibleAnswers;

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
                    OnPropertyChanged(nameof(IsLastQuestion));
                    OnPropertyChanged(nameof(CanGoPrevious));
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

        public ObservableCollection<AnswerAdapter> CurrentVisibleAnswers
        {
            get { return _currentVisibleAnswers; }
            set
            {
                _currentVisibleAnswers = value;
                OnPropertyChanged(nameof(CurrentVisibleAnswers));
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

        public bool IsLastQuestion
        {
            get { return CurrentQuestionIndex == TotalQuestions - 1; }
        }

        public bool CanGoPrevious
        {
            get { return IsQuizLoaded && !IsQuizFinished && CurrentQuestionIndex > 0; }
        }

        public string MultipleChoiceInfo
        {
            get
            {
                if (CurrentQuestion == null || CurrentQuestion.CorrectAnswers == null)
                    return "Wybierz odpowiedź";

                if (CurrentQuestion.CorrectAnswers.Count == 1)
                    return "Wybierz jedną odpowiedź";
                else
                    return $"Wybierz {CurrentQuestion.CorrectAnswers.Count} odpowiedzi";
            }
        }

        // Commands
        public ICommand LoadQuizCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }
        public ICommand FinishQuizCommand { get; }
        public ICommand RestartQuizCommand { get; }

        public QuizViewModel()
        {
            LoadQuizCommand = new RelayCommand(_ => LoadQuiz());
            NextQuestionCommand = new RelayCommand(_ => NextQuestion(), CanGoToNextQuestion);
            PreviousQuestionCommand = new RelayCommand(_ => PreviousQuestion(), _ => CanGoPrevious);
            FinishQuizCommand = new RelayCommand(_ => FinishQuiz(), _ => IsQuizLoaded && !IsQuizFinished);
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

        private bool CanGoToNextQuestion(object parameter)
        {
            // Uproszczona wersja bez sprawdzania indeksu - to mogło być problemem
            bool canExecute = IsQuizLoaded && !IsQuizFinished;
            return canExecute;
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
                    
                    // Sprawdź, czy quiz został załadowany prawidłowo
                    if (CurrentQuiz == null || CurrentQuiz.Questions == null || CurrentQuiz.Questions.Count == 0)
                    {
                        MessageBox.Show("Quiz nie zawiera żadnych pytań!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    OnPropertyChanged(nameof(IsLastQuestion));
                    OnPropertyChanged(nameof(CanGoPrevious));

                    // Start the timer
                    StartTimer();
                    
                    // Force command evaluation
                    CommandManager.InvalidateRequerySuggested();
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
            try
            {
                // Odczytaj wszystkie linie z pliku
                string[] lines = File.ReadAllLines(filePath);
                
                // Pierwszy wiersz to tytuł
                string title = lines.Length > 0 ? lines[0] : "Quiz";
                
                // Lista wszystkich pytań
                List<Question> questions = new List<Question>();
                
                // Tymczasowe zmienne do budowania pytania
                Question currentQ = null;
                List<string> options = null;
                List<string> correctOptions = null;
                
                // Przetwarzanie każdej linii
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    
                    // Pomijamy puste linie
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    // Jeśli to jest nowe pytanie (nie zaczyna się od '[')
                    if (!line.StartsWith("["))
                    {
                        // Jeśli mieliśmy już pytanie, dodajemy je do listy
                        if (currentQ != null)
                        {
                            currentQ.Questions = new List<string>(options);
                            currentQ.CorrectAnswers = new List<string>(correctOptions);
                            questions.Add(currentQ);
                        }
                        
                        // Tworzymy nowe pytanie
                        currentQ = new Question { Text = line };
                        options = new List<string>();
                        correctOptions = new List<string>();
                    }
                    // Jeśli to jest opcja odpowiedzi
                    else if (line.StartsWith("[") && currentQ != null)
                    {
                        string option = line.Substring(line.IndexOf(']') + 1).Trim();
                        options.Add(option);
                        
                        // Jeśli to poprawna odpowiedź
                        if (line.StartsWith("[X]") || line.StartsWith("[x]"))
                        {
                            correctOptions.Add(option);
                        }
                    }
                }
                
                // Dodajemy ostatnie pytanie, jeśli istnieje
                if (currentQ != null)
                {
                    currentQ.Questions = new List<string>(options);
                    currentQ.CorrectAnswers = new List<string>(correctOptions);
                    questions.Add(currentQ);
                }
                
                // Tworzymy i ustawiamy obiekt Quiz
                Models.Quiz quiz = new Models.Quiz(title, questions);
                CurrentQuiz = quiz;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeAnswersForCurrentQuestion()
        {
            if (CurrentQuestion != null)
            {
                // Clear previous answers
                SelectedAnswers = new ObservableCollection<bool>(
                    Enumerable.Repeat(false, CurrentQuestion.Questions.Count));
                    
                // Create answer items - najpierw wyczyścimy kolekcję
                if (CurrentVisibleAnswers == null)
                {
                    CurrentVisibleAnswers = new ObservableCollection<AnswerAdapter>();
                }
                else
                {
                    CurrentVisibleAnswers.Clear();
                }
                
                // Dodaj odpowiedzi do kolekcji
                foreach (var answer in CurrentQuestion.Questions)
                {
                    CurrentVisibleAnswers.Add(new AnswerAdapter(answer, false));
                }
                
                // Aktualizuj UI
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(CurrentVisibleAnswers));
                OnPropertyChanged(nameof(IsLastQuestion));
                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(MultipleChoiceInfo));
                
                // Wymuś aktualizację komend
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void NextQuestion()
        {
            try
            {
                // Save current answers
                if (CurrentQuestion != null && CurrentVisibleAnswers != null)
                {
                    List<bool> currentAnswers = CurrentVisibleAnswers.Select(a => a.IsSelected).ToList();
                    
                    // Update or add to the answers list
                    if (CurrentQuestionIndex < _allUserAnswers.Count)
                    {
                        _allUserAnswers[CurrentQuestionIndex] = currentAnswers;
                    }
                    else
                    {
                        _allUserAnswers.Add(currentAnswers);
                    }

                    // Calculate if answer is correct
                    List<string> selectedAnswerTexts = new List<string>();
                    for (int i = 0; i < CurrentVisibleAnswers.Count; i++)
                    {
                        if (CurrentVisibleAnswers[i].IsSelected)
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
                    int nextIndex = CurrentQuestionIndex + 1;
                    
                    // Set the next index
                    CurrentQuestionIndex = nextIndex;
                    
                    // Initialize answers for the new question
                    InitializeAnswersForCurrentQuestion();
                    
                    // Start the timer
                    StartTimer();
                }
                else
                {
                    FinishQuiz();
                }
                
                // Force command evaluation
                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas przechodzenia do następnego pytania: {ex.Message}", "Błąd", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void PreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
            {
                StopTimer();
                CurrentQuestionIndex--;
                InitializeAnswersForCurrentQuestion();
                
                // Restore previous answers if available
                if (_allUserAnswers.Count > CurrentQuestionIndex)
                {
                    var previousAnswers = _allUserAnswers[CurrentQuestionIndex];
                    for (int i = 0; i < previousAnswers.Count && i < CurrentVisibleAnswers.Count; i++)
                    {
                        CurrentVisibleAnswers[i].IsSelected = previousAnswers[i];
                    }
                }
                
                StartTimer();
                
                OnPropertyChanged(nameof(IsLastQuestion));
                OnPropertyChanged(nameof(CanGoPrevious));
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

    // Adapter class for answers
    public class AnswerAdapter : ViewModelBase
    {
        private string _text;
        private bool _isSelected;
        
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        
        public AnswerAdapter(string text, bool isSelected = false)
        {
            Text = text;
            IsSelected = isSelected;
        }
    }
}