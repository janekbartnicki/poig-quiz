using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Quiz.Models
{
    public class Question : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private List<Answer> _answers;
        public List<Answer> Answers
        {
            get { return _answers; }
            set
            {
                _answers = value;
                OnPropertyChanged(nameof(Answers));
            }
        }

        private double _userScore;
        public double UserScore
        {
            get { return _userScore; }
            set
            {
                _userScore = value;
                OnPropertyChanged(nameof(UserScore));
            }
        }

        private string _pointsInfo;
        public string PointsInfo
        {
            get { return _pointsInfo; }
            set
            {
                _pointsInfo = value;
                OnPropertyChanged(nameof(PointsInfo));
            }
        }

        private Brush _pointsColor;
        public Brush PointsColor
        {
            get { return _pointsColor; }
            set
            {
                _pointsColor = value;
                OnPropertyChanged(nameof(PointsColor));
            }
        }

        public string QuestionText => Text;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}