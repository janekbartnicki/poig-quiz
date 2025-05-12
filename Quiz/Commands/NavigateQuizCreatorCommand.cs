using Quiz.Stores;
using Quiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            if(parameter != null)
            {
                _navigationStore.CurrentViewModel = new QuizCreatorViewModel(_navigationStore, (Quiz.Models.Quiz)parameter);
            } else
            {
                _navigationStore.CurrentViewModel = new QuizCreatorViewModel(_navigationStore);
            }
        }
    }
}
