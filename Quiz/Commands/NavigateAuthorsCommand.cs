using Quiz.Stores;
using Quiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _navigationStore.CurrentViewModel = new AuthorsViewModel();
        }
    }
}
