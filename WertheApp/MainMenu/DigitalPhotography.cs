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




            /*
            var l_H1 = new Label
            {
                Text = "H1 biggest title",
                FontSize = App._H1FontSize
            };
            stackLayout.Children.Add(l_H1);

            var l_H2 = new Label
            {
                Text = "H2 second biggest title",
                FontSize = App._H2FontSize
            };
            stackLayout.Children.Add(l_H2);

            var l_H3 = new Label
            {
                Text = "H3 third biggest title",
                FontSize = App._H3FontSize
            };
            stackLayout.Children.Add(l_H3);

            var l_H4 = new Label
            {
                Text = "H4 fourth biggest title",
                FontSize = App._H4FontSize
            };
            stackLayout.Children.Add(l_H4);

            var l_text = new Label
            {
                Text = "text blabla ",
                FontSize = App._TextFontSize
            };
            stackLayout.Children.Add(l_text);

            var l_smallText = new Label
            {
                Text = "small text",
                FontSize = App._SmallTextFontSize
            };
            stackLayout.Children.Add(l_smallText);
            */

            /*
            var l_header = new Label
            {
                Text = "header i17 A96",
                FontSize = Device.GetNamedSize(NamedSize.Header, typeof(Label))
            };
            stackLayout.Children.Add(l_header);

            var l_title = new Label
            {
                Text = "title i28 A24",
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label))
            };
            stackLayout.Children.Add(l_title);

            var l_large = new Label
            {
                Text = "large i22 A22",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            stackLayout.Children.Add(l_large);

            var l_subtitle = new Label
            {
                Text = "subtitle i22 A16",
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label))
            };
            stackLayout.Children.Add(l_subtitle);

            var l_medium = new Label
            {
                Text = "medium i17 A17",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            stackLayout.Children.Add(l_medium);

            var l_body = new Label
            {
                Text = "body i17 A16",
                FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label))
            };
            stackLayout.Children.Add(l_body);

            var l_default = new Label
            {
                Text = "default i17 A14",
                FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label))
            };
            stackLayout.Children.Add(l_default);

            var l_small = new Label
            {
                Text = "small i14 A14",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };
            stackLayout.Children.Add(l_small);

            var l_caption = new Label
            {
                Text = "caption i12 A12",
                FontSize = Device.GetNamedSize(NamedSize.Caption, typeof(Label))
            };
            stackLayout.Children.Add(l_caption);

            var l_micro = new Label
            {
                Text = "micro i12 A10",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label))
            };
            stackLayout.Children.Add(l_micro);
            */

        }
    }
}
