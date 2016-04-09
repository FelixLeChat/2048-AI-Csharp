using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2048.AI.Enums;

namespace _2048.WPF.Model
{
    public class GameSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Mode of the online game
        /// </summary>
        private Mode _gameMode;
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

        }

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
        /// Board type to set
        /// </summary>
        private BoardType _boardType;
        public BoardType BoardType
        {
            get { return _boardType; }
            set
            {
                if (_boardType != value)
                {
                    _boardType = value;
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

        private bool _loadPreviousGen = true;
        public bool LoadPreviousGen
        {
            get { return _loadPreviousGen; }
            set
            {
                if (_loadPreviousGen != value)
                {
                    _loadPreviousGen = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FirstGenCount { get; set; } = 5;
        public string FirstGenCountString { get; set; } = "5";

        public int GenGameCount { get; set; } = 100;
        public string GenGameCountString { get; set; } = "100";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}