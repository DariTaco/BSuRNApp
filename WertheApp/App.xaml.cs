using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace WertheApp
{
    public partial class App : Application
    {
        // Button Styling
        public static Color _buttonBackground { get; set; }
        public static Color _buttonText { get; set; }
        public static double _buttonFontSize { get; set; }
        public static double _labelFontSize { get; set; }
        public static double _entryFontSize { get; set; }
        public static int _buttonCornerRadius { get; set; }
        public static Color _buttonColor = new Color(0.13, 0.58, 0.95, 0.8);

        // other styling
        public static Color _viewBackground { get; set; }

        // Names
        public static string _sHelpInfoHint { get; set; }
        public static string _disk { get; set; }

        // App Indexing
        internal const string AppLinkUri = "http://dariakern.de/wertheapp/{0}";


        //public static string _disk = { get; set; }

        public App()
        {
            InitializeComponent();

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                _buttonFontSize = Device.GetNamedSize(NamedSize.Title, typeof(Button));
                _labelFontSize = Device.GetNamedSize(NamedSize.Title, typeof(Button));
                _entryFontSize = Device.GetNamedSize(NamedSize.Title, typeof(Entry));
            }

            // Button Styling
            if (Device.RuntimePlatform == Device.Android)
            {
                
                _buttonBackground = _buttonColor;
                //_buttonBackground.AddLuminosity(50.0);
                _buttonText = Color.White;
                _buttonCornerRadius = 50;

                _viewBackground = Color.WhiteSmoke.AddLuminosity(0.93);
            }
            else
            {
                _buttonBackground = Color.Transparent;
                _buttonText = Color.Accent;
                _buttonCornerRadius = 5;
                _viewBackground = Color.WhiteSmoke;


            }

            // Names
            _disk = "disk";
            _sHelpInfoHint = "Help";

            // Start Page
            MainPage = new NavigationPage(new WertheAppPage());
        }

        // App Linking
        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            // App Linking
            Debug.WriteLine("URI received: " + uri);
            var query = uri.PathAndQuery.Trim(new[] { '/' });
            Debug.WriteLine("QUERY : " + query);

            // Start Page
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
