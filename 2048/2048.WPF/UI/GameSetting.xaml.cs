using _2048.WPF.Converter;

namespace _2048.WPF.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public GameSettings GameSettings { get; set; } = new GameSettings();
        public Window1()
        {
            InitializeComponent();
            DataContext = GameSettings;
        }
    }
}
