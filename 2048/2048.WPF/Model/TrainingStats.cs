using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2048.WPF.Annotations;

namespace _2048.WPF.Model
{
    public class TrainingStats : INotifyPropertyChanged
    {
        private int _iterationToDo;
        public int IterationToDo
        {
            get { return _iterationToDo; }
            set
            {
                if (_iterationToDo != value)
                {
                    _iterationToDo = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _iterationDone;
        public int IterationDone
        {
            get { return _iterationDone; }
            set
            {
                if (_iterationDone != value)
                {
                    _iterationDone = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _currentGeneration;
        public int CurrentGeneration
        {
            get { return _currentGeneration; }
            set
            {
                if (_currentGeneration != value)
                {
                    _currentGeneration = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalChilds;
        public int TotalChilds
        {
            get { return _totalChilds; }
            set
            {
                if (_totalChilds != value)
                {
                    _totalChilds = value;
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