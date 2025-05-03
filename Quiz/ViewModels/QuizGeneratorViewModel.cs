using System;
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
        public ICommand ReadFileCommand { get; }

        public QuizRepository QuizRepository { get; }

        public QuizGeneratorViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            ReadFileCommand = new RelayCommand(_ => ReadQuizFile());
            QuizRepository = new QuizRepository("Resources/quiz_db.json", "password123");
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
