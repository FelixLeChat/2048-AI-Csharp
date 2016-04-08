using System.Threading;

namespace _2048.WPF.Game
{
    public class TrainingManager
    {
        private static TrainingManager _instance;
        public static TrainingManager Instance => _instance ?? (_instance = new TrainingManager());
        private TrainingManager()
        {
        }

        public void StartTraining(CancellationTokenSource cancelToken)
        {
            
        }
    }
}