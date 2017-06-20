using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemModal : ContentPage
    {
        public BuddySystemModal()
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

