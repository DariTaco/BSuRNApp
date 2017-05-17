using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystem : ContentPage
    {
        public BuddySystem()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Buddy System" }
                }
            };
        }
    }
}

