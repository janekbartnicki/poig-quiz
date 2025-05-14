using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Quiz.Models
{
    public class AnswerItem : INotifyPropertyChanged
    {
        private string _text;
        private bool _isSelected;

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public AnswerItem(string text, bool isSelected = false)
        {
            _text = text;
            _isSelected = isSelected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 