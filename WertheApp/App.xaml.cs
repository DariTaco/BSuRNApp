using Xamarin.Forms;

namespace WertheApp
{
    public partial class App : Application
    {
        public static Color _buttonBackground { get; set; }
        public static Color _buttonText { get; set; }
        public static int _buttonCornerRadius { get; set; }
        public static Color _buttonColor = new Color(0.13, 0.58, 0.95, 0.8);
        public static string _sHelpInfoHint { get; set; }
        public App()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                
                _buttonBackground = _buttonColor;
                //_buttonBackground.AddLuminosity(50.0);
                _buttonText = Color.White;
                _buttonCornerRadius = 50;
            }
            else
            {
                _buttonBackground = Color.Transparent;
                _buttonText = Color.Accent;
                _buttonCornerRadius = 5;

            }
            _sHelpInfoHint = "HELP";
            MainPage = new NavigationPage(new WertheAppPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
