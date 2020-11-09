using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class PageReplacementStrategiesInfo : ContentPage
    {
        public PageReplacementStrategiesInfo()
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

