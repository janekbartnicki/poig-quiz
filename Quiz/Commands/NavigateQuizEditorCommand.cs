using Quiz.Stores;
using Quiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Commands
{
    class NavigateQuizEditorCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateQuizEditorCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new QuizEditorViewModel(_navigationStore, (Quiz.Models.Quiz)parameter);
        }
    }
}
