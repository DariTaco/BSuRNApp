using System;
using Xamarin.Forms;

namespace WertheApp
{
    public class DigitalPhotography : ContentPage
    {
        //CONSTRUCTOR
        public DigitalPhotography()
        {
            Title = "Digital Photography";
            var stackLayout = new StackLayout { Margin = new Thickness(10) };
            this.Content = stackLayout;

            CreateContent(stackLayout);
        }

        //METHODS
        void CreateContent(StackLayout stackLayout)
        {
            var l_cs = new Label
            {
                Text = "Coming soon...",
                FontSize = App._labelFontSize

            };
            stackLayout.Children.Add(l_cs);
        }
    }
}
