using System;
using Xamarin.Essentials;


using Xamarin.Forms;

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
            var l_03112020 = new Label { Text = "11/03/2020: bug fixes", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_03112020);
            var l_25102020 = new Label { Text = "10/25/2020: restart buttons added", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_25102020);
            var l_25102020b = new Label { Text = "10/25/2020: Deadlock (new)", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_25102020b);
            VersionTracking.Track();
        }
    }
}

