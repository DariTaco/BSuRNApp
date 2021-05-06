using System;
using Xamarin.Forms;

namespace WertheApp
{
    public class EmbeddedSystems: ContentPage
    {
        //CONSTRUCTOR
        public EmbeddedSystems()
        {
            Title = "Embedded Systems";
            var scrollView = new ScrollView
            {
                Margin = new Thickness(10)
            };
            var stackLayout = new StackLayout();
            this.Content = scrollView;
            scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content
            CreateContent(stackLayout);

        }

        //METHODS
        void CreateContent(StackLayout stackLayout)
        {
            var l_cs = new Label
            {
                Text = "Coming soon...",
                FontSize = App._H4FontSize

            };
            stackLayout.Children.Add(l_cs);
        }
    }
}
