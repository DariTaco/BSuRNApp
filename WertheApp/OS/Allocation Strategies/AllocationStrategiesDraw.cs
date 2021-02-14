using System;

using Xamarin.Forms;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategiesDraw : ContentPage
    {
        public AllocationStrategiesDraw()
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

