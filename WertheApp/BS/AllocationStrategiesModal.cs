using System;
using System.Text.RegularExpressions; //Regex.IsMatch

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class AllocationStrategiesModal : ContentPage
    {
        //VARIABLES
        String availableMemory = "Free memory: ";
        Entry e_MemoryRequest; //has to be definded here instead of Constructor because it's used in a method

		//CONSTRUCTOR
		public AllocationStrategiesModal()
		{
            //put all memory blocks from previous page into a String
            for (int i = 0; i < AllocationStrategies.memoryBlocks.Length; i++){
                if(i == AllocationStrategies.memoryBlocks.Length-1){
                    availableMemory += AllocationStrategies.memoryBlocks[i] + " ";
                }else{
                    availableMemory += AllocationStrategies.memoryBlocks[i] + ", ";
                }
            }
			//Title = "";
			CreateContent();
           
		}

		//METHODS
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
			var b_OK = new Button { Text = "OK" };
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

		//validates the string in e_MemoryRequest. For example a request with size 0 is not allowed.
		//returns true if string is valid 
		bool ValidateMemoryRequestInput()
		{
			String s = e_MemoryRequest.Text;
			return Regex.IsMatch(s, "^[1-9]+[0-9]*$"); //matches only numbers(exept 0);
		}

        //If Button Start is clicked
        async void B_OK_Clicked(object sender, EventArgs e)
        {
            if (e_MemoryRequest.Text != null && ValidateMemoryRequestInput())
            {
                AllocationStrategies.memoryRequest = Int32.Parse(e_MemoryRequest.Text);
				MessagingCenter.Send<AllocationStrategiesModal>(this, "new memory request");// inform all subscribers
				await Navigation.PopModalAsync(); // close Modal
            }
            else if (e_MemoryRequest.Text == null) { await DisplayAlert("Alert", "Please enter size of memory request", "OK"); }
            else if (!ValidateMemoryRequestInput()) { await DisplayAlert("Alert", "Please enter a valid memory request (only integers > 0)", "OK"); }
        }
    }
}

