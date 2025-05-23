﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Quiz.Commands;
using Quiz.Repositories;
using Quiz.Stores;
using System.IO;

namespace Quiz.ViewModels
{
    class QuizGeneratorViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateQuizCreatorCommand { get; }
        public ICommand NavigateQuizEditorCommand { get; }
        public ICommand ReadFileCommand { get; }
        public ICommand EditQuizCommand { get; }

        private Quiz.Models.Quiz _selectedQuiz;

        public Quiz.Models.Quiz SelectedQuiz
        {
            get { return _selectedQuiz; }
            set
            {
                if (_selectedQuiz != value)
                {
                    _selectedQuiz = value;
                    OnPropertyChanged(nameof(SelectedQuiz));
                }
            }
        }

        public QuizRepository QuizRepository { get; }

        public List<Quiz.Models.Quiz> QuizList { get; set; }

        public QuizGeneratorViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            NavigateQuizCreatorCommand = new NavigateQuizCreatorCommand(navigationStore);
            NavigateQuizEditorCommand = new NavigateQuizEditorCommand(navigationStore);
            ReadFileCommand = new RelayCommand(_ => ReadQuizFile());
            EditQuizCommand = new RelayCommand(_ =>
            {
                if (SelectedQuiz != null)
                {
                    NavigateQuizEditorCommand.Execute(SelectedQuiz);
                }
            });

            QuizRepository = new QuizRepository();
            QuizList = QuizRepository.LoadAll();
        }

        private void ReadQuizFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Quiz Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Select a Quiz File"
            };

            if(openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string json = File.ReadAllText(filePath);
                Quiz.Models.Quiz quiz = JsonSerializer.Deserialize<Quiz.Models.Quiz>(json);
                QuizRepository.AddOrUpdate(quiz);

                MessageBox.Show(
                    $"Pomyślnie wgrano plik: {filePath}", 
                    "Poprawnie wgrano plik", 
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK
                );
            }

        }
    }
}
