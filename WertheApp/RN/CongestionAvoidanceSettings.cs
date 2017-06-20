using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class CongestionAvoidanceSettings : ContentPage
    {
		//VARIABLES


		//CONSTRUCTOR
		public CongestionAvoidanceSettings()
		{
			Title = "Congestion Avoidance";
			CreateContent();
		}

		//METHODS
		void CreateContent()
		{
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10)
			};
			var stackLayout = new StackLayout();

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

			//add elements to stacklayout
			var b_Start = new Button { Text = "Start" };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)
			stackLayout.Children.Add(b_Start);
		}

		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            await Navigation.PushAsync(new CongestionAvoidance());
		}
    }
}

