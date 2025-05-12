using Microsoft.Win32;
//using Quiz.Commands;
using Quiz.Services;
using Quiz.ViewModels.Base;
using System;
using System.Windows;
using System.Windows.Input;

namespace Quiz.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly QuizService _quizService;
        private Models.Quiz _currentQuiz;

        // Flagi widoczności poszczególnych widoków
        private bool _isMainMenuVisible = true;
        public bool IsMainMenuVisible
        {
            get { return _isMainMenuVisible; }
            set { SetProperty(ref _isMainMenuVisible, value); }
        }

        private bool _isInfoVisible = false;
        public bool IsInfoVisible
        {
            get { return _isInfoVisible; }
            set { SetProperty(ref _isInfoVisible, value); }
        }

        private bool _isLoadQuizVisible = false;
        public bool IsLoadQuizVisible
        {
            get { return _isLoadQuizVisible; }
            set { SetProperty(ref _isLoadQuizVisible, value); }
        }

        // ViewModel dla ekranu pytań
        private QuizViewModel _quizViewModel;
        public QuizViewModel QuizViewModel
        {
            get { return _quizViewModel; }
            set { SetProperty(ref _quizViewModel, value); }
        }

        // Właściwości dla ekranu wczytywania quizu
        private string _quizFilePath;
        public string QuizFilePath
        {
            get { return _quizFilePath; }
            set
            {
                SetProperty(ref _quizFilePath, value);
                // Update the RelayCommand instantiation for StartQuizCommand to use a lambda expression for CanExecute
                StartQuizCommand = new RelayCommand(() => StartQuiz(null), () => CanStartQuiz(null));
                StartQuizCommand.RaiseCanExecuteChanged();
            }
        }

        private string _quizInfo;
        public string QuizInfo
        {
            get { return _quizInfo; }
            set { SetProperty(ref _quizInfo, value); }
        }

        private bool _isQuizInfoVisible;
        public bool IsQuizInfoVisible
        {
            get { return _isQuizInfoVisible; }
            set { SetProperty(ref _isQuizInfoVisible, value); }
        }

        // Komendy
        public ICommand PlayCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand BackToMainCommand { get; }
        public ICommand BrowseFileCommand { get; }
        public RelayCommand StartQuizCommand { get; }
        public ICommand CancelLoadQuizCommand { get; }

        public MainViewModel()
        {
            _quizService = new QuizService();

            // Inicjalizacja komend
            // Update RelayCommand instantiations to use lambda expressions instead of method groups
            PlayCommand = new RelayCommand(() => ShowLoadQuizScreen(null));
            InfoCommand = new RelayCommand(() => ShowInfoScreen(null));
            ExitCommand = new RelayCommand(() => ExitApplication(null));
            BackToMainCommand = new RelayCommand(() => ShowMainMenu(null));
            BrowseFileCommand = new RelayCommand(() => BrowseQuizFile(null));
            StartQuizCommand = new RelayCommand(() => StartQuiz(null));
            CancelLoadQuizCommand = new RelayCommand(() => ShowMainMenu(null));
        }

        private void ShowLoadQuizScreen(object parameter)
        {
            IsMainMenuVisible = false;
            IsInfoVisible = false;
            IsLoadQuizVisible = true;
        }

        private void ShowInfoScreen(object parameter)
        {
            IsMainMenuVisible = false;
            IsLoadQuizVisible = false;
            IsInfoVisible = true;
        }

        private void ShowMainMenu(object parameter)
        {
            IsInfoVisible = false;
            IsLoadQuizVisible = false;
            IsMainMenuVisible = true;

            // Jeśli istnieje QuizViewModel, ukrywamy go
            if (QuizViewModel != null)
            {
                QuizViewModel.IsQuizVisible = false;
                QuizViewModel.IsSummaryVisible = false;
            }
        }

        private void ExitApplication(object parameter)
        {
            if (MessageBox.Show("Czy na pewno chcesz zamknąć aplikację?", "Potwierdzenie",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void BrowseQuizFile(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                QuizFilePath = openFileDialog.FileName;

                try
                {
                    _currentQuiz = _quizService.LoadQuizFromFile(QuizFilePath);
                    QuizInfo = $"Wczytano poprawnie quiz z {_currentQuiz.Questions.Count} pytaniami.";
                    IsQuizInfoVisible = true;
                }
                catch (Exception ex)
                {
                    QuizInfo = $"Błąd wczytywania quizu: {ex.Message}";
                    IsQuizInfoVisible = true;
                    _currentQuiz = null;
                }
            }
        }

        private bool CanStartQuiz(object parameter)
        {
            return _currentQuiz != null && _currentQuiz.Questions.Count > 0;
        }

        private void StartQuiz(object parameter)
        {
            IsLoadQuizVisible = false;

            // Tworzenie ViewModel dla quizu
            QuizViewModel = new QuizViewModel(_currentQuiz);
            QuizViewModel.QuizCompleted += OnQuizCompleted;
            QuizViewModel.StartQuiz();
        }

        private void OnQuizCompleted(object sender, EventArgs e)
        {
            // Możemy tutaj obsłużyć dodatkowe akcje po zakończeniu quizu
        }
    }
}