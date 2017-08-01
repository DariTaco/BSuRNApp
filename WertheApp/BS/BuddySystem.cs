using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.BS
{
    public class BuddySystem : ContentPage
    {
		//VARIABLES
        public static double absoluteMemorySize;
        public static int powerOfTwo;
		public static int startedProcessSize; //gets its value from the modal page
        public static string startedProcessName; //gets its value from the modal page
        public static string endedProcessName; //gets its value from modal page
        public static List<int> activeProcesses; //Process names

        StackLayout stackLayout2;
        CocosSharpView gameView;
		BuddySystemScene gameScene;
		CCGameView cc_gameView;

		bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;
        					   
        //CONSTRUCTOR
		public BuddySystem(int a)
        {
            powerOfTwo = a;
            absoluteMemorySize = Math.Pow(2, Double.Parse(powerOfTwo.ToString())); //2ExponentX
			activeProcesses = new List<int>();

			Title = "Buddy System";

            //do only create content if device is rotated in landscape
			if (Application.Current.MainPage.Width > Application.Current.MainPage.Height)
			{
				CreateContent();
			}
			else
			{
				this.Content = new Label { Text = "please rotate your device" };
			}

			//subscribe to Message in order to know if a new process was started
			MessagingCenter.Subscribe<BuddySystemModal>(this, "new process started", (args) =>
			{
				Debug.WriteLine("#####################");
                Debug.WriteLine("new Process size: " + startedProcessSize);
                Debug.WriteLine("new Process name: " + startedProcessName);
			});

			//subscribe to Message in order to know if a new process was ended
			MessagingCenter.Subscribe<BuddySystemModal>(this, "process ended", (args) =>
			{
				Debug.WriteLine("#####################");
				Debug.WriteLine("ended Process name: " + endedProcessName);
			});
        }

		//METHODS
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

        void CreateTopHalf(Grid grid){
            var scrollview = new ScrollView();
            stackLayout2 = new StackLayout();
            AddScene();
            scrollview.Content = stackLayout2;
            grid.Children.Add(scrollview, 0, 0);
        }

        void CreateBottomHalf(Grid grid){
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

            //add elements to stacklayout
			var b_Start = new Button
			{
				Text = "START PROCESS",
				WidthRequest = StackChildSize,
				VerticalOptions = LayoutOptions.Center
			};
			b_Start.Clicked += B_Start_Clicked;
			stackLayout.Children.Add(b_Start);

			var b_End = new Button
			{
				Text = "END PROCESS",
				WidthRequest = StackChildSize,
				VerticalOptions = LayoutOptions.Center
			};
			b_End.Clicked += B_End_Clicked;
			stackLayout.Children.Add(b_End);

			grid.Children.Add(stackLayout, 0, 1);
        }


		async void B_Start_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new BuddySystemModal(), true);



		}

		async void B_End_Clicked(object sender, EventArgs e)
		{
            await Navigation.PushModalAsync(new BuddySystemModal2(), true); //await pop up drop down menu wegen Konsistenz nicht verwendet
       
            gameView.HeightRequest = 900; // SCROLLING!!!!!!!!!!!!!!!!
            cc_gameView.DesignResolution = new CCSizeI(330, 900); //CLIPPING
			gameScene = new BuddySystemScene(cc_gameView);
			cc_gameView.RunWithScene(gameScene);
		}

		/// <summary> deletes all content and informs the user to rotate the device </summary>
		void DeleteContent()
		{
			this.Content = null;
			this.Content = new Label { Text = "please rotate your device" };
			isContentCreated = false;
		}

		void AddScene()
		{

			gameView = new CocosSharpView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};
			////#######################################################
			gameView.HeightRequest = 100; // SCROLLING!!!!!!!!!!!!!!!!
										  //#########################################################

			stackLayout2.Children.Add(gameView);
		}

		//sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
			
			cc_gameView = sender as CCGameView;
			if (cc_gameView != null)
			{
                // This sets the game "world" resolution to 330x100:
                //Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
                //####################################################
                cc_gameView.DesignResolution = new CCSizeI(330, 100); //CLIPPING
                //########################################################
				// GameScene is the root of the CocosSharp rendering hierarchy:
				gameScene = new BuddySystemScene(cc_gameView);
				// Starts CocosSharp:
				cc_gameView.RunWithScene(gameScene);
			}
		}

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

