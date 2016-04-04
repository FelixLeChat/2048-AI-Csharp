using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2048.WPF.Annotations;

namespace _2048.WPF.Model
{
    public class StatModel : INotifyPropertyChanged
    {
        private int _maxScore;
        public int MaxScore
        {
            get { return _maxScore; }
            set
            {
                if (_maxScore != value)
                {
                    _maxScore = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _maxTile;
        public int MaxTile
        {
            get { return _maxTile; }
            set
            {
                if (_maxTile != value)
                {
                    _maxTile = value;
                    OnPropertyChanged();
                }
            }
        }


        private long _averageGameTime;
        public long AverageGameTime
        {
            get { return _averageGameTime; }
            set
            {
                if (_averageGameTime != value)
                {
                    _averageGameTime = value;
                    OnPropertyChanged();
                }
            }
        }    

        public long TotalGameTime { get; set; }


        private long _averageMoveTime;
        public long AverageMoveTime
        {
            get { return _averageMoveTime; }
            set
            {
                if (_averageMoveTime != value)
                {
                    _averageMoveTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public long TotalMoveTime { get; set; }

        private int _averageMoveCount;
        public int AverageMoveCount
        {
            get { return _averageMoveCount; }
            set
            {
                if (_averageMoveCount != value)
                {
                    _averageMoveCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalMoveCount { get; set; }


        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }

        public int TotalGamePlayed { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}