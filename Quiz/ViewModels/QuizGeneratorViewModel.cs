using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Quiz.Commands;
using Quiz.Stores;

namespace Quiz.ViewModels
{
    class QuizGeneratorViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        public QuizGeneratorViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
        }
    }
}
