using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Quiz.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}