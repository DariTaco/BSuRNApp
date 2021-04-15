using System;
using Xamarin.Forms;

namespace WertheApp.CN
{
    public class CongestionControlSettings : ContentPage
    {
        //VARIABLES
        Picker p_Treshold;//has to be definded here instead of Constructor because value is also needed in method
        Switch s_Tahoe;//same
        Switch s_Reno;//same

		//CONSTRUCTOR
		public CongestionControlSettings()
		{
			Title = "Congestion Control";

            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;
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
            var stackLayout3 = new StackLayout
            {
                Orientation = StackOrientation.Horizontal

            };
            this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            //add elements to stackLayout2
            var l_Tahoe = new Label { 
                VerticalOptions = LayoutOptions.Center,
                Text = "TCP Tahoe",
                TextColor = Color.Blue};
            s_Tahoe = new Switch();
            s_Tahoe.IsToggled = true;
            s_Tahoe.Toggled += S_Tahoe_Toggled; //adds an event 

            var l_Space4 = new Label { HorizontalOptions = LayoutOptions.FillAndExpand, Text = " " };

            var l_Reno = new Label { 
                VerticalOptions = LayoutOptions.Center,
                Text = "TCP Reno",
                TextColor = Color.Red};
            s_Reno = new Switch();
            s_Reno.IsToggled = true;
            s_Reno.Toggled += S_Reno_Toggled; // adds an event 

            stackLayout2.Children.Add(l_Tahoe);
            stackLayout2.Children.Add(s_Tahoe);
            stackLayout2.Children.Add(l_Space4);
            stackLayout2.Children.Add(l_Reno);
            stackLayout2.Children.Add(s_Reno);

            //add elements to stacklayout
            var l_Choose = new Label { Text = "Strategy", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            var l_Space = new Label { Text = "  " };
            var l_initialSetting = new Label { Text = "Initial Settings", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            var l_thresh = new Label { Text = "Threshold:", VerticalOptions = LayoutOptions.Center,
            };
            p_Treshold = new Picker();
            //p_Treshold.Items.Add("1");
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
            var b_Default = new Button { Text = "Default" , HorizontalOptions = LayoutOptions.Start,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
            b_Default.Clicked += B_Default_Clicked; //add Click Event(Method)
			var b_Start = new Button { Text = "Start",
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)
            var l_Space3 = new Label { Text = "  " };

            stackLayout.Children.Add(l_Choose);
            stackLayout.Children.Add(stackLayout2);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(l_initialSetting);
            stackLayout3.Children.Add(l_thresh);
            stackLayout3.Children.Add(p_Treshold);
            stackLayout.Children.Add(stackLayout3);
            stackLayout.Children.Add(l_Space2);
            stackLayout.Children.Add(b_Default);
            stackLayout.Children.Add(l_Space3);
			stackLayout.Children.Add(b_Start);

		}

		/**********************************************************************
        *********************************************************************/
		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            await Navigation.PushAsync(new CongestionControl(
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

        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CongestionControlHelp());
        }
    }
}