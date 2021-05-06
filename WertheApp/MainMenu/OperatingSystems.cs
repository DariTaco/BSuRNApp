using Xamarin.Forms;
using System.Diagnostics;
using WertheApp.OS;
using WertheApp.OS.AllocationStrategies;
using System.Collections.Generic;

namespace WertheApp
{
    public class OperatingSystems : ContentPage
    {
        //CONSTRUCTOR
        public OperatingSystems()
        {

            Title = "Operating Systems";

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
			var l_choose = new Label
			{
				Text = "Choose an App",
				FontSize = App._H4FontSize

            };
			stackLayout.Children.Add(l_choose);

			var l_space = new Label();
			stackLayout.Children.Add(l_space);

            //add buttons for apps
            List<string> appNameList = new List<string>()    {
                        "Allocation Strategies",
                        "Buddy System",
                        "Deadlock",
                        "Page Replacement Strategies"
                    };
            foreach (string appName in appNameList)
            {
                var b_button = new Button
                {
                    Text = appName,
                    BackgroundColor = App._buttonBackground,
                    TextColor = App._buttonText,
                    CornerRadius = App._buttonCornerRadius,
                    FontSize = App._buttonFontSize
                };
                b_button.Clicked += Button_Clicked;
                stackLayout.Children.Add(b_button);
            }
        }

        async void Button_Clicked(object sender, System.EventArgs e)
        {
            var button = (sender as Button);
            string appName = button.Text;

            switch (appName)
            {
                case "Allocation Strategies":
                    await Navigation.PushAsync(new AllocationStrategiesSettings());
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
        }
        protected override void OnAppearing()
		{
			base.OnAppearing();
			MessagingCenter.Send<object>(this, "Unspecified");
		}
	}
}


/*
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
                        await Navigation.PushAsync(new AllocationStrategiesSettings());
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
 */