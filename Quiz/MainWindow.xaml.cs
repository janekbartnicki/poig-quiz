using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Quiz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Klasa reprezentująca odpowiedź
        public class Answer
        {
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
            public bool IsSelected { get; set; }

            // Dla ekranu podsumowania
            public string Status { get; set; }
            public Brush StatusColor { get; set; }
        }

        // Klasa reprezentująca pytanie
        public class Question
        {
            public string Text { get; set; }
            public List<Answer> Answers { get; set; }
            public double UserScore { get; set; } // Punkty zdobyte przez użytkownika

            // Dla ekranu podsumowania
            public string PointsInfo { get; set; }
            public Brush PointsColor { get; set; }
            public string QuestionText { get { return Text; } }
        }

        private List<Question> quizQuestions = new List<Question>();
        private int currentQuestionIndex = 0;
        private double totalScore = 0;
        private DispatcherTimer questionTimer;
        private int secondsRemaining = 30;

        public MainWindow()
        {
            InitializeComponent();

            // Inicjalizacja timera dla pytań
            questionTimer = new DispatcherTimer();
            questionTimer.Interval = TimeSpan.FromSeconds(1);
            questionTimer.Tick += QuestionTimer_Tick;
        }

        // Obsługa kliknięcia przycisku Zagraj
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            LoadQuizGrid.Visibility = Visibility.Visible;
        }

        // Przejście do ekranu informacji
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            InfoGrid.Visibility = Visibility.Visible;
        }

        // Powrót do menu głównego
        private void BackToMainButton_Click(object sender, RoutedEventArgs e)
        {
            InfoGrid.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
        }

        // Wyjście z aplikacji
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz zamknąć aplikację?", "Potwierdzenie",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // Obsługa przycisku Przeglądaj do wyboru pliku
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                QuizFilePathTextBox.Text = openFileDialog.FileName;

                try
                {
                    LoadQuizFromFile(openFileDialog.FileName);
                    QuizInfoTextBlock.Text = $"Wczytano poprawnie quiz z {quizQuestions.Count} pytaniami.";
                    QuizInfoTextBlock.Visibility = Visibility.Visible;
                    StartQuizButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    QuizInfoTextBlock.Text = $"Błąd wczytywania quizu: {ex.Message}";
                    QuizInfoTextBlock.Visibility = Visibility.Visible;
                    StartQuizButton.IsEnabled = false;
                }
            }
        }

        // Wczytywanie quizu z pliku
        private void LoadQuizFromFile(string filePath)
        {
            quizQuestions.Clear();

            // Dla celów prezentacyjnych, tworzymy przykładowe pytania
            // W rzeczywistym scenariuszu tutaj należałoby wczytać pytania z pliku
            quizQuestions.Add(new Question
            {
                Text = "Które z poniższych są planetami Układu Słonecznego?",
                Answers = new List<Answer>
                {
                    new Answer { Text = "Merkury", IsCorrect = true },
                    new Answer { Text = "Pluton", IsCorrect = false },
                    new Answer { Text = "Wenus", IsCorrect = true },
                    new Answer { Text = "Ceres", IsCorrect = false }
                }
            });

            quizQuestions.Add(new Question
            {
                Text = "Wybierz języki programowania z silnym typowaniem:",
                Answers = new List<Answer>
                {
                    new Answer { Text = "Python", IsCorrect = false },
                    new Answer { Text = "Java", IsCorrect = true },
                    new Answer { Text = "C#", IsCorrect = true },
                    new Answer { Text = "JavaScript", IsCorrect = false }
                }
            });

            quizQuestions.Add(new Question
            {
                Text = "Które państwa leżą w Europie?",
                Answers = new List<Answer>
                {
                    new Answer { Text = "Francja", IsCorrect = true },
                    new Answer { Text = "Japonia", IsCorrect = false },
                    new Answer { Text = "Polska", IsCorrect = true },
                    new Answer { Text = "Egipt", IsCorrect = false }
                }
            });

            // W rzeczywistym scenariuszu powyższe przykładowe pytania należałoby zastąpić
            // wczytywaniem z pliku, np. w formacie JSON lub XML
        }

        // Anulowanie wczytywania quizu
        private void CancelLoadQuizButton_Click(object sender, RoutedEventArgs e)
        {
            LoadQuizGrid.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
        }

        // Rozpoczęcie quizu
        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            LoadQuizGrid.Visibility = Visibility.Collapsed;
            QuizQuestionGrid.Visibility = Visibility.Visible;

            currentQuestionIndex = 0;
            totalScore = 0;
            DisplayCurrentQuestion();
        }

        // Wyświetlenie aktualnego pytania
        private void DisplayCurrentQuestion()
        {
            Question currentQuestion = quizQuestions[currentQuestionIndex];

            QuestionNumberTextBlock.Text = $"Pytanie {currentQuestionIndex + 1}/{quizQuestions.Count}";
            QuestionTextBlock.Text = currentQuestion.Text;

            // Resetowanie wybranych odpowiedzi
            foreach (var answer in currentQuestion.Answers)
            {
                answer.IsSelected = false;
            }

            // Ustawienie odpowiedzi do wyświetlenia
            AnswersItemsControl.ItemsSource = new ObservableCollection<Answer>(currentQuestion.Answers);

            // Uruchomienie timera
            secondsRemaining = 30;
            TimerTextBlock.Text = secondsRemaining.ToString();
            questionTimer.Start();
        }

        // Obsługa timera dla pytania
        private void QuestionTimer_Tick(object sender, EventArgs e)
        {
            secondsRemaining--;
            TimerTextBlock.Text = secondsRemaining.ToString();

            if (secondsRemaining <= 0)
            {
                questionTimer.Stop();
                // Automatyczne przejście do następnego pytania po upływie czasu
                ProcessAnswers();
                MoveToNextQuestion();
            }
        }

        // Przetworzenie odpowiedzi użytkownika
        private void ProcessAnswers()
        {
            Question currentQuestion = quizQuestions[currentQuestionIndex];
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

                // Dodanie statusu dla podsumowania
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
            totalScore += pointsForQuestion;

            // Informacja o punktach dla podsumowania
            currentQuestion.PointsInfo = $"Zdobyte punkty: {pointsForQuestion:F1}";
            currentQuestion.PointsColor = pointsForQuestion >= 0.5 ? Brushes.Green : Brushes.Red;
        }

        // Przejście do następnego pytania
        private void MoveToNextQuestion()
        {
            currentQuestionIndex++;

            if (currentQuestionIndex < quizQuestions.Count)
            {
                DisplayCurrentQuestion();
            }
            else
            {
                // Koniec quizu
                ShowQuizSummary();
            }
        }

        // Obsługa przycisku Następne pytanie
        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            questionTimer.Stop();
            ProcessAnswers();
            MoveToNextQuestion();
        }

        // Wyświetlenie podsumowania quizu
        private void ShowQuizSummary()
        {
            QuizQuestionGrid.Visibility = Visibility.Collapsed;
            QuizSummaryGrid.Visibility = Visibility.Visible;

            // Wyświetlenie wyniku
            ScoreTextBlock.Text = totalScore.ToString("F1");
            MaxScoreTextBlock.Text = $"/{quizQuestions.Count}";

            // Wyświetlenie podsumowania pytań
            SummaryItemsControl.ItemsSource = quizQuestions;
        }

        // Powrót do menu głównego z podsumowania
        private void BackToMainFromSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            QuizSummaryGrid.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
        }
    }
}