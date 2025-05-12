using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Quiz.ViewModels;
using QuizApp.Commands;
using QuizApp.Models;

namespace QuizApp.ViewModels
{
    public class SummaryViewModel : ViewModelBase
    {
        private List<QuestionResult> _questionResults;
        private ObservableCollection<QuestionSummary> _questionSummaries;
        private int _correctAnswers;
        private int _totalQuestions;
        private double _percentageScore;
        private string _scoreSummary;

        public ObservableCollection<QuestionSummary> QuestionSummaries
        {
            get => _questionSummaries;
            set => SetProperty(ref _questionSummaries, value);
        }

        public int CorrectAnswers
        {
            get => _correctAnswers;
            set => SetProperty(ref _correctAnswers, value);
        }

        public int TotalQuestions
        {
            get => _totalQuestions;
            set => SetProperty(ref _totalQuestions, value);
        }

        public double PercentageScore
        {
            get => _percentageScore;
            set => SetProperty(ref _percentageScore, value);
        }

        public string ScoreSummary
        {
            get => _scoreSummary;
            set => SetProperty(ref _scoreSummary, value);
        }

        // Commands
        public RelayCommand RestartQuizCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        // Constructor
        public SummaryViewModel(List<QuestionResult> questionResults)
        {
            _questionResults = questionResults ?? new List<QuestionResult>();
            InitializeProperties();
            InitializeCommands();
        }

        // Default constructor for designer support
        public SummaryViewModel()
        {
            _questionResults = new List<QuestionResult>();
            InitializeProperties();
            InitializeCommands();
        }

        private void InitializeProperties()
        {
            // Calculate scores
            TotalQuestions = _questionResults.Count;
            CorrectAnswers = _questionResults.Count(r => r.IsCorrect);
            PercentageScore = TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;

            // Create summary text
            ScoreSummary = $"Score: {CorrectAnswers} out of {TotalQuestions} correct answers";

            // Create question summaries
            QuestionSummaries = new ObservableCollection<QuestionSummary>(
                _questionResults.Select((result, index) => new QuestionSummary
                {
                    QuestionNumber = index + 1,
                    Question = result.Question.QuestionText,
                    YourAnswer = result.SelectedAnswer?.Text ?? "No answer",
                    CorrectAnswer = result.Question.Answers.FirstOrDefault(a => a.IsCorrect)?.Text ?? "No correct answer",
                    IsCorrect = result.IsCorrect
                })
            );
        }

        private void InitializeCommands()
        {
            RestartQuizCommand = new RelayCommand(
                param => RestartQuiz(),
                param => CanRestartQuiz()
            );

            ExitCommand = new RelayCommand(
                param => ExitApplication(),
                param => CanExitApplication()
            );
        }

        private bool CanRestartQuiz()
        {
            // Add logic if needed, e.g., check if the quiz can be restarted
            return true;
        }

        private bool CanExitApplication()
        {
            // Add logic if needed, e.g., check if the application can be exited
            return true;
        }

        private void RestartQuiz()
        {
            try
            {
                var mainViewModel = Application.Current.Resources["MainViewModel"] as MainViewModel;
                mainViewModel?.StartNewQuiz();
            }
            catch (Exception ex)
            {
                // Log or handle the error
                MessageBox.Show($"Error restarting quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitApplication()
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                // Log or handle the error
                MessageBox.Show($"Error exiting application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // DTO for question summary display
    public class QuestionSummary
    {
        public int QuestionNumber { get; set; }
        public string Question { get; set; }
        public string YourAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
