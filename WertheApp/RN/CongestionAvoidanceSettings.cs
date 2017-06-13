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
		}
    }
}

