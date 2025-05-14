using System;
using System.Windows.Input;
using Quiz.Stores;
using Quiz.ViewModels;

namespace Quiz.Commands
{
    public class NavigateHomeCommand : ICommand
    {
        private readonly NavigationStore _navigationStore;

        public NavigateHomeCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
        }
    }
}
