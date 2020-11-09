using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class DeadlockInfo : ContentPage
    {
        public DeadlockInfo()
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

