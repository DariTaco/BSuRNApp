using System;
using Xamarin.Forms;

namespace WertheApp.OS
{
    public class BuddySystemSettings : ContentPage
    {
        //VARIABLES
        Picker p_Exponent; //has to be definded here instead of Constructor because value is also needed method
        Label l_AbsoluteMemorySize;
        double absoluteMemorySize = 32;

		//CONSTRUCTOR
		public BuddySystemSettings()
        {
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            Title = "Buddy System";
			CreateContent();
        }

		//METHODS

		/**********************************************************************
        *********************************************************************/
		void CreateContent(){
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10)
			};
			var stackLayout = new StackLayout();
            var stackLayout2 = new StackLayout() { Orientation = StackOrientation.Horizontal };

            this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            //add elements to stacklayout
            var l_MemorySize = new Label {
                FontSize = App._h3FontSize ,
                Text = "Memory Size" };
            var l_Exponent = new Label { Text = "Exponent: ", VerticalOptions = LayoutOptions.Center, FontSize = App._textFontSize };
            p_Exponent = new Picker() { FontSize = App._textFontSize };
            p_Exponent.Items.Add("1");
            p_Exponent.Items.Add("2");
            p_Exponent.Items.Add("3");
            p_Exponent.Items.Add("4");
            p_Exponent.Items.Add("5");
            p_Exponent.Items.Add("6");
            p_Exponent.Items.Add("7");
            p_Exponent.Items.Add("8");
            p_Exponent.Items.Add("9");
            p_Exponent.Items.Add("10");
            p_Exponent.Items.Add("11");
            p_Exponent.Items.Add("12");
            p_Exponent.Items.Add("13");
            p_Exponent.Items.Add("14");
            p_Exponent.Items.Add("15");//been to lazy to programm a loop.sorry
			p_Exponent.SelectedIndex = 4; //5
            p_Exponent.SelectedIndexChanged += P_Exponent_SelectedIndexChanged; //add method


            //TO DO: Fire Event when absoulute memorysize needs to be calculated
            l_AbsoluteMemorySize = new Label { Text = "Absoulute memory size: 32", FontSize = App._smallTextFontSize };
            var l_Space = new Label { Text = "  "};
            var b_Start = new Button { Text = "Start",
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(l_MemorySize);
            stackLayout2.Children.Add(l_Exponent);
            stackLayout2.Children.Add(p_Exponent);
            stackLayout.Children.Add(stackLayout2);
            stackLayout.Children.Add(l_AbsoluteMemorySize);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(b_Start);
		}

		/**********************************************************************
        *********************************************************************/
		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BuddySystem(Int32.Parse(p_Exponent.SelectedItem.ToString())));
        }

		/**********************************************************************
        *********************************************************************/
		//If an exponent was picked
		void P_Exponent_SelectedIndexChanged(object sender, EventArgs e){
            absoluteMemorySize = Math.Pow(2, Double.Parse(p_Exponent.SelectedItem.ToString())); //2ExponentX
            l_AbsoluteMemorySize.Text = "Absolute memory size : " + absoluteMemorySize;
        }
        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BuddySystemHelp());
        }
    }
}