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
        double StackChildSize;
        public static List<int> sequenceList { get; set; }
        public static String strategy;
        public static int ram;
        public static int disc;

        bool isContentCreated; //indicates weather the Content of the page was already created
        bool landscape = false; //indicates device orientation

		private double width = 0;
		private double height = 0;
        public int gameviewWidth;
        public int gameviewHeight;

        public static ScrollView scrollview;
        public static Button b_Reset_Rbits;
        public static Button b_Set_Mbit;


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

            isContentCreated = false;

            //if orientation Horizontal
            if (Application.Current.MainPage.Width < Application.Current.MainPage.Height)
            {
                landscape = false;
                CreateContent();
                this.Content.IsVisible = false;
            }
            //if orientation Landscape
            else
            {
                landscape = true;
                CreateContent();
            }

            b_Set_Mbit.IsEnabled = false;
            b_Reset_Rbits.IsEnabled = false;
        }

		//METHODS
        /**********************************************************************
        *********************************************************************/
        //Gets called everytime the Page is not shown anymore. For example when clicking the back navigation
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

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
		}

		/**********************************************************************
        *********************************************************************/
		void CreateTopHalf(Grid grid)
        {
            scrollview = new ScrollView();

            double scaleFactor;
            double desiredGameViewHeight;
            if (landscape)
            {
                gameviewWidth = (int)Application.Current.MainPage.Width;
                gameviewHeight = (int)Application.Current.MainPage.Height;

                scaleFactor = gameviewWidth/ 200;
                desiredGameViewHeight = gameviewHeight;
            }
            else
            {
                gameviewWidth = (int)Application.Current.MainPage.Height;
                gameviewHeight = (int)Application.Current.MainPage.Width;

                scaleFactor = gameviewHeight/ 200;
                desiredGameViewHeight = gameviewWidth;
            }

			var gameView = new CocosSharpView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Red,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};

          

            Debug.WriteLine("GAMMEVIEW WIDTH: " + gameviewWidth + " GAMEVIEW HEIGHT:" + gameviewHeight);
            //gameView.WidthRequest = (int)Application.Current.MainPage.Width;
            gameView.HeightRequest = desiredGameViewHeight *scaleFactor;
            //gameView.HeightRequest = gameviewHeight * scaleFactor; // SCROLLING!!!!!!!!!!!!!!!!
            scrollview.Content = gameView;
            grid.Children.Add(scrollview, 0, 0);
		}

		/**********************************************************************
        *********************************************************************/
		void CreateBottomHalf(Grid grid)
		{
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (3)
			//Screen Width -20 because Margin is 10
            if (!landscape)
            {
                StackChildSize = (Application.Current.MainPage.Height -20)/ 3;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width -20) / 3;
            }

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            b_Reset_Rbits = new Button
            {
                Text = "Reset R-Bits",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Reset_Rbits.Clicked += B_Reset_Rbits_Clicked;
            stackLayout.Children.Add(b_Reset_Rbits);

            b_Set_Mbit = new Button
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
            switch (strategy)
            {
                case "Optimal Strategy":
                    PageReplacementStrategiesScene.Optimal();
                    break;
                case "FIFO":
                    PageReplacementStrategiesScene.Fifo();
                    break;
                case "FIFO Second Chance":
                    PageReplacementStrategiesScene.FifoSecond();
                    break;
                case "RNU FIFO":
                    PageReplacementStrategiesScene.Rnu();
                    break;
                case "RNU FIFO Second Chance":
                    PageReplacementStrategiesScene.RnuSecond();
                    break;
            }
        }

		/**********************************************************************
        *********************************************************************/
		//sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
           // gameviewWidth = (int)Application.Current.MainPage.Width;
            //gameviewHeight = (int)(Application.Current.MainPage.Height / 5) * 4;

            PageReplacementStrategiesScene gameScene;

			var gameView = sender as CCGameView;
			if (gameView != null)
			{
				// This sets the game "world" resolution to 200x400:
				//Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
                gameView.DesignResolution = new CCSizeI(200,400);
                Debug.WriteLine("gameviewWidth: " + gameviewWidth + " GameviewHeight: " + gameviewHeight);
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
            if (width > height)
            {
                //isContentCreated = true;
                this.Content.IsVisible = true;
            }
            else if (height > width)
            {
                this.Content.IsVisible = false;
            }
		}

	}
}