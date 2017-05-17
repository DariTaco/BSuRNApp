using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class PageReplacementStrategies : ContentPage
    {
        public PageReplacementStrategies()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Page Replacement Strategies" }
                }
            };
        }
    }
}

