using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class AllocationStrategies : ContentPage
    {
        public AllocationStrategies()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Allocation Strategies" }
                }
            };
        }
    }
}

