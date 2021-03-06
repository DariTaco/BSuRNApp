﻿using System;
using System.Text.RegularExpressions; //Regex.IsMatch();
using Xamarin.Forms;
using System.Collections.Generic; //List<int>

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategiesSettings : ContentPage
    {
        //VARIABLES
        // controls
        private Xamarin.Forms.Picker p_Algorithm; 
        private Xamarin.Forms.Entry e_Fragmentation;

        //CONSTRUCTOR
        public AllocationStrategiesSettings()
        {
            //Help Button top right corner
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            Title = "Allocation Strategies";

            // content starts only after notch
            On<iOS>().SetUseSafeArea(true);

            CreateContent();

        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            // View
            var scrollView = new Xamarin.Forms.ScrollView
            {
                Margin = new Thickness(10)
            };
            var stackLayout = new StackLayout();
            this.Content = scrollView;
            scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            // Algorithm selection
            var l_Algorithm = new Label { Text = "Algorithm", FontSize = App._h3FontSize };
            p_Algorithm = new Xamarin.Forms.Picker { Title = "Select a Strategy", FontSize = App._textFontSize };
            p_Algorithm.Items.Add("First Fit");
            p_Algorithm.Items.Add("Next Fit");
            p_Algorithm.Items.Add("Best Fit");
            p_Algorithm.Items.Add("Worst Fit");
            p_Algorithm.Items.Add("Combined Fit");
            p_Algorithm.SelectedIndex = 0; //pre selects First Fit
            var l_Space = new Label { Text = " " };
            stackLayout.Children.Add(l_Algorithm);
            stackLayout.Children.Add(p_Algorithm);
            stackLayout.Children.Add(l_Space);

            // Fragmentation input
            var l_Fragmentation = new Label { Text = "Fragmentation", FontSize = App._h3FontSize  };
            e_Fragmentation = new Xamarin.Forms.Entry { Keyboard = Keyboard.Telephone, Text = "10,4,20,18,7,9,12,15", FontSize = App._textFontSize };
            var b_Default = new Button
            {
                Text = "Default",
                HorizontalOptions = LayoutOptions.Start,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._smallButtonFontSize

            };
            b_Default.Clicked += (sender, e) => e_Fragmentation.Text = "10,4,20,18,7,9,12,15";
            var l_Space2 = new Label { Text = " " };
            stackLayout.Children.Add(l_Fragmentation);
            stackLayout.Children.Add(e_Fragmentation);
            stackLayout.Children.Add(b_Default);
            stackLayout.Children.Add(l_Space2);

            // Application Start
            var b_Start = new Button
            {
                Text = "Start",
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
            b_Start.Clicked += B_Start_Clicked;
            stackLayout.Children.Add(b_Start);

        }

        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllocationStrategiesHelp());
        }

        /**********************************************************************
       *********************************************************************/
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            //if a strategy was selected and a valid fragmentation was entered
            if (p_Algorithm.SelectedIndex != -1 && e_Fragmentation.Text != null && ValidateFragmentationInput()&&ValidateMaxMemorySize())
            {
                
                await Navigation.PushAsync(new AllocationStrategies(
                    p_Algorithm.SelectedItem.ToString(),
                    CreateAllFragmentsList()
                    ));
            }

            //actually not needed. Case should never occur
            else if (e_Fragmentation.Text == null) { await DisplayAlert("Alert", "fragmentation = null", "OK"); }

            else if (!ValidateFragmentationInput()) { await DisplayAlert("Alert", "Please insert a valid fragmentation! (only digits, greater than zero, separated by a comma)", "OK"); }

            else if (!ValidateMaxMemorySize()) { await DisplayAlert("Alert", "Sum of all fragments must be <= 125", "OK"); }

            else { await DisplayAlert("Alert", "Please fill in all necessary information", "OK"); }
        }

        /**********************************************************************
        ***********************************************************************
        validates the string in e_Fragmentation. For example a Fragment with size 0 is not allowed.
        returns true if string is valid*/ 
        bool ValidateFragmentationInput()
        {
            String s = e_Fragmentation.Text;
            return Regex.IsMatch(s, "^[1-9]+[0-9]*(,[1-9]+[0-9]*)*$"); //matches only numbers(exept 0) separated by a comma;
        }

        /**********************************************************************
        ***********************************************************************
        validates if the memory size is less or equal than 125*/
        bool ValidateMaxMemorySize()
        {

            List<FragmentBlock> allFragmentsList = CreateAllFragmentsList();
            int memorySize = 0;
            foreach(FragmentBlock fb in allFragmentsList)
            {
                memorySize += fb.GetSize();
            }
          
            return memorySize <= 125;

        }

        /**********************************************************************
        ***********************************************************************
        reads the input String from e_Fragmentation and adds elements to a List*/
        private List<FragmentBlock> CreateAllFragmentsList()
        {
            String s = e_Fragmentation.Text;
            List<FragmentBlock> allFragmentsList = new List<FragmentBlock>();
            String ss = "";

            /*if a comma appears, the string ss will be converted to int and added to fragmentList
            in the other case(digit appears), it will be added to ss
            IMPORTANT: due to the function ValidateFragmentationInput, it is ensured that only commas and digits appear*/
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ',')
                {
                    int fragmentBlockSize = Int32.Parse(ss);
                    allFragmentsList.Add(new FragmentBlock(fragmentBlockSize, true, i - fragmentBlockSize)); // add free fragment
                    allFragmentsList.Add(new FragmentBlock(1, false, i)); // add used fragment

                    ss = "";
                }
                else if (i == s.Length - 1)
                { //if end of String is reached
                    ss += s[i];
                    int fragmentBlockSize = Int32.Parse(ss);
                    allFragmentsList.Add(new FragmentBlock(fragmentBlockSize, true, i - fragmentBlockSize)); // add free fragment

                    ss = "";
                }
                else { ss += s[i]; }
            }

            return allFragmentsList;
        }

    }
}