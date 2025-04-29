using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Quiz.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowQuizGeneratorCommand { get; }
        public ICommand ShowInfoCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel()
        {
            ShowQuizGeneratorCommand = new RelayCommand(o => ShowQuizGenerator());
            ShowInfoCommand = new RelayCommand(o => ShowInfo());
            ExitCommand = new RelayCommand(o => Application.Current.Shutdown());

            // Widok początkowy
            CurrentViewModel = new QuizGeneratorViewModel();
        }

        private void ShowQuizGenerator()
        {
            CurrentViewModel = new QuizGeneratorViewModel();
        }

        private void ShowInfo()
        {
            CurrentViewModel = new InfoViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
