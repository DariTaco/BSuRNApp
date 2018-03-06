using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq; //fragmentList.ElementAt(i);

using System.Diagnostics;
namespace WertheApp.BS
{
    public class PageReplacementStrategies : ContentPage
    {
        //VARIABLES
        public static List<int> sequenceList { get; set; }
        public static String strategy;
        public static int ram;
        public static int disc;


		bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;

		//CONSTRUCTOR
		public PageReplacementStrategies(List<int> l, String s, int r, int d)
        {
            sequenceList = l;
            strategy = s;
            ram = r;
            disc = d;

			/*Debug.WriteLine("##########");
            Debug.WriteLine("strategy: " + strategy);
            Debug.WriteLine("ram: " + ram);
            Debug.WriteLine("disc: "+ disc);
            Debug.WriteLine("seq: " + sequenceList.ElementAt(2));*/

			Title = "Page Replacement Strategies"; //since the name is longer than average, 
            //the button ahead will automatically be named "back" instead of "Betriebssysteme"

			//do only create content if device is rotated in landscape
			if (Application.Current.MainPage.Width > Application.Current.MainPage.Height)
			{
				CreateContent();
			}
			else
			{
				this.Content = new Label { Text = "please rotate your device" };
			}
        }

		//METHODS

		/**********************************************************************
        *********************************************************************/
		void CreateContent()
		{
			// This is the top-level grid, which will split our page in half
			var grid = new Grid();
			this.Content = grid;
			grid.RowDefinitions = new RowDefinitionCollection {
                    // Each half will be the same size:
                    new RowDefinition{ Height = new GridLength(4, GridUnitType.Star)},
					new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
				};
			CreateTopHalf(grid);
			CreateBottomHalf(grid);

			isContentCreated = true;
		}

		/**********************************************************************
        *********************************************************************/
		void CreateTopHalf(Grid grid)
		{
			var gameView = new CocosSharpView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};
			grid.Children.Add(gameView, 0, 0);
		}

		/**********************************************************************
        *********************************************************************/
		void CreateBottomHalf(Grid grid)
		{
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (3)
			//Screen Width -20 because Margin is 10
			double StackChildSize = (Application.Current.MainPage.Width - 20) / 3;

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            Button b_Reset_Rbits = new Button
            {
                Text = "Reset R-Bits",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Reset_Rbits.Clicked += B_Reset_Rbits_Clicked;
            stackLayout.Children.Add(b_Reset_Rbits);

            Button b_Set_Mbit = new Button
            {
                Text = "Set M-Bit",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Set_Mbit.Clicked += B_Set_Mbit_Clicked;
            stackLayout.Children.Add(b_Set_Mbit);

            Button b_Next = new Button
            {
                Text = "Next",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            stackLayout.Children.Add(b_Next);

			grid.Children.Add(stackLayout, 0, 1);
		}

		/**********************************************************************
        *********************************************************************/
		void B_Reset_Rbits_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		void B_Set_Mbit_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		void B_Next_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		/// <summary> deletes all content and informs the user to rotate the device </summary>
		void DeleteContent()
		{
			this.Content = null;
			this.Content = new Label { Text = "please rotate your device" };
			isContentCreated = false;
		}

		/**********************************************************************
        *********************************************************************/
		//sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
            PageReplacementStrategiesScene gameScene;

			var gameView = sender as CCGameView;
			if (gameView != null)
			{
				// This sets the game "world" resolution to 330x100:
				//Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
				gameView.DesignResolution = new CCSizeI(330, 100);
				// GameScene is the root of the CocosSharp rendering hierarchy:
				gameScene = new PageReplacementStrategiesScene(gameView);
				// Starts CocosSharp:
				gameView.RunWithScene(gameScene);
			}
		}

		/**********************************************************************
        *********************************************************************/
		//this method is called everytime the device is rotated
		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height); //must be called
			if (this.width != width || this.height != height)
			{
				this.width = width;
				this.height = height;
			}

			//reconfigure layout
			if (width > height && isContentCreated == false)
			{
				CreateContent();
			}
			else if (height > width && isContentCreated)
			{
				DeleteContent();
			}
		}

	}
}

