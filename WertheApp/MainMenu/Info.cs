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
        //CONSTRUCTOR
        public Info()
        {
            Title = "Info";
            StackLayout stackL = new StackLayout { };
            Content = stackL;

            // Current app version 
            var currentVersion = VersionTracking.CurrentVersion;
            var l_version = new Label { Text = "current version " + currentVersion , FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            stackL.Children.Add(l_version);

            var l_changes = new Label { Text = "changes: ", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            stackL.Children.Add(l_changes);
            var l_08112020 = new Label { Text = "08-Nov-2020: dark mode", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_08112020);
            var l_03112020 = new Label { Text = "03-Nov-2020: bug fixes", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_03112020);
            var l_25102020 = new Label { Text = "25-Oct-2020: restart buttons added", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_25102020);
            var l_25102020b = new Label { Text = "25-Oct-2020: Deadlock (new)", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_25102020b);
            var l_24102020b = new Label { Text = "24-Oct-2020: versioning introduced", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_24102020b);
            VersionTracking.Track();
        }
    }
}

