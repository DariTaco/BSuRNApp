﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace WertheApp
{
    public partial class WertheAppPage : ContentPage
    {
		StackLayout stackLayout;
		List<Button> buttonList;

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
			var bottomGrid = new Grid();
			bottomGrid.ColumnDefinitions = new ColumnDefinitionCollection {
            // Bottom half will be twice as big as top half:
			new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
			new ColumnDefinition{ Width = new GridLength(4, GridUnitType.Auto)},
			new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
			};

			//organize content in Stacklayout
			stackLayout = new StackLayout
            {
                Margin = new Thickness(20),
			};
			var l_pick = new Label 
            { 
                Text = "Pick your course",
				FontSize = App._h4FontSize,
				HorizontalOptions = LayoutOptions.Center

			};
            stackLayout.Children.Add(l_pick);

            var l_space = new Label();
			stackLayout.Children.Add(l_space);

			//add buttons for apps
			List<string> appNameList = new List<string>()    {
						"Computer Networks",
						"Operating Systems",
						"Digital Photography",
						"Embedded Systems",
						
					};

			buttonList = new List<Button>();
			foreach (string appName in appNameList)
			{
				var b_button = new Button
				{
					Text = appName,
					BackgroundColor = App._buttonBackground,
					TextColor = App._buttonText,
					CornerRadius = App._buttonCornerRadius,
					FontSize = App._buttonFontSize,
				};
				b_button.Clicked += Button_Clicked;
				stackLayout.Children.Add(b_button);
				buttonList.Add(b_button);

				// temporary fix to disable digital photography and embedded systems
				if(b_button.Text == "Digital Photography" || b_button.Text == "Embedded Systems")
                {
					b_button.IsEnabled = false;
				}

			}


			//add content to Toplevel grid
		
			bottomGrid.Children.Add(stackLayout, 1, 0);
			grid.Children.Add(bottomGrid, 0, 1);
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