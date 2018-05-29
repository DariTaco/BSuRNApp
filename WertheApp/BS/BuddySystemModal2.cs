/**********************CLASS FOR END PROCESS POP UP****************************/

using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemModal2 : ContentPage
    {
		//VARIABLES
		Picker p_ProcessNames;

        //CONSTRUCTOR
        public BuddySystemModal2()
        {
			//Title = "";
			CreateContent();
        }

		//METHODS

		/**********************************************************************
        *********************************************************************/
		void CreateContent()
		{
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};
			
            var stackLayout = new StackLayout();
            var stackLayout2 = new StackLayout
            {
                Orientation = StackOrientation.Horizontal 
            };

			var b_End = new Button { Text = "End process" };
			b_End.Clicked += B_End_Clicked;
            var l_Space = new Label { Text = "  " };
			var b_Cancel = new Button { Text = "Cancel" };
			b_Cancel.Clicked += B_Cancel_Clicked;

            stackLayout2.Children.Add(b_End);
            stackLayout2.Children.Add(l_Space);
            stackLayout2.Children.Add(b_Cancel);

			var l_Active = new Label { Text = "Active processes:" };
			p_ProcessNames = new Picker();
            p_ProcessNames.ItemsSource = BuddySystem.activeProcesses;
			p_ProcessNames.SelectedIndex = 0;

			stackLayout.Children.Add(l_Active);
			stackLayout.Children.Add(p_ProcessNames);
			stackLayout.Children.Add(stackLayout2);

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content
		}

		/**********************************************************************
        *********************************************************************/
		async void B_End_Clicked(object sender, EventArgs e)
        {
            BuddySystem.DeallocateBlock(p_ProcessNames.SelectedItem.ToString());
            BuddySystem.activeProcesses.Remove(p_ProcessNames.SelectedItem.ToString());
            BuddySystem.availableProcesses.Add(p_ProcessNames.SelectedItem.ToString());
            await Navigation.PopModalAsync(); // close Modal
		}

		/**********************************************************************
        *********************************************************************/
		async void B_Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // close Modal
		}
    }
}