using Quiz.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Quiz.Services
{
    public class QuizService
    {
        // Metoda wczytująca quiz z pliku
        public Models.Quiz LoadQuizFromFile(string filePath)
        {
            // W rzeczywistej aplikacji tutaj byłaby logika parsowania pliku
            // Dla celów demonstracyjnych zwracamy przykładowy quiz
            return CreateSampleQuiz();
        }

        // Metoda tworząca przykładowy quiz (w rzeczywistej aplikacji zastąpić wczytywaniem z pliku)
        private Models.Quiz CreateSampleQuiz()
        {
            var quiz = new Models.Quiz
            {
                Title = "Quiz przykładowy",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Które z poniższych są planetami Układu Słonecznego?",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Merkury", IsCorrect = true },
                            new Answer { Text = "Pluton", IsCorrect = false },
                            new Answer { Text = "Wenus", IsCorrect = true },
                            new Answer { Text = "Ceres", IsCorrect = false }
                        }
                    },
                    new Question
                    {
                        Text = "Wybierz języki programowania z silnym typowaniem:",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Python", IsCorrect = false },
                            new Answer { Text = "Java", IsCorrect = true },
                            new Answer { Text = "C#", IsCorrect = true },
                            new Answer { Text = "JavaScript", IsCorrect = false }
                        }
                    },
                    new Question
                    {
                        Text = "Które państwa leżą w Europie?",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Francja", IsCorrect = true },
                            new Answer { Text = "Japonia", IsCorrect = false },
                            new Answer { Text = "Polska", IsCorrect = true },
                            new Answer { Text = "Egipt", IsCorrect = false }
                        }
                    }
                }
            };

            return quiz;
        }

        // Metoda zapisująca wyniki quizu
        public void SaveQuizResults(Models.Quiz quiz, string filePath)
        {
            // Tutaj byłaby logika zapisywania wyników do pliku
            // W wersji demonstracyjnej ta metoda nie wykonuje żadnych działań
        }
    }
}