/********************CLASS FOR START PROCESS POP UP****************************/
using System;
using System.Text.RegularExpressions; //Regex.IsMatch
using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemModal : ContentPage
    {
        //VARIABLES
        Picker p_ProcessNames;
        Entry e_ProcessSize;
      
        //CONSTRUCTOR
        public BuddySystemModal()
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

			var l_Available = new Label { Text = "Available process names:" };
			p_ProcessNames = new Picker(); 
            p_ProcessNames.Items.Add("TESTNAME");
            p_ProcessNames.SelectedIndex = 0;
            var l_Space = new Label { Text = "  " };
            var l_ProcessSize = new Label { Text = "Process size:" };
            e_ProcessSize = new Entry();
            var l_Min = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), 
                Text = "Minimum process size: 2" };
			var b_Start = new Button { Text = "Start process" };
            b_Start.Clicked += B_Start_Clicked;


            stackLayout.Children.Add(l_Available);
            stackLayout.Children.Add(p_ProcessNames);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(l_ProcessSize);
            stackLayout.Children.Add(e_ProcessSize);
            stackLayout.Children.Add(l_Min);
            stackLayout.Children.Add(b_Start);

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content
		}

		/**********************************************************************
        *********************************************************************/
		//validates the string in e_ProcessSize. For example a request with size <2 is not allowed.
		//returns true if string is valid 
		bool ValidateMemoryRequestInput()
		{
            String s = e_ProcessSize.Text;
            return Regex.IsMatch(s, "^(?:[2-9]|[1-9]+[0-9]+)$"); //matches only numbers >= 2;
		}

		/**********************************************************************
        *********************************************************************/
		async void B_Start_Clicked(object sender, EventArgs e)
        {
            if (e_ProcessSize.Text != null && ValidateMemoryRequestInput())
            {
                BuddySystem.startedProcessName = p_ProcessNames.SelectedItem.ToString();
                BuddySystem.startedProcessSize = Int32.Parse(e_ProcessSize.Text);
                BuddySystemViewCell a = new BuddySystemViewCell();
                BuddySystem.AddBuddySystemCell(a);
                MessagingCenter.Send<BuddySystemModal>(this, "new process started");// inform all subscribers
                await Navigation.PopModalAsync(); // close Modal
            }
            else if (e_ProcessSize.Text == null){await DisplayAlert("Alert", "Please enter a process size", "OK");}
            else if (!ValidateMemoryRequestInput()){await DisplayAlert("Alert", "Please enter a valid process size (only integers >= 2)", "OK");}
		}
    }

}