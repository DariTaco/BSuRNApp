using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace WertheApp.RN
{
    public class PipelineProtocols : ContentPage
    {
        //VARIABLES
        PipelineProtocolsScene gameScene;
        PipelineProtocolsScene2 gameScene2;

        public static int windowSize;
        public static String strategy;
        public static int timeoutTime;

        public static bool paused = false;
        public static CocosSharpView gameView;
        Button b_Stop;

        bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;

        public static Label l_LastRecentInOrderAtReceiver;
        public static Label l_LastRecentAcknowlegement;
        public static Label l_Timeout;

        public const int gameviewWidth = 400;
        public const int gameviewHeight = 2000;

		//CONSTRUCTOR
		public PipelineProtocols(int a, String s, int t)
        {
            windowSize = a;
            strategy = s;
            timeoutTime = t;

            Title = "Pipeline Protocols" + strategy;

            //do only create content if device is not roated
			if (Application.Current.MainPage.Width < Application.Current.MainPage.Height)
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
        //Gets called everytime the Page is not shown anymore. For example when clicking the back navigation
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopAsync(); // skip the settings page and go back to the overview
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
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(14, GridUnitType.Star)},
					new RowDefinition{ Height = new GridLength(2, GridUnitType.Star)}
				};
            CreateTopTopHalf(grid);
			CreateTopHalf(grid);
			CreateBottomHalf(grid);

            isContentCreated = true;
		}

		/**********************************************************************
        *********************************************************************/
		void CreateTopTopHalf(Grid grid)
        {
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (2)
			//Screen Width -20 because Margin is 10
			double StackChildSize = (Application.Current.MainPage.Width - 20) / 1;

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
                Orientation = StackOrientation.Vertical,
				Margin = new Thickness(10),

			};

            l_Timeout = new Label { Text = "Timeout: --" };
            //l_LastRecentInOrderAtReceiver = new Label{ Text = "Last recent in-order received packet: --"};
            //l_LastRecentAcknowlegement = new Label { Text = "Last recent acknowlegment: --" };
            stackLayout.Children.Add(l_Timeout);
            //stackLayout.Children.Add(l_LastRecentInOrderAtReceiver);
            //stackLayout.Children.Add(l_LastRecentAcknowlegement);

			grid.Children.Add(stackLayout, 0, 0);
        }

		/**********************************************************************
        *********************************************************************/
		void CreateTopHalf(Grid grid)
		{
            var scrollview = new ScrollView();

			gameView = new CocosSharpView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};
            double scaleFactor = Application.Current.MainPage.Width / gameviewWidth;
            gameView.HeightRequest = gameviewHeight* scaleFactor; // SCROLLING!!!!!!!!!!!!!!!!
            scrollview.Content = gameView;
			grid.Children.Add(scrollview, 0, 1);
		}

		/**********************************************************************
        *********************************************************************/
		void CreateBottomHalf(Grid grid)
		{
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (2)
			//Screen Width -20 because Margin is 10
			double StackChildSize = (Application.Current.MainPage.Width - 20) / 2;

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
                Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            Button b_Send = new Button
            {
                Text = "Send Package",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Send.Clicked += B_Send_Clicked;
            stackLayout.Children.Add(b_Send);

            b_Stop = new Button
            {
                Text = "Stop",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Stop.Clicked += B_Stop_Clicked;
            stackLayout.Children.Add(b_Stop);

			grid.Children.Add(stackLayout, 0, 2);
		}

		/**********************************************************************
        *********************************************************************/
		async void B_Send_Clicked(object sender, EventArgs e)
        {
            if (strategy == "Selective Repeat")
            {
                int a1 = PipelineProtocolsScene.nextSeqnum;
                if (a1 == 29)
                {
                    await DisplayAlert("Alert", "You are done!", "OK");
                }
                else
                {//PipelineProtocolsScene.SendPackageAt(0);
                    PipelineProtocolsScene.InvokeSender();
                }
            }
            else
            {
                int a1 = PipelineProtocolsScene2.nextSeqnum;
                if (a1 == 29)
                {
                    await DisplayAlert("Alert", "You are done!", "OK");
                }
                else
                {//PipelineProtocolsScene.SendPackageAt(0);
                    PipelineProtocolsScene2.InvokeSender();
                }
            }

        }

		/**********************************************************************
        *********************************************************************/
		void B_Stop_Clicked(object sender, EventArgs e)
        {
			switch (paused)
			{
				case true:
                    gameView.Paused = false;
                    paused = false;
                    b_Stop.Text = "Stop";
					break;
                case false:
					b_Stop.Text = "Continue";
                    gameView.Paused = true;
                    paused = true;
					break;
			}
        }

		/**********************************************************************
        *********************************************************************/
		/// <summary> deletes all content and informs the user to rotate the device </summary>
		void DeleteContent()
		{
            //if (gameScene != null) { gameScene.Dispose(); }
            //if (gameScene2 != null) { gameScene2.Dispose(); }
			this.Content = null;
			this.Content = new Label { Text = "please rotate your device" };
			isContentCreated = false;
		}

		/**********************************************************************
        *********************************************************************/
		//sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
			var cc_gameView = sender as CCGameView;
			if (cc_gameView != null)
			{
				// This sets the game "world" resolution 
				//Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
				//###############################################################
                cc_gameView.DesignResolution = new CCSizeI(gameviewWidth, gameviewHeight); //CLIPPING
                                                                       //###############################################################

                //choose gamescene for GoBackN or Selective Repeat
                if(strategy == "Selective Repeat"){
                    gameScene = new PipelineProtocolsScene(cc_gameView); 
                    // Starts CocosSharp:
                    cc_gameView.RunWithScene(gameScene);
                }else{
                    gameScene2 = new PipelineProtocolsScene2(cc_gameView); 
                    // Starts CocosSharp:
                    cc_gameView.RunWithScene(gameScene2);
                }
				
			
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
			if (width > height && isContentCreated)
			{
				DeleteContent();
			}
			else if (height > width && isContentCreated == false)
			{
				CreateContent();
			}
		}


	}
}

