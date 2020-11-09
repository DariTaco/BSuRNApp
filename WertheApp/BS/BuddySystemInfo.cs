using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemInfo : ContentPage
    {
        public BuddySystemInfo()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

