using Quiz.Commands;
using Quiz.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quiz.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        public ICommand NavigateAuthorsCommand { get; }

        public HomeViewModel(NavigationStore navigationStore)
        {
            NavigateAuthorsCommand = new NavigateAuthorsCommand(navigationStore);
        }
    }
}
