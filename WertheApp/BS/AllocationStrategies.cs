using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic; 
using System.Linq; //fragmentList.ElementAt(i);


using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.BS
{
    //This class splits the Content in 2 halfs. 
    //Top half is the animated scene, which is actually defined in AllocationStrategiesScene
    //Bottom half are buttons and labels that interact with the scene
    public class AllocationStrategies : ContentPage
    {
        //VARIABLES
        double StackChildSize;
        public static List<int> fragmentList { get; set; } //List of fragments. From Settings Page passed to the constructor and later accesed from Scene
        public static String strategy { get; set; } //Choosen strategy (Firts Fit, ...) from Settings Page passed to the constructor and later accesed from Scene
        //public static int[] memoryBlocks; //=array of framents from fragmentList

        public static int memoryRequest; //gets its value from the modal page

		private double width = 0;
		private double height = 0;

		public const int gameviewWidth = 330;
		public const int gameviewHeight = 100;

        bool isContentCreated; //indicates weather the Content of the page was already created

        public enum myEnum
        {
            newRequest = 0, //new request was entered after clicking th next button
            searchingForBlock = 1, // currently searching for a memory block which is big enough 
            successfull = 2, //free memory found 
            unsuccessfull = 3, //no memory block found which is big enough to fit in the request
            noRequestYet = 4,   //the application just started an no memory has been requested yet
            memoryFull = 5 //no free space at all. Every block is full
        }
        public static myEnum memoryRequestState;

        //Labels. Values change
        public static Label l_Size;
        public static Label l_Free;
        public static Label l_Diff;
        public static Label l_Best;

        //values for labels
        public static String size = "-";
        public static String free = "-";
        public static String diff = "-";
        public static String best = "-";


        //CONSTRUCTOR
        public AllocationStrategies(List<int> l, String s)
        {
            ToolbarItem i = new ToolbarItem();
            i.Text = "Hilfe";
            this.ToolbarItems.Add(i);

            fragmentList = l;
            strategy = s;
            memoryRequestState = myEnum.noRequestYet;

            Title = "Allocation Strategies: " + strategy;

            isContentCreated = false;

            if (Application.Current.MainPage.Width > Application.Current.MainPage.Height)
            {
                this.isContentCreated = true;
                CreateContent();
                this.Content.IsVisible = true;
            }
            else
            {
                CreateContent();
                this.isContentCreated = true;
                this.Content.IsVisible = false;
            }

            //EVENT LISTENER
			//subscribe to Message in order to know if a new memory request was made
            MessagingCenter.Subscribe<AllocationStrategiesModal>(this, "new memory request", (args) =>{
                l_Size.Text = memoryRequest.ToString();
                l_Diff.Text = diff;
                l_Free.Text = free;
				if (AllocationStrategiesScene.CheckIfFull())
				{
					memoryRequestState = myEnum.memoryFull;
                }
                else
                {
                    memoryRequestState = myEnum.newRequest;
                }


            });
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
        //sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
			AllocationStrategiesScene gameScene;

			var gameView = sender as CCGameView;
			if (gameView != null)
			{
				// This sets the game "world" resolution to 330x100:
                //Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
                gameView.DesignResolution = new CCSizeI(gameviewWidth, gameviewHeight);
               
				// GameScene is the root of the CocosSharp rendering hierarchy:
				gameScene = new AllocationStrategiesScene(gameView);

				// Starts CocosSharp:
				gameView.RunWithScene(gameScene);

			}
		}

        /**********************************************************************
        *********************************************************************/
		void CreateTopHalf(Grid grid)
		{
            
            /*TODO schwarzer Rand weg*/
            var gameView = new CocosSharpView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White, /*Android bug: Doesn't work for Android*/
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
            //Screen Width is divided by the amount of elements (9)
            //Screen Width -20 because Margin is 10
            if(!isContentCreated){
                StackChildSize = (Application.Current.MainPage.Height - 20) / 9;
            }
            else{
                StackChildSize = (Application.Current.MainPage.Width - 20) / 9;
            }

            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            //For example l_Size, size
            var stackLayout = new StackLayout{
                Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

            };

            var l_1 = new Label { Text = "Size:", WidthRequest = StackChildSize, 
                VerticalOptions= LayoutOptions.Center};
			l_Size = new Label { Text = size, WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center };
			stackLayout.Children.Add(l_1);
            stackLayout.Children.Add(l_Size);

            var l_2 = new Label { Text = "Free:", WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center };
            l_Free = new Label { Text = free, WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center };
			stackLayout.Children.Add(l_2);
            stackLayout.Children.Add(l_Free);

            var l_3 = new Label { Text = "Diff.:", WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center };
            l_Diff = new Label { Text = diff, WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center };
            stackLayout.Children.Add(l_3);
            stackLayout.Children.Add(l_Diff);

            var l_4 = new Label { Text = "Best:", WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center };
            l_Best = new Label { Text = best, WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.Center};
			stackLayout.Children.Add(l_4);
            stackLayout.Children.Add(l_Best);

			var b_Next = new Button{ Text = "Next", WidthRequest = StackChildSize, 
                VerticalOptions = LayoutOptions.CenterAndExpand };
            b_Next.Clicked += B_Next_Clicked;
            stackLayout.Children.Add(b_Next);

			grid.Children.Add(stackLayout, 0, 1);
		}

        /**********************************************************************
        *********************************************************************/
        async void B_Next_Clicked(object sender, EventArgs e)
        {
            switch (memoryRequestState)
			{
                case myEnum.newRequest: //new request
                    memoryRequestState = myEnum.searchingForBlock;
                    AllocationStrategiesScene.RequestNew(memoryRequest);
                    //Debug.WriteLine("NEW REQUEST");
					break;
                case myEnum.searchingForBlock: //searching for block
                    Debug.WriteLine("SEARCHING FOR BLOCK");
					switch (strategy)
					{
						case "First Fit":
							AllocationStrategiesScene.pos++;
							AllocationStrategiesScene.FirstFit(memoryRequest);
							break;
						case "Next Fit":
                            AllocationStrategiesScene.pos++;
                            AllocationStrategiesScene.NextFit(memoryRequest);
							break;
						case "Best Fit":
                            AllocationStrategiesScene.pos++;
                            AllocationStrategiesScene.BestFit(memoryRequest);
							break;
						case "Worst Fit":
                            AllocationStrategiesScene.pos++;
                            AllocationStrategiesScene.WorstFit(memoryRequest);
							break;
						case "Tailoring Best Fit":
                            AllocationStrategiesScene.pos++;
                            AllocationStrategiesScene.TailoringBestFit(memoryRequest);
							break;
					}
					break;
                case myEnum.successfull: //successfull
                    //Debug.WriteLine("SUCCESSFULL");
					AllocationStrategiesScene.ClearRedArrow();
					AllocationStrategiesScene.ClearGrayArrow();
                    AllocationStrategiesScene.DrawFill();
					memoryRequestState = myEnum.noRequestYet;

					//clear information bar
					l_Size.Text = size;
					l_Diff.Text = diff;
					l_Best.Text = best;
					l_Free.Text = free;
					break;
                case myEnum.unsuccessfull: //unsucessfull
                    //Debug.WriteLine("UNSUCCESSFULL");
					AllocationStrategiesScene.ClearGrayArrow();
					AllocationStrategiesScene.ClearRedArrow();
					await DisplayAlert("Alert", "No free space has been found", "OK");
					memoryRequestState = myEnum.noRequestYet; //ready for a new request
															  
                    //clear information bar
					l_Size.Text = size;
					l_Diff.Text = diff;
					l_Best.Text = best;
					l_Free.Text = free;
					break;
                case myEnum.noRequestYet: //no requst yet
                    //Debug.WriteLine("NO REQUEST YET");
                    await Navigation.PushModalAsync(new AllocationStrategiesModal(), true);
                    break;
                case myEnum.memoryFull: //memory is full
                    //Debug.WriteLine("MEMORY IS FULL");
                    await DisplayAlert("Alert", "Out of memory! The app will close now", "OK");
                    await Navigation.PopAsync();
                    break;
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
            if (width > height)
			{
                //isContentCreated = true;
                this.Content.IsVisible = true;
			}
            else if (height > width && isContentCreated)
            {
                //isContentCreated = false;
                this.Content.IsVisible = false;

                DisplayAlert("Alert", "Please rotate the device", "OK");
			}
		}
	}
}