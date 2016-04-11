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
        /*private int _totalMoveCount;
        public int TotalMoveCount
        {
            get { return _totalMoveCount; }
            set
            {
                if (_totalMoveCount != value)
                {
                    _totalMoveCount = value;
                    OnPropertyChanged();
                }
            }
        }*/

        private int _totalWins;
        public int TotalWins
        {
            get { return _totalWins; }
            set
            {
                if (_totalWins != value)
                {
                    _totalWins = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalLosses;
        public int TotalLosses
        {
            get { return _totalLosses; }
            set
            {
                if (_totalLosses != value)
                {
                    _totalLosses = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalGamesPlayed;
        public int TotalGamePlayed
        {
            get { return _totalGamesPlayed; }
            set
            {
                if (_totalGamesPlayed != value)
                {
                    _totalGamesPlayed = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get128;
        public int Get128
        {
            get { return _get128; }
            set
            {
                if (_get128 != value)
                {
                    _get128 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get256;
        public int Get256
        {
            get { return _get256; }
            set
            {
                if (_get256 != value)
                {
                    _get256 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get512;
        public int Get512
        {
            get { return _get512; }
            set
            {
                if (_get512 != value)
                {
                    _get512 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get1024;
        public int Get1024
        {
            get { return _get1024; }
            set
            {
                if (_get1024 != value)
                {
                    _get1024 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get2048;
        public int Get2048
        {
            get { return _get2048; }
            set
            {
                if (_get2048 != value)
                {
                    _get2048 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get4096;
        public int Get4096
        {
            get { return _get4096; }
            set
            {
                if (_get4096 != value)
                {
                    _get4096 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get8192;
        public int Get8192
        {
            get { return _get8192; }
            set
            {
                if (_get8192 != value)
                {
                    _get8192 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _get16384;
        public int Get16384
        {
            get { return _get16384; }
            set
            {
                if (_get16384 != value)
                {
                    _get16384 = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}