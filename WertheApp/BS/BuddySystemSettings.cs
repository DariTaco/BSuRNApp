using System;
using Xamarin.Forms;

namespace WertheApp.BS
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
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), 
                Text = "Memory Size" };
            var l_Exponent = new Label { Text = "Exponent: ", VerticalOptions = LayoutOptions.Center };
            p_Exponent = new Picker();
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
            l_AbsoluteMemorySize = new Label { Text = "Absoulute memory size: 32"};
            var l_Space = new Label { Text = "  "};
            var b_Start = new Button { Text = "Start" };
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
            if (!IsLandscape())
            {
                await DisplayAlert("Alert", "Please hold your phone horizontally for landscape mode", "OK");
            }
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
        static bool IsLandscape()
        {
            bool isLandscape = false;
            if (Application.Current.MainPage.Width > Application.Current.MainPage.Height)
            {
                isLandscape = true;
            }
            else
            {
                isLandscape = false;
            }
            return isLandscape;
        }
    }
}