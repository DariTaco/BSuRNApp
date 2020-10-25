using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class PipelineProtocolsInfo : ContentPage
    {
        public PipelineProtocolsInfo()
        {
            Title = "Hint";
            StackLayout stackL = new StackLayout { };
            Content = stackL;
             
            // clicks explained
            var l_first = new Label { Text = "1st click: corrupt packet", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_first);
            var l_second = new Label { Text = "2nd click: slow packet, but ok", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_second);
            var l_third = new Label { Text = "3rd click: slow and corrupt packet", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_third);
            var l_fourth = new Label { Text = "4th click: packet lost", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            stackL.Children.Add(l_fourth);
        }
    }
}

