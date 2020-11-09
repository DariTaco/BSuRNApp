using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class CongestionAvoidanceInfo : ContentPage
    {
        public CongestionAvoidanceInfo()
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

