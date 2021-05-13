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
        public static double _labelFontSize { get; set; }
        public static double _entryFontSize { get; set; }
        public static int _buttonCornerRadius { get; set; }
        public static Color _buttonColor = new Color(0.13, 0.58, 0.95, 0.8);
        public static LayoutOptions _buttonHorizontalOptions { get; set; }

        // font sizes
        public static double _h1FontSize { get; set; }
        public static double _h2FontSize { get; set; }
        public static double _h3FontSize { get; set; }
        public static double _h4FontSize { get; set; }
        public static double _textFontSize { get; set; }
        public static double _smallTextFontSize { get; set; }
        public static double _buttonFontSize { get; set; }
        public static double _smallButtonFontSize { get; set; }





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

            // Tablet font size settings
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                _buttonFontSize = 28;
                _smallButtonFontSize = 22;
                _labelFontSize = Device.GetNamedSize(NamedSize.Title, typeof(Button));
                _entryFontSize = Device.GetNamedSize(NamedSize.Title, typeof(Entry));

                _h1FontSize = 60;
                _h2FontSize = 46;
                _h3FontSize = 40;
                _h4FontSize = 34;
                _textFontSize = 28;
                _smallTextFontSize = 22;

            }
            // Smartphone font size settings
            else
            {
                _buttonFontSize = 18;
                _smallButtonFontSize = 14;
                _labelFontSize = Device.GetNamedSize(NamedSize.Default, typeof(Button));
                _entryFontSize = Device.GetNamedSize(NamedSize.Default, typeof(Entry));

                _h1FontSize = 40;
                _h2FontSize = 32;
                _h3FontSize = 26;
                _h4FontSize = 22;
                _textFontSize = 18;
                _smallTextFontSize = 14;

            }

            // add to resources so it can be used in XAML
            Resources.Add("_H1FontSize", _h1FontSize);
            Resources.Add("_H2FontSize", _h2FontSize);
            Resources.Add("_H3FontSize", _h3FontSize);
            Resources.Add("_H4FontSize", _h4FontSize);
            Resources.Add("_TextFontSize", _textFontSize);
            Resources.Add("_SmallTextFontSize", _smallTextFontSize);


            // Button styling for Android
            if (Device.RuntimePlatform == Device.Android)
            {
                
                _buttonBackground = _buttonColor;
                _buttonText = Color.White;
                _buttonCornerRadius = 50;
                _viewBackground = new Color(0.93, 0.93, 0.93);
                _buttonHorizontalOptions = LayoutOptions.Center;
            }
            // Button sytling for iOS and other
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
