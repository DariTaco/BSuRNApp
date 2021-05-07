﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace WertheApp
{
    public partial class WertheAppPage : ContentPage
    {
		
        //CONSTRUCTOR
        public WertheAppPage()
        {
            InitializeComponent();

			ToolbarItem info = new ToolbarItem();
			info.Text = "Info";
			this.ToolbarItems.Add(info);
			info.Clicked += B_Info_Clicked;

			Title = "WertheApp";
			//Title = "Start Screen"

			// This is the top-level grid, which will split our page in half
			var grid = new Grid();
			var scrollView = new ScrollView();
			scrollView.Content = grid; //Wrap ScrollView around StackLayout to be able to scroll the content
			this.Content = scrollView;
			grid.RowDefinitions = new RowDefinitionCollection {
            // Bottom half will be twice as big as top half:
            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
			new RowDefinition{ Height = new GridLength(2, GridUnitType.Star)},
            };
			CreateTopHalf(grid);
			CreateBottomHalf(grid);
        }


		//VARIABLES
		private double width = 0;
		private double height = 0;
        Image i_hsLogo = new Image { Aspect = Aspect.AspectFit, Margin = new Thickness(10)};


		//METHODS
		//this method is called everytime the device is rotated
		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height); //must be called
			if (this.width != width || this.height != height)
			{
				this.width = width;
				this.height = height;

				//reconfigure layout
				if (width < height)
				{
					i_hsLogo.Source = ImageSource.FromResource("AalenHS2.png");
				}
				else
				{
					i_hsLogo.Source = ImageSource.FromResource("AalenHS1.png");
				}
			}
		}

		void CreateTopHalf(Grid grid)
		{
            //add content to Toplevel grid
            grid.Children.Add(i_hsLogo, 0, 0);
		}
		void CreateBottomHalf(Grid grid)
		{
            //organize content in Stacklayout
            var stackLayout = new StackLayout
            {
                Margin = new Thickness(20),
				Orientation = StackOrientation.Vertical
			};
			var sL = new StackLayout
			{
				Margin = new Thickness(20),
				Orientation = StackOrientation.Horizontal


			};
			var sL2 = new StackLayout
			{
				Margin = new Thickness(20),
				Orientation = StackOrientation.Horizontal

			};

			var l_pick = new Label 
            { 
                Text = "Pick your course",
				FontSize = App._H4FontSize

			};
            stackLayout.Children.Add(l_pick);

            var l_space = new Label();
			stackLayout.Children.Add(l_space);

			//add buttons for apps
			List<string> appNameList = new List<string>()    {
						"Computer Networks",
						"Digital Photography",
						"Embedded Systems",
						"Operating Systems"
					};
			int count = 0;
			foreach (string appName in appNameList)
			{
				var b_button = new Button
				{
					Text = appName,
					BackgroundColor = App._buttonBackground,
					TextColor = App._buttonText,
					CornerRadius = App._buttonCornerRadius,
					FontSize = App._buttonFontSize,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand

				};
				b_button.Clicked += Button_Clicked;
                if (count % 2 == 0)
                {
					sL.Children.Add(b_button);
                }
                else
                {
					sL2.Children.Add(b_button);
                }
				count++;
			}

			stackLayout.Children.Add(sL);
			stackLayout.Children.Add(sL2);

			//add content to Toplevel grid
			grid.Children.Add(stackLayout, 0, 1);
		}

		async void Button_Clicked(object sender, System.EventArgs e)
		{
			var button = (sender as Button);
			string appName = button.Text;

			switch (appName)
			{
				case "Computer Networks":
					await Navigation.PushAsync(new ComputerNetworks());
					break;
				case "Digital Photography":
					await Navigation.PushAsync(new DigitalPhotography());
					break;
				case "Embedded Systems":
					await Navigation.PushAsync(new EmbeddedSystems());
					break;
				case "Operating Systems":
					await Navigation.PushAsync(new OperatingSystems());
					break;
			}
		}
		async void B_Info_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Info());
		}
    }
}