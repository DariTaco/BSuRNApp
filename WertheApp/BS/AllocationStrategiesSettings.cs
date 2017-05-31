using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;

namespace WertheApp.BS
{
    public class AllocationStrategiesSettings : ContentPage
    {
        //VARIABLES
        Picker p_Algorithm; //has to be definded here instead of Constructor because value is also needed method
        Entry e_Fragmentation; //same as above
		List<int> fragmentsList; //will be given to the Constructor of AllocationStrategies

        //CONSTRUCTOR
        public AllocationStrategiesSettings()
        {
			Title = "Allocation Strategies";
            CreateContent();
        }

        //METHODS
        void CreateContent(){
            var stackLayout = new StackLayout
            {
                Margin = new Thickness(10)
            };
            this.Content = stackLayout;

            var l_Algorithm = new Label { Text = "Algorithm:" };
            p_Algorithm = new Picker { Title = "Select a Strategy" };
            p_Algorithm.Items.Add("First Fit");
            p_Algorithm.Items.Add("Next Fit");
            p_Algorithm.Items.Add("Best Fit");
            p_Algorithm.Items.Add("Worst Fit");
            p_Algorithm.Items.Add("Tailoring Best Fit");

            var l_Space = new Label { Text = " " };
            var l_Fragmentation = new Label { Text = "Fragmentation:" };
            e_Fragmentation = new Entry { Text = "10,4,20,18,7,9,12,15"};
            var b_Default = new Button { Text = "Default Value", HorizontalOptions = LayoutOptions.Start };
            b_Default.Clicked += (sender, e) => e_Fragmentation.Text= "10,4,20,18,7,9,12,15"; //add Click Event(so short, no Method needed)


            var l_Space2 = new Label { Text = " " };
            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(l_Algorithm);
            stackLayout.Children.Add(p_Algorithm);
            stackLayout.Children.Add(l_Space);
			stackLayout.Children.Add(l_Fragmentation);
			stackLayout.Children.Add(b_Default);
            stackLayout.Children.Add(e_Fragmentation);
            stackLayout.Children.Add(l_Space2);
			stackLayout.Children.Add(b_Start);
        }

        //reads the input String from e_Fragmentation and adds elements to a List
        void CreateFragmentsList(){
            String s = e_Fragmentation.Text;
            fragmentsList = new List<int>();
            String ss = "";
            //if a comma appears, the string ss will be converted to int and added to fragmentList
            //in the other case(digit appears), it will be added to ss
            //IMPORTANT: due to the funktion ValidateFragmentationInput, it is ensured that only commas and digits appear
			for (int i = 0; i < s.Length; i++)
			{
                if(s[i]==','){
                    int frag = Int32.Parse(ss);
                    fragmentsList.Add(frag);
                    ss = "";
                }else{ ss += s[i]; }
				
			}

        }

        //validates the string in e_Fragmentation. For example a Fragment with size 0 is not allowed.
        //returns true if string is valid 
        bool ValidateFragmentationInput(){
            String s = e_Fragmentation.Text;
            return Regex.IsMatch(s, "^[1-9]+[0-9]*(,[1-9]+[0-9]*)*$"); //matches only numbers(exept 0) separated by a comma;
		}

        //If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            //if a strategy was selected and a valid fragmentation was entered
            if (p_Algorithm.SelectedIndex != -1 && e_Fragmentation.Text != null && ValidateFragmentationInput())
            {
                CreateFragmentsList();

                await Navigation.PushAsync(new AllocationStrategies(fragmentsList, p_Algorithm.SelectedItem.ToString()));
            }
            else if (e_Fragmentation.Text == null) { await DisplayAlert("Alert", "fragmentation = null", "OK"); }//actually not needed. Case should never occur
            else if(!ValidateFragmentationInput()){ await DisplayAlert("Alert", "Please insert a valid fragmentation! (only digits, greater than zero, separated by a comma)", "OK");}
            else{ await DisplayAlert("Alert", "Please fill in all necessary information", "OK"); }
		}
    }
}

