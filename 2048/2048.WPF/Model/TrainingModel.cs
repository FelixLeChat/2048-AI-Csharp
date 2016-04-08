using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2048.WPF.Annotations;

namespace _2048.WPF.Model
{
    public class TrainingModel : INotifyPropertyChanged
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

        private int _minTile;
        public int MinTile
        {
            get { return _minTile; }
            set
            {
                if (_minTile != value)
                {
                    _minTile = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _averageScore;
        public int AverageScore
        {
            get { return _averageScore; }
            set
            {
                if (_averageScore != value)
                {
                    _averageScore = value;
                    OnPropertyChanged();
                }
            }
        }


        public float LostPenalty { get; set; }
        public float MonoticityPower { get; set; }
        public float MonoticityWeight { get; set; }
        public float SumPower { get; set; }
        public float SumWeight { get; set; }
        public float MergeWeigth { get; set; }
        public float EmptyWeigth { get; set; }
        public float FillWeigth { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}