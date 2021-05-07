using System;
using Xamarin.Forms;

namespace WertheApp.CN
{
    public class PipelineProtocolsSettings : ContentPage
    {
        //VARIABLES
        PipelineProtocols pipelineProtocols;
        Picker p_WindowSize; //
        Picker p_Strategy;
        Slider s_Timeout;
        Label l_TimeoutValue;
        public static double stepValue = 1.0;

		//CONSTRUCTOR
		public PipelineProtocolsSettings()
		{
			Title = "Pipeline Protocols";

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

            //define and add elements to stacklayout
            var l_Strategy = new Label { Text = "Strategy", FontSize = App._h3FontSize };
            p_Strategy = new Picker() { FontSize = App._textFontSize };
            p_Strategy.Items.Add("Go Back N");
            p_Strategy.Items.Add("Selective Repeat");
            p_Strategy.SelectedIndex = 0;//Go Back N
            var l_Space = new Label { Text = "  " };

            var l_WindowSize = new Label { Text = "Window Size", FontSize = App._h3FontSize };
            p_WindowSize = new Picker() { FontSize = App._textFontSize };
            p_WindowSize.Items.Add("2");
            p_WindowSize.Items.Add("3");
            p_WindowSize.Items.Add("4");
            p_WindowSize.Items.Add("5");
            p_WindowSize.SelectedIndex = 3;//5
            var l_Space2 = new Label { Text = "  " };

            var l_Timeout = new Label { Text = "Timeout =", FontSize = App._h3FontSize };
            l_TimeoutValue = new Label { Text = "11" , VerticalOptions = LayoutOptions.Center, FontSize = App._textFontSize };
            s_Timeout = new Slider(11, 20, 11); //min, max, val
            s_Timeout.HorizontalOptions = LayoutOptions.FillAndExpand;
            s_Timeout.ValueChanged += S_Timeout_ValueChanged;;
            //s_Timeout.Minimum = 0.0f; //minimum roundtrip time
            //s_Timeout.Maximum = 10.0f; //maximum roundtrip time
            var l_TimeoutMi = new Label { VerticalOptions = LayoutOptions.Center,
                FontSize = App._textFontSize,
                Text = "11" };
            var l_TimeoutMa = new Label { VerticalOptions = LayoutOptions.Center,
                FontSize = App._textFontSize, 
                Text = "20" };
            var l_RoundtripTime = new Label {
                FontSize = App._smallTextFontSize, 
                Text = "Round Trip Time: 10" };
            var l_Space3 = new Label { Text = "  " };

			var b_Start = new Button { Text = "Start",
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
			b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout3.Children.Add(l_Timeout);
            stackLayout3.Children.Add(l_TimeoutValue);

			stackLayout2.Children.Add(l_TimeoutMi);
			stackLayout2.Children.Add(s_Timeout);
            stackLayout2.Children.Add(l_TimeoutMa);

            stackLayout.Children.Add(l_Strategy);
            stackLayout.Children.Add(p_Strategy);
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(l_WindowSize);
            stackLayout.Children.Add(p_WindowSize);
            stackLayout.Children.Add(l_Space2);
            stackLayout.Children.Add(stackLayout3);
            stackLayout.Children.Add(stackLayout2);
            stackLayout.Children.Add(l_RoundtripTime);
            stackLayout.Children.Add(l_Space3);
			stackLayout.Children.Add(b_Start);
		}

		/**********************************************************************
        *********************************************************************/
		//If slider is moved
		void S_Timeout_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / stepValue);
            double val = newStep * stepValue;
            s_Timeout.Value = val;
            l_TimeoutValue.Text = "" + val;
        }

		/**********************************************************************
        *********************************************************************/
		//If Button Start is clicked
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            pipelineProtocols = new PipelineProtocols(
                Int32.Parse(p_WindowSize.SelectedItem.ToString()),
                p_Strategy.SelectedItem.ToString(), Int32.Parse(s_Timeout.Value.ToString()));
            await Navigation.PushAsync(pipelineProtocols);
		}

        /**********************************************************************
        *********************************************************************/
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GC.Collect();
        }
        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PipelineProtocolsHelp());
        }
    }
}