using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2018.AI.Enums;
using _2048.WPF.Annotations;

namespace _2048.WPF.Model
{
    public class ScoreModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Score of game
        /// </summary>
        private int _score;
        public int Score
        {
            get { return _score; }
            set
            {
                if (_score == value) return;
                _score = value;
                OnPropertyChanged();
            }

        }

        /// <summary>
        /// Biggest tile made
        /// </summary>
        private int _maxTile;
        public int MaxTile
        {
            get { return _maxTile; }
            set
            {
                if (_maxTile == value) return;
                _maxTile = value;
                OnPropertyChanged();
            }

        }

        /// <summary>
        /// State of the end game (won/lost/notFinished)
        /// </summary>
        private State _state;
        public State State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged();
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}