using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class DijkstraInfo : ContentPage
    {
        public DijkstraInfo()
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

