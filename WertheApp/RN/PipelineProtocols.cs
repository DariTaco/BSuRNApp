using System;
using CocosSharp;
using Xamarin.Forms;
using System.Diagnostics;

namespace WertheApp.RN
{
    public class PipelineProtocols : ContentPage
    {
        //VARIABLES
        private static ScrollView scrollView;
        public static CCGameView cc_gameView;
        PipelineProtocolsScene gameScene;
        PipelineProtocolsScene2 gameScene2;

        public static int windowSize;
        public static String strategy;
        public static int timeoutTime;

        public static bool paused = false;
        public static CocosSharpView gameView;
        Button b_Pause, b_Restart;

        bool landscape = false; //indicates device orientation

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

            ToolbarItem info = new ToolbarItem();
            info.Text = "Info";
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            windowSize = a;
            strategy = s;
            timeoutTime = t;

            Title = "Pipeline Protocols: " + strategy;

            //if orientation Horizontal
			if (Application.Current.MainPage.Width < Application.Current.MainPage.Height)
			{
                landscape = false;
				CreateContent();
			}
            //if orientation Landscape
			else
            {   
                landscape = true;
                CreateContent();
                this.Content.IsVisible = false;
				//this.Content = new Label { Text = "please rotate your device" };
			}
        }

		//METHODS
        /**********************************************************************
        *********************************************************************/
        //Gets called everytime the Page is not shown anymore. For example when clicking the back navigation
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (gameScene != null) { gameScene.Dispose(); }
            if (gameScene2 != null) { gameScene2.Dispose(); }
            PipelineProtocolsPack.stopEverything = true;
            PipelineProtocolsACK.stopEverything = true;
            if(strategy == "Selective Repeat"){
                PipelineProtocolsScene.stopEverything = true;
            }else{
                PipelineProtocolsScene2.stopEverything = true;
            }
        

        }

		/**********************************************************************
        *********************************************************************/
		void CreateContent()
		{
			// This is the top-level grid, which will split our page in half
			var grid = new Grid();
			this.Content = grid;
			grid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition{ Height = new GridLength(7, GridUnitType.Star)},
					new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
				};

			CreateTopHalf(grid);
			CreateBottomHalf(grid);
		}

		/**********************************************************************
        *********************************************************************/
		void CreateTopHalf(Grid grid)
		{
            scrollView = new ScrollView();
			gameView = new CocosSharpView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};
            double scaleFactorX, scaleFactorY;
            if(landscape){
                scaleFactorX = Application.Current.MainPage.Height / gameviewWidth;
                scaleFactorY = Application.Current.MainPage.Width / gameviewHeight;
            }
            else{
                scaleFactorX = Application.Current.MainPage.Width / gameviewWidth;
                scaleFactorY = Application.Current.MainPage.Height / gameviewWidth;
            }
            //TODO: Seit neuem update funktioniert das scrollen bei ios nicht mehr 
            //und zwar für die Werte 400 und 2000. Für 1000 geht es aber dann ist die Anzeige verschoben
            //Debug.WriteLine("get heightrequest : " + gameView.GetSizeRequest());
            if(Device.RuntimePlatform == Device.iOS){
                /*
                gameView.HeightRequest = gameviewHeight * scaleFactor; // SCROLLING!!!!!!!!!!!!!!!!
                scrollView.Content = gameView;
                grid.Children.Add(scrollView, 0, 0);
                */
                //gameView.HorizontalOptions = LayoutOptions.CenterAndExpand;
               // gameView.VerticalOptions = LayoutOptions.FillAndExpand;
               // gameView.HorizontalOptions = LayoutOptions.FillAndExpand;
                gameView.HeightRequest = Application.Current.MainPage.Height;
                //gameView.HeightRequest = (int)(gameviewHeight * scaleFactor); // SCROLLING!!!!!!!!!!!!!!!!
                scrollView.Content = gameView;
                grid.Children.Add(scrollView, 0, 0);
                
            }
            else
            {
                //gameView.WidthRequest = Application.Current.MainPage.Width;
                //gameView.HeightRequest = gameviewHeight * scaleFactor; // SCROLLING!!!!!!!!!!!!!!!!
                scrollView.Content = gameView;
                //scrollView.ScaleYTo(2.0);
                scrollView.ScaleXTo(2.5);
                grid.Children.Add(scrollView, 0, 0);
            }
		}

        /**********************************************************************
        *********************************************************************/
        //the attempt to add a gesture recognizer to this view didnt work 
        private static void AddGestureRecognizers()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 2; // double-tap
            tapGestureRecognizer.Tapped += (s, e) => {
            };
            gameView.GestureRecognizers.Add(tapGestureRecognizer);

            //add pan gesture recognizer
            double x = 0;
            double y = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) =>
            {
                // Handle the pan (only in zoomed in state)
                if (e.StatusType != GestureStatus.Completed)
                {
                    //only when within screen bounds or 20% more or less
                    if (y + e.TotalY >= gameView.Height * 1.2f / 2 * -1
                        && y + e.TotalY <= gameView.Height * 1.2f / 2)
                    {
                        y = y + e.TotalY;
                        gameView.TranslateTo(x, y);
                    }
                }
            };
            gameView.GestureRecognizers.Add(panGesture);
        }
            /**********************************************************************
            *********************************************************************/
            void CreateBottomHalf(Grid grid)
		{

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
                Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            b_Restart = new Button
            {
                Text = "Restart",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Restart.IsEnabled = false;
            stackLayout.Children.Add(b_Restart);

            b_Pause = new Button
            {
                Text = "Pause",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Pause.Clicked += B_Stop_Clicked;
            stackLayout.Children.Add(b_Pause);

            Button b_Send = new Button
            {
                Text = "Send",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Send.Clicked += B_Send_Clicked;
            stackLayout.Children.Add(b_Send);

            grid.Children.Add(stackLayout, 0, 1);
		}

		/**********************************************************************
        *********************************************************************/
		async void B_Send_Clicked(object sender, EventArgs e)
        {
            b_Restart.IsEnabled = true;
            if (strategy == "Selective Repeat")
            {
                int a1 = PipelineProtocolsScene.nextSeqnum;
                if (a1 == 29)
                {
                    await DisplayAlert("Alert", "You are done!", "OK");
                }
                else
                {
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
                {
                    PipelineProtocolsScene2.InvokeSender();
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Restart_Clicked(object sender, EventArgs e)
        {
            b_Restart.IsEnabled = false;
            OnDisappearing();
            CreateContent();
        }
        /**********************************************************************
       *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PipelineProtocolsInfo());
        }
        /**********************************************************************
        *********************************************************************/
        void B_Stop_Clicked(object sender, EventArgs e)
        {
			switch (paused)
			{
				case true:
                    if (strategy == "Selective Repeat"){ 
                        PipelineProtocolsScene.animationIsPaused = false;
                    }
                    else{
                        PipelineProtocolsScene2.animationIsPaused = false;
                    }
                    gameView.Paused = false;
                    paused = false;
                    b_Pause.Text = "Pause";
					break;
                case false:
                    if (strategy == "Selective Repeat"){
                        PipelineProtocolsScene.animationIsPaused = true;
                    }
                    else{
                        PipelineProtocolsScene2.animationIsPaused = true;
                    }
					b_Pause.Text = "Continue";
                    gameView.Paused = true;
                    paused = true;
					break;
			}
        }
		/**********************************************************************
        *********************************************************************/
		//sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
			cc_gameView = sender as CCGameView;
			if (cc_gameView != null)
			{
                // This sets the game "world" resolution 
                //Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
                //##############################################################
                if (Device.RuntimePlatform == Device.Android) { 
                    cc_gameView.DesignResolution = new CCSizeI(gameviewWidth, gameviewHeight); //CLIPPING
                }


                //TODO: temporary ios fix
            if (Device.RuntimePlatform == Device.iOS)
             {
                    /*
                 cc_gameView.DesignResolution = new CCSizeI(gameviewWidth, gameviewHeight); //CLIPPING
                  */
                    cc_gameView.DesignResolution = new CCSizeI((int)(Application.Current.MainPage.Width), (gameviewHeight)); //CLIPPING

                 //cc_gameView.DesignResolution = new CCSizeI(gameviewWidth, gameviewHeight); //damit würde es bei ipad gestreched werden
                 //double scaleFactor = Application.Current.MainPage.Width / gameviewWidth;
                 //cc_gameView.DesignResolution = new CCSizeI(gameviewWidth, (int)(gameviewHeight * scaleFactor));
                 cc_gameView.ResolutionPolicy = CCViewResolutionPolicy.ExactFit;


                 //cc_gameView.ViewportRectRatio = new CCRect(0, 0, (float)scrollView.Height, (float)scrollView.Width);
                 //cc_gameView.ViewportRectRatio = new CCRect(0, 0, gameviewWidth, gameviewHeight);
                 
             }

                //###############################################################

                //choose gamescene for GoBackN or Selective Repeat
                if (strategy == "Selective Repeat"){
                    gameScene = new PipelineProtocolsScene(cc_gameView);
                    // Starts CocosSharp:
                    cc_gameView.RunWithScene(gameScene);
                }else{
                    gameScene2 = new PipelineProtocolsScene2(cc_gameView);
                    //gameScene2.PositionY = -1100; //reagiert nicht mehr auf touch

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
			if (width > height)
			{
                this.Content.IsVisible = false;
			}
			else if (height > width)
			{
                this.Content.IsVisible = true;
                gameView.HeightRequest = Application.Current.MainPage.Height;
            }
		}
	}
}