using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics; //Debug.WriteLine("");
using System.Text.RegularExpressions; ////Regex.IsMatch();
using Xamarin.Forms;

namespace WertheApp.BS
{
    public class PageReplacementStrategiesSettings : ContentPage
    {
        //VARIABLES
        Picker p_Strategy;//has to be definded here instead of Constructor because value is also needed in method
		Picker p_RAM;//same
        Picker p_DISC;//same
        Entry e_Sequence;//same
        List<int> sequenceList; //will be given to the Constructor

		//CONSTRUCTOR
		public PageReplacementStrategiesSettings()
		{
			Title = "Page Replacement Strategies";
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
            var l_Zero = new Label { Text = "0" ,VerticalOptions = LayoutOptions.Center };
            e_Sequence = new Entry { Text = "12340156012356" ,HorizontalOptions = LayoutOptions.FillAndExpand };


			stackLayout2.Children.Add(l_Zero);
			stackLayout2.Children.Add(e_Sequence);

            //add elements to stackLayout3
			var l_RAM = new Label { Text = "RAM:", VerticalOptions = LayoutOptions.Center };
			p_RAM = new Picker();
            p_RAM.Items.Add("1");
            p_RAM.Items.Add("2");
			p_RAM.Items.Add("3");
			p_RAM.Items.Add("4");
			p_RAM.Items.Add("5");
			p_RAM.Items.Add("6");
			p_RAM.Items.Add("7");
            p_RAM.SelectedIndex = 2; //"3"
			var l_Space3 = new Label { Text = "  " };
			var l_DISC = new Label { Text = "DISC:", VerticalOptions = LayoutOptions.Center };
			p_DISC = new Picker();
            p_DISC.Items.Add("1");
            p_DISC.Items.Add("2");
            p_DISC.Items.Add("3");
            p_DISC.Items.Add("4");
            p_DISC.Items.Add("5");
            p_DISC.Items.Add("6");
            p_DISC.Items.Add("7");
            p_DISC.SelectedIndex = 3; //"4"

			stackLayout3.Children.Add(l_RAM);
			stackLayout3.Children.Add(p_RAM);
			stackLayout3.Children.Add(l_Space3);
			stackLayout3.Children.Add(l_DISC);
			stackLayout3.Children.Add(p_DISC);

            //add elements to StackLayout
            var l_Strategy = new Label { Text = "Strategy:" };
			p_Strategy = new Picker { Title = "Select a Strategy" };
			p_Strategy.Items.Add("Optimal Strategy");
			p_Strategy.Items.Add("FIFO");
			p_Strategy.Items.Add("FIFO Second Chance");
			p_Strategy.Items.Add("RNU FIFO");
			p_Strategy.Items.Add("RNU FIFO Second Chance");
            p_Strategy.SelectedIndex = 0; //"Optimal Strategy"
            var l_Space = new Label { Text = "  " };
            var l_Sequence = new Label { Text = "Reference sequence:" };
            var b_DefaultValue = new Button { Text = "Default Value", HorizontalOptions = LayoutOptions.Start };
            b_DefaultValue.Clicked += B_DefaultValue_Clicked;
            var l_Space2 = new Label { Text = "  " };
            var l_MemorySize = new Label { Text = "Memory size:" };
            var l_MaxSize = new Label{
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                Text = "Maximal size of RAM and DISC together: 8"};
            var l_Space4 = new Label { Text = "  " };
			var b_Start = new Button { Text = "Start" };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

			stackLayout.Children.Add(l_Strategy);
            stackLayout.Children.Add(p_Strategy);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(l_Sequence);
            stackLayout.Children.Add(b_DefaultValue);
            stackLayout.Children.Add(stackLayout2); //Stacklayout2 is nested 
            stackLayout.Children.Add(l_Space2);
            stackLayout.Children.Add(l_MemorySize);
            stackLayout.Children.Add(stackLayout3);//Stacklayout3 is nested
            stackLayout.Children.Add(l_MaxSize);
            stackLayout.Children.Add(l_Space4);
            stackLayout.Children.Add(b_Start);
            //note : a space label element can somehow only be added once, therefore I needed to define 4 of them
		}

		/**********************************************************************
        *********************************************************************/
		void CreateSequenceList(){
            String s = e_Sequence.Text;
            sequenceList = new List<int>();
            sequenceList.Add(0); //leading zero in settings
            for(int i = 0; i < s.Length; i++){
                sequenceList.Add(Int32.Parse(s[i].ToString()));
			}
        }

		/**********************************************************************
        *********************************************************************/
		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            if (ValidateSequenceInput())
            {
                if(ValidateRAMandDISC())
                {
                    CreateSequenceList();
                    await Navigation.PushAsync(new PageReplacementStrategies(sequenceList, 
                                                                             p_Strategy.SelectedItem.ToString(), 
                                                                             Int32.Parse(p_RAM.SelectedItem.ToString()), 
                                                                             Int32.Parse(p_DISC.SelectedItem.ToString())
                                                                            )); 

                }
                else
                {
                    await DisplayAlert("Alert", "RAM and DISC together must be equal or smaller than 8", "OK");
                }

            }else{
                await DisplayAlert("Alert", "Please enter a valid reference sequence", "OK");
            }
		}

		/**********************************************************************
        *********************************************************************/
		//Apply default value for Reference Sequence
		void B_DefaultValue_Clicked(object sender, EventArgs e)
        {
            e_Sequence.Text = "12340156012356";
        }

		/**********************************************************************
        *********************************************************************/
		//validates the string in e_Sequence. For example a Fragment with size 0 is not allowed.
		//returns true if string is valid 
		bool ValidateSequenceInput()
		{
			String s = e_Sequence.Text;
			return Regex.IsMatch(s, "^[0-9]*$"); //matches only a sequence of numbers
		}

		/**********************************************************************
        *********************************************************************/
		//validates if the sum of ram and disc together is smaller than eight
		bool ValidateRAMandDISC(){
            int ram = Int32.Parse(p_RAM.SelectedItem.ToString());
            int disc = Int32.Parse(p_DISC.SelectedItem.ToString());

            return ram + disc <= 8;
        }
    }
}