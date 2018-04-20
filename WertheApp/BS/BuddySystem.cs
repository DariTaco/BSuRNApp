using System;
using CocosSharp;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Linq; //list.Any()
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.BS
{
    public class BuddySystem : ContentPage
    {
        //VARIABLES
        public static double StackChildSize;
        public static double absoluteMemorySize;
        public static int powerOfTwo;
		public static int startedProcessSize; //gets its value from the modal page
        public static string startedProcessName; //gets its value from the modal page
        public static string endedProcessName; //gets its value from modal page
        public static string [] availableProcessesInput;
        public static List<String> availableProcesses; //unued process names
        public static List<String> activeProcesses; //Process names that are in use right now 
        public static ObservableCollection<BuddySystemViewCell> buddySystemCells; // buddysystem canvas

        public static ListView listView;

		bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;
        					   
        //CONSTRUCTOR
		public BuddySystem(int a)
        {
            powerOfTwo = a;
            absoluteMemorySize = Math.Pow(2, Double.Parse(powerOfTwo.ToString())); //2ExponentX
			activeProcesses = new List<String>();
            availableProcessesInput= new string [26] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            availableProcesses = new List<string>(availableProcessesInput);

			Title = "Buddy System";

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
        /**********************************************************************
        *********************************************************************/
        public static void AddBuddySystemCell(){
            BuddySystemViewCell b = new BuddySystemViewCell();
            BuddySystemViewCell a = new BuddySystemViewCell();
            buddySystemCells.Add(a); //actually creates a new buddysystemviewcell
            listView.ScrollTo(buddySystemCells[buddySystemCells.Count-1],ScrollToPosition.End, false);
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

			isContentCreated = true;
		}

		/**********************************************************************
        *********************************************************************/
		void CreateTopHalf(Grid grid){
            listView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(BuddySystemViewCell)),
                RowHeight = 100,
            };
            buddySystemCells = new ObservableCollection<BuddySystemViewCell>();
            listView.ItemsSource = buddySystemCells;
            grid.Children.Add(listView, 0, 0);
            Debug.WriteLine("create tophalft viewcell");
            AddBuddySystemCell();
        }

		/**********************************************************************
        *********************************************************************/
		void CreateBottomHalf(Grid grid){
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (2)
			//Screen Width -20 because Margin is 10
            if (!isContentCreated)
            {
                StackChildSize = (Application.Current.MainPage.Height - 20) / 2;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width - 20) / 2;
            }

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

		/**********************************************************************
        *********************************************************************/
		async void B_Start_Clicked(object sender, EventArgs e)
		{
            if (availableProcesses.Any())
            {
                await Navigation.PushModalAsync(new BuddySystemModal(), true);
            }else
            {
                await DisplayAlert("Alert", "Maximum amount of active processes is reached", "OK");
            }

		}

		/**********************************************************************
        *********************************************************************/
		async void B_End_Clicked(object sender, EventArgs e)
		{
            if(activeProcesses.Any()){
                await Navigation.PushModalAsync(new BuddySystemModal2(), true); //await pop up drop down menu wegen Konsistenz nicht verwendet
            }else{
                await DisplayAlert("Alert", "No active processes", "OK");
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
            else if (height > width && isContentCreated)
            {
                //isContentCreated = false;
                this.Content.IsVisible = false;

                DisplayAlert("Alert", "Please rotate the device", "OK");
            }
		}

	}
}