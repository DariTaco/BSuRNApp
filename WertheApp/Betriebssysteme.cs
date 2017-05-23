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
            var stackLayout = new StackLayout { Margin = new Thickness(10) };
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
                "Allocation Strategies", "Buddy System", "Page Replacement Strategies"
            };
			//after an item was clicked, open the respective app 
			listView.ItemTapped += async (sender, e) =>
			{
				var appName = e.Item.ToString();

				switch (appName)
				{
					case "Allocation Strategies":
                        await Navigation.PushAsync(new AllocationStrategies());
						break;
					case "Buddy System":
						await Navigation.PushAsync(new BuddySystem());
						break;
                    case "Page Replacement Strategies":
                        await Navigation.PushAsync(new PageReplacementStrategies());
                        break;
				}

			};

            stackLayout.Children.Add(listView);
		}
    }
}

