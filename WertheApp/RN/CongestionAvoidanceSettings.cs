using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;

namespace WertheApp.RN
{
    public class CongestionAvoidanceSettings : ContentPage
    {
        //VARIABLES
        Picker p_Treshold;//has to be definded here instead of Constructor because value is also needed in method
		Picker p_ErrorTreshold;//same as above
        Switch s_Tahoe;//same
        Switch s_Reno;//same

		//CONSTRUCTOR
		public CongestionAvoidanceSettings()
		{
			Title = "Congestion Avoidance";
			CreateContent();
		}

		//METHODS

		/**********************************************************************
        *********************************************************************/
		void CreateContent()
		{
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10)
			};
			var stackLayout = new StackLayout();

			var stackLayout2 = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
                                              
			};

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            //add elements to stackLayout2
            var l_Tahoe = new Label { 
                VerticalOptions = LayoutOptions.Center,
                Text = "TCP Tahoe"};
            s_Tahoe = new Switch();
            s_Tahoe.IsToggled = true;
            s_Tahoe.Toggled += S_Tahoe_Toggled; //adds an event 

            var l_Space4 = new Label { HorizontalOptions = LayoutOptions.FillAndExpand, Text = " " };

            var l_Reno = new Label { 
                VerticalOptions = LayoutOptions.Center,
                Text = "TCP Reno" };
            s_Reno = new Switch();
            s_Reno.IsToggled = true;
            s_Reno.Toggled += S_Reno_Toggled; // adds an event 

            stackLayout2.Children.Add(l_Tahoe);
            stackLayout2.Children.Add(s_Tahoe);
            stackLayout2.Children.Add(l_Space4);
            stackLayout2.Children.Add(l_Reno);
            stackLayout2.Children.Add(s_Reno);

            //add elements to stacklayout
            var l_Choose = new Label { Text = "Choose strategies:" };
            var l_Space = new Label { Text = "  " };
            var l_Treshold = new Label { Text = "Initial treshold:" };
            p_Treshold = new Picker();
            p_Treshold.Items.Add("1");
            p_Treshold.Items.Add("2");
            p_Treshold.Items.Add("3");
            p_Treshold.Items.Add("4");
            p_Treshold.Items.Add("5");
            p_Treshold.Items.Add("6");
            p_Treshold.Items.Add("7");
            p_Treshold.Items.Add("8");
            p_Treshold.Items.Add("9");
            p_Treshold.Items.Add("10");
            p_Treshold.Items.Add("11");
            p_Treshold.Items.Add("12");
            p_Treshold.Items.Add("13");
            p_Treshold.Items.Add("14");
            p_Treshold.Items.Add("15");
            p_Treshold.SelectedIndex = 7; //8
            var l_Space2 = new Label { Text = "  " };
            var l_ErrorTreshold = new Label { Text = "Initial error treshold" };
            p_ErrorTreshold = new Picker();
            p_ErrorTreshold.Items.Add("1");
            p_ErrorTreshold.Items.Add("2");
            p_ErrorTreshold.Items.Add("3");
            p_ErrorTreshold.Items.Add("4");
            p_ErrorTreshold.Items.Add("5");
            p_ErrorTreshold.Items.Add("6");
            p_ErrorTreshold.Items.Add("7");
            p_ErrorTreshold.Items.Add("8");
            p_ErrorTreshold.Items.Add("9");
            p_ErrorTreshold.Items.Add("10");
            p_ErrorTreshold.Items.Add("11");
            p_ErrorTreshold.Items.Add("12");
            p_ErrorTreshold.Items.Add("13");
            p_ErrorTreshold.Items.Add("14");
            p_ErrorTreshold.Items.Add("15");
            p_ErrorTreshold.SelectedIndex = 11; //12
            var b_Default = new Button { Text = "Set default values" , HorizontalOptions = LayoutOptions.Start };
            b_Default.Clicked += B_Default_Clicked; //add Click Event(Method)
			var b_Start = new Button { Text = "Start" };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(l_Choose);
            stackLayout.Children.Add(stackLayout2);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(l_Treshold);
            stackLayout.Children.Add(p_Treshold);
            stackLayout.Children.Add(l_Space2);
            stackLayout.Children.Add(l_ErrorTreshold);
            stackLayout.Children.Add(p_ErrorTreshold);
            stackLayout.Children.Add(b_Default);
			stackLayout.Children.Add(b_Start);

		}

		/**********************************************************************
        *********************************************************************/
		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            await Navigation.PushAsync(new CongestionAvoidance(
                Int32.Parse(p_ErrorTreshold.SelectedItem.ToString()), 
                Int32.Parse(p_Treshold.SelectedItem.ToString()),
                s_Reno.IsToggled,
                s_Tahoe.IsToggled
            ));
		}

		/**********************************************************************
        *********************************************************************/
		//sets default values
		void B_Default_Clicked(object sender, EventArgs e)
        {
            s_Reno.IsToggled = true;
            s_Tahoe.IsToggled = true;
            p_ErrorTreshold.SelectedIndex = 11; //12
            p_Treshold.SelectedIndex = 7; //8
        }

		/**********************************************************************
        *********************************************************************/
		void S_Tahoe_Toggled(object sender, ToggledEventArgs e)
        {
            if(!s_Tahoe.IsToggled){
                s_Reno.IsToggled = true;
            }
        }

		/**********************************************************************
        *********************************************************************/
		void S_Reno_Toggled(object sender, ToggledEventArgs e)
        {
            if(!s_Reno.IsToggled){
                s_Tahoe.IsToggled = true;
            }
        }
    }
}

