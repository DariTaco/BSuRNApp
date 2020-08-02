using WertheApp.BS;
using Xamarin.Forms;
using System.Diagnostics;

namespace WertheApp
{
    public class Betriebssysteme : ContentPage
    {
        //CONSTRUCTOR
        public Betriebssysteme()
        {
            Title = "Operating Systems";
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
			listView.BackgroundColor = Color.Transparent;
			listView.ItemsSource = new string[]
            {
                "Allocation Strategies", "Buddy System", "Page Replacement Strategies", "Deadlock"
			};
			//after an item was clicked, open the respective app 
			listView.ItemTapped += async (sender, e) =>
			{
				var appName = e.Item.ToString();

				switch (appName)
				{
					case "Allocation Strategies":
                        await Navigation.PushAsync(new OldAllocationStrategiesSettings());
						break;
					case "Buddy System":
						await Navigation.PushAsync(new BuddySystemSettings());
						break;
                    case "Page Replacement Strategies":
                        await Navigation.PushAsync(new PageReplacementStrategiesSettings());
                        break;
					case "Deadlock":
						await Navigation.PushAsync(new DeadlockSettings());
						break;
				}

			};

            stackLayout.Children.Add(listView);
		}
    }
}