using System;
using System.Windows.Input;
//using Quiz.Commands;
using Quiz.ViewModels.Base;

namespace Quiz.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        private string _applicationInfo;
        private string _versionInfo;
        private string _authorInfo;
        private bool _isVisible;

        public string ApplicationInfo
        {
            get { return _applicationInfo; }
            set { SetProperty(ref _applicationInfo, value); }
        }

        public string VersionInfo
        {
            get { return _versionInfo; }
            set { SetProperty(ref _versionInfo, value); }
        }

        public string AuthorInfo
        {
            get { return _authorInfo; }
            set { SetProperty(ref _authorInfo, value); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public ICommand BackCommand { get; }

        public InfoViewModel()
        {
            // Inicjalizacja danych informacyjnych
            ApplicationInfo = "Quiz Application - Aplikacja do rozwiązywania quizów";
            VersionInfo = "Wersja 1.0.0";
            AuthorInfo = "Autor: Twoje Imię i Nazwisko";

            // Inicjalizacja komend
            BackCommand = new RelayCommand(() => OnBack(null));
        }

        private void OnBack(object parameter)
        {
            // Obsługa powrotu do głównego menu
            IsVisible = false;

            // Jeśli potrzebne jest powiadomienie głównego ViewModelu
            // można tutaj wyzwolić zdarzenie lub wykorzystać wzorzec mediator
            // Przykład zdarzenia:
            BackToMainRequested?.Invoke(this, EventArgs.Empty);
        }

        // Zdarzenie, które może być obsługiwane przez MainViewModel
        public event EventHandler BackToMainRequested;
    }
}