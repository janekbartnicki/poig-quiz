using Quiz.Stores;
using Quiz.ViewModels;

namespace Quiz.Commands
{
    public class NavigateAuthorsCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateAuthorsCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new AuthorsViewModel(_navigationStore);
        }
    }
}
