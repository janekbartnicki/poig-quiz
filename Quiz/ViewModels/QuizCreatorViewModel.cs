using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quiz.Commands;
using Quiz.Models;
using Quiz.Repositories;

namespace Quiz.ViewModels
{
    public class QuizCreatorViewModel : ViewModelBase
    {
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

        public QuizRepository QuizRepository { get; set; }

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

        public ICommand NextQuestionCommand { get; set; }
        public ICommand FinishQuizCommand { get; set; }

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

        public QuizCreatorViewModel()
        {
            QuizRepository = new QuizRepository();
            NextQuestionCommand = new RelayCommand(_ => NextQuestion());
            FinishQuizCommand = new RelayCommand(_ => FinishQuiz());
        }

        private void NextQuestion()
        {
            if (string.IsNullOrEmpty(QuizTitle))
            {
                return;
            }

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

            QuestionText = "";

            Answers = new ObservableCollection<string> { "", "", "", "" };
            OnPropertyChanged(nameof(Answers));

            CorrectAnswers = new ObservableCollection<bool> { false, false, false, false };
            OnPropertyChanged(nameof(CorrectAnswers));

            CurrentQuestionIndex++;

            IsTitlePanelVisible = false;
            IsQuestionPanelVisible = true;
        }

        private void FinishQuiz()
        {
            if(string.IsNullOrEmpty(QuizTitle) || Questions.Count == 0)
            {
                return;
            }

            try
            {
                var quiz = new Quiz.Models.Quiz(QuizTitle, Questions.ToList());

                QuizRepository.AddOrUpdate(quiz);

                MessageBox.Show(
                    $"Pomyślnie stworzono quiz {QuizTitle}!",
                    "Stworzono nowy quiz",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK
                );
            } catch
            {
                MessageBox.Show(
                    "Wystąpił błąd.",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK
                );
            }
        }
    }
}
