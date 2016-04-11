using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
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
            var value = 0;
            if (!int.TryParse(GameSettings.FirstGenCountString, out value)) return;
            GameSettings.FirstGenCount = value;
            if (!int.TryParse(GameSettings.GenGameCountString, out value)) return;
            GameSettings.GenGameCount = value;
            if (!int.TryParse(GameSettings.DepthString, out value)) return;
            GameSettings.Depth = value;


            //Init Game Manager
            GameManager.Instance.Init(GameSettings);

            var gameWindow = new GameWindow(GameSettings);
            gameWindow.Show();
            Close();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // handled = true : do not put text
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            var regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }
    }
}
