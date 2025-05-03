using Quiz.Stores;
using Quiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Commands
{
    class NavigateQuizCreatorCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateQuizCreatorCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new QuizCreatorViewModel();
        }
    }
}
