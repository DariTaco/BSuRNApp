using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;

namespace WertheApp.RN
{
    public class PipelineProtocolsSettings : ContentPage
    {
        //VARIABLES
        Picker p_WindowSize; //
        Picker p_Strategy;

		//CONSTRUCTOR
		public PipelineProtocolsSettings()
		{
			Title = "Pipeline Protocols";
			CreateContent();
		}

		//METHODS
		void CreateContent()
		{
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10)
			};
			var stackLayout = new StackLayout();

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            //add elements to stacklayout
            var l_Strategy = new Label { Text = "Strategy:" };
            p_Strategy = new Picker();
            p_Strategy.Items.Add("Go Back N");
            p_Strategy.Items.Add("Selective Repeat");
            p_Strategy.SelectedIndex = 0;//Go Back N
            var l_Space = new Label { Text = "  " };
            var l_WindowSize = new Label { Text = "Window size:" };
            p_WindowSize = new Picker();
            p_WindowSize.Items.Add("2");
            p_WindowSize.Items.Add("3");
            p_WindowSize.Items.Add("4");
            p_WindowSize.Items.Add("5");
            p_WindowSize.SelectedIndex = 3;//5
            var l_Space2 = new Label { Text = "  " };
			var b_Start = new Button { Text = "Start" };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(l_Strategy);
            stackLayout.Children.Add(p_Strategy);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(l_WindowSize);
            stackLayout.Children.Add(p_WindowSize);
            stackLayout.Children.Add(l_Space2);
			stackLayout.Children.Add(b_Start);
		}

		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            await Navigation.PushAsync(new PipelineProtocols(
                Int32.Parse(p_WindowSize.SelectedItem.ToString()),
                p_Strategy.SelectedItem.ToString()));
		}
    }
}

