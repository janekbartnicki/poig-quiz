using System.Windows.Input;
using Quiz.Commands;
using Quiz.Stores;

namespace Quiz.ViewModels
{
    class AuthorsViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        public AuthorsViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
        }
    }
}
