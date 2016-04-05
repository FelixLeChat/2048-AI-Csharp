using _2048.WPF.Converter;
using _2048.WPF.Game;
using _2048.WPF.Model;

namespace _2048.WPF.UI
{
    public partial class GameSettingWindow
    {
        public static GameSettings GameSettings { get; set; } = new GameSettings();
        public GameSettingWindow()
        {
            InitializeComponent();
            DataContext = GameSettings;
        }

        private void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Init Game Manager
            GameManager.Instance.Init(GameSettings);

            var gameWindow = new GameWindow(GameSettings);
            gameWindow.Show();
            Close();
        }
    }
}
