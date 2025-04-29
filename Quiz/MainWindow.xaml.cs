using Quiz.View;
using Quiz.ViewModel;
using System.Windows;

namespace Quiz
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Utwórz instancję modelu widoku
            var quizGeneratorViewModel = new QuizGeneratorViewModel();

            // Ustaw model widoku jako DataContext
            this.DataContext = new MainViewModel
            {
                CurrentViewModel = quizGeneratorViewModel
            };
        }
    }
}