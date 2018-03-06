using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;

using System.Diagnostics;

namespace WertheApp.RN
{
    public class CongestionAvoidance : ContentPage
    {
        //VARIABLES
        public static int errorTreshold;
        public static int treshold;
        public static bool reno;
        public static bool tahoe;

		bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;

		//CONSTRUCTOR
		public CongestionAvoidance(int eth, int th, bool r, bool t)
        {

            errorTreshold = eth;
            treshold = th;
            reno = r;
            tahoe = t;

			/*Debug.WriteLine("##########");
            Debug.WriteLine("error Treshold: " + errorTreshold);
			Debug.WriteLine("treshold: " + treshold);
			Debug.WriteLine("reno: " + reno);
			Debug.WriteLine("tahoe: " + tahoe);*/

			Title = "Congestion Avoidance";

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

            Button b_Triple = new Button
            {
                Text = "Triple Duplicate Ack",
				WidthRequest = StackChildSize,
				VerticalOptions = LayoutOptions.Center
            };
            b_Triple.Clicked += B_Triple_Clicked;
            stackLayout.Children.Add(b_Triple);

            Button b_Set_ErrorValue = new Button
            {
                Text = "Set Error-Value",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Set_ErrorValue.Clicked += B_Set_ErrorValue_Clicked;
            stackLayout.Children.Add(b_Set_ErrorValue);

            Button b_Next = new Button
            {
                Text = "Next Step",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            stackLayout.Children.Add(b_Next);

			grid.Children.Add(stackLayout, 0, 1);
		}

		/**********************************************************************
        *********************************************************************/
		void B_Triple_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		void B_Set_ErrorValue_Clicked(object sender, EventArgs e)
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
			CongestionAvoidanceScene gameScene;

			var gameView = sender as CCGameView;
			if (gameView != null)
			{
				// This sets the game "world" resolution to 330x100:
				//Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
				gameView.DesignResolution = new CCSizeI(330, 100);
				// GameScene is the root of the CocosSharp rendering hierarchy:
				gameScene = new CongestionAvoidanceScene(gameView);
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

