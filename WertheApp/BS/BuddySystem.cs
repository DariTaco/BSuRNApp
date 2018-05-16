using System;
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
		
        public static string [] availableProcessesInput;
        public static List<String> availableProcesses; //unued process names
        public static List<String> activeProcesses; //Process names that are in use right now 
        public static ObservableCollection<BuddySystemViewCell> buddySystemCells; // buddysystem canvas
        public static List<BuddySystemBlock> buddySystem;

        public static ListView listView;

		bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;
        					   
        //CONSTRUCTOR
		public BuddySystem(int a)
        {
            Title = "Buddy System";

            powerOfTwo = a;
            absoluteMemorySize = Math.Pow(2, Double.Parse(powerOfTwo.ToString())); //2ExponentX
			activeProcesses = new List<String>();
            availableProcessesInput= new string [26] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            availableProcesses = new List<string>(availableProcessesInput);
            buddySystem = new List<BuddySystemBlock>();
            buddySystem.Add(new BuddySystemBlock((int)absoluteMemorySize));

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
        }

		//METHODS

        /**********************************************************************
        *********************************************************************/
        //checks if there is a block, in which the process could possibly fit in
        public static bool AllocateBlock(int processSize, String processName){
            //find the fitting block size
            int blockSize = BuddySystem.FindFittingBlockSize(processSize); //is 0 when process is bigger than the whole memory

            //does such a block exist? or a bigger one?
            int block = (int)absoluteMemorySize+1; //bigger than all blocks ever available
            int index = -1;
            //find the block clostest(and still bigger) to requested blocksize
            for (int i = 0; i < buddySystem.Count; i++){
                int dummy = buddySystem[i].GetBlockSize();
                if(dummy >= blockSize && dummy <= block && buddySystem[i].GetFree()){
                    block = dummy;
                    index = i;
                }
            }
            //process bigger than memory or a blog with fitting size doesn't exist
            if(index == -1 || blockSize == 0){
                Debug.WriteLine("process bigger than memory or a blog with fitting size doesn't exist");
                printBuddySystemList();
                return false;
            } 
            //block fits perfectly
            if(block == blockSize && buddySystem[index].GetFree()){
                Debug.WriteLine("block "+block + "blocksize "+ blockSize);
                Debug.WriteLine("block fits perfectly");
                buddySystem[index].OccupyBlock(processName, processSize);
                printBuddySystemList();
                return true;
            }

            //blocksize found but has to be split
            if(block > blockSize && buddySystem[index].GetFree()){
                Debug.WriteLine("blocksize found but has to be split");
                SplitBlock(index, blockSize);
                buddySystem[index].OccupyBlock(processName, processSize);
                printBuddySystemList();
                return true;
            }
            Debug.WriteLine("sonderfall");
            return false;
        }

        /**********************************************************************
        *********************************************************************/
        public static void DeallocateBlock(String processName){
            for (int i = 0; i < buddySystem.Count; i++)
            { 
                if(buddySystem[i].GetProcessName() == processName){
                    buddySystem[i].FreeBlock();
                    i = buddySystem.Count;
                }
            }
            printBuddySystemList();
        }

        /**********************************************************************
        *********************************************************************/
        public static void MergeBlocks(){

        }


        /**********************************************************************
        *********************************************************************/
        //splits the block at the index until it has the required size 
        public static void SplitBlock(int index, int requiredSize){

            int blockSize = buddySystem[index].GetBlockSize();
            //entferne Block an Index in der Liste 
            //Teile in zwei auf, solange bis blocksize = requiredsize entspricht 
            while (blockSize > requiredSize)
            {
                int dummy = blockSize / 2;
                if (dummy >= requiredSize)
                {
                    //split
                    buddySystem.Remove(buddySystem[index]);
                    buddySystem.Insert(index, new BuddySystemBlock(dummy));
                    buddySystem.Insert(index, new BuddySystemBlock(dummy));
                }
                blockSize = dummy;
            }
        }

        /**********************************************************************
        *********************************************************************/
        //finds the fitting blocksize for a process and returns 0 when process is bigger than the whole memory
        public static int FindFittingBlockSize(int processSize){
            int fitting = (int)absoluteMemorySize;
            if (processSize > absoluteMemorySize) { return 0; }
            while(fitting >= processSize){
                fitting = fitting / 2; 
            }
            fitting = fitting * 2;
            return fitting;
        }

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

        private static void printBuddySystemList(){
            Debug.WriteLine("##############");
            String s = "";
            for (int i = 0; i < buddySystem.Count; i++)
            {
                int value = buddySystem[i].GetBlockSize();
                s += value + " ";
            }
            Debug.WriteLine(s);

            String st = "";
            for (int i = 0; i < buddySystem.Count; i++)
            {
                int value = buddySystem[i].GetProcessSize();
                st += value + " ";
            }
            Debug.WriteLine(st);
        }
	}
}