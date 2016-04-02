using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2048.WPF.Enums;

namespace _2048.WPF.Converter
{
    public class GameSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Mode of the online game
        /// </summary>
        /*private Mode _gameMode;
        public Mode GameMode
        {
            get { return _gameMode; }
            set
            {
                if (_gameMode != value)
                {
                    _gameMode = value;
                    OnPropertyChanged();
                }
            }

        }*/

        /// <summary>
        /// Type of the online game
        /// </summary>
        private Type _gameType;
        public Type GameType
        {
            get { return _gameType; }
            set
            {
                if (_gameType != value)
                {
                    _gameType = value;
                    OnPropertyChanged();
                }
            }

        }

        /// <summary>
        /// Seed for the random cell generation in the game
        /// </summary>
        private string _gameSeed;
        public string GameSeed
        {
            get { return _gameSeed; }
            set
            {
                if (_gameSeed != value)
                {
                    _gameSeed = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Animate the game or not
        /// </summary>
        private bool _isAnimated;
        public bool IsAnimated
        {
            get { return _isAnimated; }
            set
            {
                if (_isAnimated != value)
                {
                    _isAnimated = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Restart the game when done
        /// </summary>
        private bool _isRestart = true;
        public bool IsRestart
        {
            get { return _isRestart; }
            set
            {
                if (_isRestart != value)
                {
                    _isRestart = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}