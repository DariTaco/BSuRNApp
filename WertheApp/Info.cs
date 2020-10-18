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
            var l_version = new Label { Text = "version " + currentVersion , FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_version);
            VersionTracking.Track();
        }
    }
}

