using System;
using Xamarin.Essentials;
using Xamarin.Forms;



/*
 In der Historie Datumswerte wie folgt angeben:

dd-mmm-yyyy, also heute: 15-Nov-2020,

d.h. Tageswerte zweistellig, Monatsangaben wie folgt:
Jan, Feb, Mar, Apr, Jun, Jul, Aug, Sep, Oct, Nov, Dec
Jahreszahlen vierstellig.

Ganz unten ein Datum mit Versionierung eingeführt angeben.
Hinter jeden Eintrag daneben auch die Versionsnummer angeben.
 
 
 */

namespace WertheApp
{
    public class Info : ContentPage
    {
        ScrollView scrollView;
        Label l_lastLabel;

        //CONSTRUCTOR
        public Info()
        {
            Title = "Info";

            scrollView = new ScrollView
            {
                Margin = new Thickness(10)
            };
            var stackL = new StackLayout();

            this.Content = scrollView;
            scrollView.Content = stackL; //Wrap ScrollView around StackLayout to be able to scroll the content

          

            // Current app version 
            var currentVersion = VersionTracking.CurrentVersion;
            var l_version = new Label { Text = "current version " + currentVersion , FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            stackL.Children.Add(l_version);

            var l_history = new Label { Text = "History ", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            stackL.Children.Add(l_history);

            var l_14122020 = new Label { Text = "14-Dec-2020, v. 1.6.4 : bug fixes ", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_14122020);
            var l_07122020 = new Label { Text = "7-Dec-2020, v. 1.6.3 : bug fixes ", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_07122020);
            var l_24112020 = new Label { Text = "24-Nov-2020, v. 1.6.2 : bug fixes ", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_24112020);
            var l_18112020 = new Label { Text = "18-Nov-2020, v. 1.6.1 : bug fixes ", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_18112020);
            var l_08112020 = new Label { Text = "08-Nov-2020, v. 1.6.0 : dark mode", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_08112020);
            var l_03112020 = new Label { Text = "03-Nov-2020, v. 1.6.0: bug fixes", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_03112020);
            var l_25102020 = new Label { Text = "25-Oct-2020, v. 1.5.0: restart buttons added", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_25102020);
            var l_25102020b = new Label { Text = "25-Oct-2020, v. 1.5.0: Deadlock (new)", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_25102020b);
            var l_24102020b = new Label { Text = "24-Oct-2020, v. 1.5.0: versioning introduced", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_24102020b);
            VersionTracking.Track();

            l_lastLabel = new Label { Text = "" };
            stackL.Children.Add(l_lastLabel);
        }


        public async void scroll()
        {
            //await scrollView.ScrollToAsync(0, 1500, true);
            await scrollView.ScrollToAsync(l_lastLabel, ScrollToPosition.End, true);


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            scroll();
        }
    }
}

