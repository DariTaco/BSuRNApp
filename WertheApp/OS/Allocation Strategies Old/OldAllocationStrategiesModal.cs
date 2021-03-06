﻿using System;
using System.Text.RegularExpressions; //Regex.IsMatch
using Xamarin.Forms; //Messaging Center

namespace WertheApp.OS
{
    public class OldAllocationStrategiesModal : ContentPage
    {
        //VARIABLES
        String availableMemory = "Free memory: ";
        Entry e_MemoryRequest; //has to be definded here instead of Constructor because it's used in a method

		//CONSTRUCTOR
		public OldAllocationStrategiesModal()
		{

            //put all memory blocks from previous page into a String
            for (int i = 0; i < OldAllocationStrategiesScene.memoryBlocks.GetLength(0); i++){
                if(i == OldAllocationStrategiesScene.memoryBlocks.GetLength(0)-1){
                    availableMemory += OldAllocationStrategiesScene.memoryBlocks[i,0] + " ";
                }else{
                    availableMemory += OldAllocationStrategiesScene.memoryBlocks[i,0] + ", ";
                }
            }

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

            var l_Instructions = new Label { Text = "Enter memory request:" };
            e_MemoryRequest = new Entry { Keyboard = Keyboard.Numeric }; //only numbers are allowed
			var b_OK = new Button { Text = "OK",
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius
            };
            b_OK.Clicked += B_OK_Clicked;
			var l_AvailableMemory = new Label { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), 
                Text = availableMemory };


            stackLayout.Children.Add(l_Instructions);
            stackLayout.Children.Add(e_MemoryRequest);
            stackLayout.Children.Add(b_OK);
            stackLayout.Children.Add(l_AvailableMemory);

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content
		}

		/**********************************************************************
        *********************************************************************/
		//validates the string in e_MemoryRequest. For example a request with size 0 is not allowed.
		//returns true if string is valid 
		bool ValidateMemoryRequestInput()
		{
			String s = e_MemoryRequest.Text;
            //			return Regex.IsMatch(s, "[1-9]+[0-9]*$"); //matches only numbers(exept 0);

            return Regex.IsMatch(s, @"^[1-9]\d*$"); //matches only numbers(exept 0);
		}

		/**********************************************************************
        *********************************************************************/
		//If Button Start is clicked
		async void B_OK_Clicked(object sender, EventArgs e)
        {
            if (e_MemoryRequest.Text != null && ValidateMemoryRequestInput())
            {
                OldAllocationStrategies.memoryRequest = Int32.Parse(e_MemoryRequest.Text);
				MessagingCenter.Send<OldAllocationStrategiesModal>(this, "new memory request");// inform all subscribers
				await Navigation.PopModalAsync(); // close Modal
            }
            else if (e_MemoryRequest.Text == null) { await DisplayAlert("Alert", "Please enter size of memory request", "OK"); }
            else if (!ValidateMemoryRequestInput()) { await DisplayAlert("Alert", "Please enter a valid memory request (only integers > 0)", "OK"); }
        }
    }
}