using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class CongestionAvoidance : ContentPage
    {
        public CongestionAvoidance()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Congestion Avpoidance" }
                }
            };
        }
    }
}

