using System;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemSettings : ContentPage
    {
        //VARIABLES
        Picker p_Exponent; //has to be definded here instead of Constructor because value is also needed method
        String absouluteMemorySize = "-";

		//CONSTRUCTOR
		public BuddySystemSettings()
        {
			Title = "Buddy System";
			CreateContent();
        }

        //METHODS
        void CreateContent(){
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10)
			};
			var stackLayout = new StackLayout();

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            //add elements to stacklayout
            var l_MemorySize = new Label { 
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), 
                Text = "Memory size" };
            var l_Exponent = new Label { Text = "Exponent:"};
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
            p_Exponent.Items.Add("15");
            p_Exponent.SelectedIndex = 4;
            //been to lazy to programm aloop.sorry

            //TO DO: Fire Event when absoulute memorysize needs to be calculated
            var l_AbsoluteMemorySize = new Label { Text = "Absoulute memory size: " + absouluteMemorySize };
            var l_Space = new Label { Text = "  " };
            var b_Start = new Button { Text = "Start" };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(l_MemorySize);
            stackLayout.Children.Add(l_Exponent);
            stackLayout.Children.Add(p_Exponent);
            stackLayout.Children.Add(l_AbsoluteMemorySize);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(b_Start);
		}

        //If Button Start is clicked
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BuddySystem());
        }
    }
}

