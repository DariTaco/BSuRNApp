using System;
using WertheApp.BS;
using Xamarin.Forms;

namespace WertheApp
{
    public class Betriebssysteme : ContentPage
    {
        //CONSTRUCTOR
        public Betriebssysteme()
        {
            Title = "Betriebssysteme";
            var stackLayout = new StackLayout();
            this.Content = stackLayout;

            CreateContent(stackLayout);
        }

		//METHODS
		void CreateContent(StackLayout stackLayout)
		{
			var l_choose = new Label
			{
				Text = "Choose an App:"
			};
			stackLayout.Children.Add(l_choose);

			var l_space = new Label();
			stackLayout.Children.Add(l_space);

            var listView = new ListView();
			listView.ItemsSource = new string[]
            {
                "Allocation Strategies", "Buddy System", "PageReplacement Strategies"
            };
            stackLayout.Children.Add(listView);
		}
    }
}

