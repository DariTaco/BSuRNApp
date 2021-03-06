﻿using System;
using System.Linq; //list.Any()
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WertheApp.OS
{
    public class BuddySystem : ContentPage
    {
        //VARIABLES
        public static double absoluteMemorySize;
        public static int powerOfTwo;
		
        public static string [] availableProcessesInput;
        public static List<String> availableProcesses; //unued process names
        public static List<String> activeProcesses; //Process names that are in use right now 
        public static ObservableCollection<BuddySystemViewCell> buddySystemCells; // buddysystem canvas
        public static List<BuddySystemBlock> buddySystem;
        public static String currentProcess; //important variable for class buddysytsemviewcell
        public static bool endProcess; //important variable for class buddysytsemviewcell
        public static Button b_Restart;

        public static Xamarin.Forms.ListView listView;

		private double width = 0;
		private double height = 0;
    
        //CONSTRUCTOR
        public BuddySystem(int a)
        {
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            Title = "Buddy System";

            powerOfTwo = a;
            absoluteMemorySize = Math.Pow(2, Double.Parse(powerOfTwo.ToString())); //2ExponentX
			activeProcesses = new List<String>();
            availableProcessesInput= new string [26] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            availableProcesses = new List<string>(availableProcessesInput);

            currentProcess = "first";
            buddySystem = new List<BuddySystemBlock>();
            buddySystem.Add(new BuddySystemBlock((int)absoluteMemorySize, 0));
            endProcess = false;

            // content starts only after notch
            On<iOS>().SetUseSafeArea(true);

            CreateContent();
        }

		//METHODS

        /**********************************************************************
        *********************************************************************/
        //checks if there is a block, in which the process could possibly fit in
        public static bool AllocateBlock(int processSize, String processName){
            currentProcess = processName;

            //find the fitting block size
            int blockSize = BuddySystem.FindFittingBlockSize(processSize); //is 0 when process is bigger than the whole memory

            //does such a block exist? or a bigger one?
            int block = (int)absoluteMemorySize+1; //bigger than all blocks ever available
            int index = -1;
            //find the block clostest(and still bigger) to requested blocksize
            for (int i = 0; i < buddySystem.Count; i++){
                int dummy = buddySystem[i].GetBlockSize();
                if(dummy >= blockSize && dummy < block && buddySystem[i].GetFree()){
                    block = dummy;
                    index = i;
                }
            }
            //process bigger than memory or a blog with fitting size doesn't exist
            if(index == -1 || blockSize == 0){

                return false;
            } 
            //block fits perfectly
            if(block == blockSize && buddySystem[index].GetFree()){
                buddySystem[index].OccupyBlock(processName, processSize);
                AddBuddySystemCell();
                b_Restart.IsEnabled = true;

                return true;
            }

            //blocksize found but has to be split
            if(block > blockSize && buddySystem[index].GetFree()){
                SplitBlock(index, blockSize);
                buddySystem[index].OccupyBlock(processName, processSize);
                AddBuddySystemCell();
                b_Restart.IsEnabled = true;

                return true;
            }
            return false;
        }

        /**********************************************************************
        *********************************************************************/
        //Deallocates the Block which contains the given process
        public static void DeallocateBlock(String processName){
            currentProcess = processName;
            for (int i = 0; i < buddySystem.Count; i++)
            { 
                if(buddySystem[i].GetProcessName() == processName){
                    buddySystem[i].FreeBlock();
                    i = buddySystem.Count;
                }
            }
           
            AddBuddySystemCell();

            if(buddySystem.Count > 1){
                MergeBlocks();
            }
        }

        /**********************************************************************
        *********************************************************************/
        //checks if there a blocks that can be merged
        public static void MergeBlocks(){
            currentProcess = "merge";
            int blockSize, blockSizeR, blockSizeL;
            int buddyNo, buddyNoR, buddyNoL, buddyNoMergedBlock;
            bool free, freeR, freeL;
            List<int> buddyNoListCopy = new List<int>();
            //for 3 or more items in list, check every item, except the outter right and outter left
            for (int i = 1; i < buddySystem.Count - 1;)
            {
                //assign variables
                blockSize = buddySystem[i].GetBlockSize();
                blockSizeR = buddySystem[i + 1].GetBlockSize();
                blockSizeL = buddySystem[i - 1].GetBlockSize();
                buddyNo = buddySystem[i].GetBuddyNo();
                buddyNoR = buddySystem[i + 1].GetBuddyNo();
                buddyNoL = buddySystem[i - 1].GetBuddyNo();
                free = buddySystem[i].GetFree();
                freeR = buddySystem[i + 1].GetFree();
                freeL = buddySystem[i - 1].GetFree();
                buddyNoListCopy = buddySystem[i].GetBuddyNoList();


                if (free && buddyNo == 2)
                {
                    if (blockSizeL == blockSize && buddyNoL == 1 && freeL)
                    {
                      
                        buddyNoListCopy.RemoveAt(buddyNoListCopy.Count - 1); //remove last in List, which is the buddyNo of the child
                        buddyNoMergedBlock = buddyNoListCopy.Last(); //now the last element in the list is the buddyno
                        buddyNoListCopy.RemoveAt(buddyNoListCopy.Count - 1); //remove the last item since it will be attached again when using setbuddynolist


                        //merge:
                        //remove the 2 blocks that will be merged
                        buddySystem.Remove(buddySystem[i]);
                        buddySystem.Remove(buddySystem[i - 1]);
                        //replace them with the merged block 
                        buddySystem.Insert(i - 1, new BuddySystemBlock(blockSize + blockSizeL, buddyNoMergedBlock));

                        /*Bugfix 11.01.19 -> i-1 statt i*/
                        buddySystem[i-1].SetBuddyNoList(buddyNoListCopy);
                        buddySystem[i-1].FreeBlock();
                        AddBuddySystemCell();
                        i = 1; //start again
                    }
                    else
                    {
                        i++;
                    }
                }
                else if (free && buddyNo == 1)
                {
                    if (blockSizeR == blockSize && buddyNoR == 2 && freeR)
                    {
                      
                        buddyNoListCopy.RemoveAt(buddyNoListCopy.Count-1); //remove last in List, which is the buddyNo of the child
                        buddyNoMergedBlock = buddyNoListCopy.Last(); //now the last element in the list is the buddyno
                        buddyNoListCopy.RemoveAt(buddyNoListCopy.Count - 1); //remove the last item since it will be attached again when using setbuddynolist

                        //merge:
                        //remove the 2 blocks that will be merged
                        buddySystem.Remove(buddySystem[i + 1]);
                        buddySystem.Remove(buddySystem[i]);
                        //replace them with the merged block 
                        buddySystem.Insert(i, new BuddySystemBlock(blockSize + blockSizeR, buddyNoMergedBlock));

                        buddySystem[i].SetBuddyNoList(buddyNoListCopy);
                        buddySystem[i].FreeBlock();
                        AddBuddySystemCell();
                        i = 1; //start again
                    }else{
                        i++;
                    }    
                }
                else{
                    i++;
                }
            }

            //for 2 items in list
            if (buddySystem.Count == 2)
            {
                blockSize = buddySystem[0].GetBlockSize();
                //and both are free (no check for other properties since they have to be buddys)
                if (buddySystem[0].GetFree() && buddySystem[1].GetFree())
                {
                   
                    //merge
                    buddySystem.Remove(buddySystem[1]);
                    buddySystem.Remove(buddySystem[0]);
                    buddySystem.Insert(0, new BuddySystemBlock(blockSize*2, 0));
                    AddBuddySystemCell();
                }
            }
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
                    List<int> buddyNoList = buddySystem[index].GetBuddyNoList();
                    buddySystem.Remove(buddySystem[index]);
                    BuddySystemBlock block1 = new BuddySystemBlock(dummy, 2);
                    BuddySystemBlock block2 = new BuddySystemBlock(dummy, 1);
                    block1.SetBuddyNoList(buddyNoList);
                    block2.SetBuddyNoList(buddyNoList);
                    buddySystem.Insert(index, block1);
                    buddySystem.Insert(index, block2);
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
            //BuddySystemViewCell a = new BuddySystemViewCell();/*TODO*/
            buddySystemCells.Add(new BuddySystemViewCell()); //actually creates a new buddysystemviewcell
            listView.ScrollTo(buddySystemCells[buddySystemCells.Count-1],ScrollToPosition.MakeVisible, true);
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
		void CreateTopHalf(Grid grid){
            listView = new Xamarin.Forms.ListView
            {
                ItemTemplate = new DataTemplate(typeof(BuddySystemViewCell)),
                RowHeight = 100,
                BackgroundColor = Color.Transparent,
            };
            buddySystemCells = new ObservableCollection<BuddySystemViewCell>();
            listView.ItemsSource = buddySystemCells;
            grid.Children.Add(listView, 0, 0);
            AddBuddySystemCell(); //add very first cell
        }

		/**********************************************************************
        *********************************************************************/
		void CreateBottomHalf(Grid grid){
		
			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(5),
			};

            //add elements to stacklayout
            b_Restart = new Button
            {
                Text = "Restart",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Restart.IsEnabled = false;
            stackLayout.Children.Add(b_Restart);

            var b_Start = new Button
			{
				Text = "Start Process",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
			b_Start.Clicked += B_Start_Clicked;
			stackLayout.Children.Add(b_Start);

			var b_End = new Button
			{
				Text = "End Process",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
			b_End.Clicked += B_End_Clicked;
			stackLayout.Children.Add(b_End);

			grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        void B_Restart_Clicked(object sender, EventArgs e)
        {
            activeProcesses = new List<String>();
            availableProcesses = new List<string>(availableProcessesInput);

            currentProcess = "first";
            buddySystem = new List<BuddySystemBlock>();
            buddySystem.Add(new BuddySystemBlock((int)absoluteMemorySize, 0));
            endProcess = false;
            b_Restart.IsEnabled = false;

            CreateContent();
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
                MessagingCenter.Send<object>(this, "Landscape");  // enforce landscape mode
            }

        }

        AppLinkEntry _appLink; // App Linking
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object>(this, "Landscape");  // enforce landscape mode


            // App Linking
            Uri appLinkUri = new Uri(string.Format(App.AppLinkUri, Title).Replace(" ", "_"));
            _appLink = new AppLinkEntry
            {
                AppLinkUri = appLinkUri,
                Description = string.Format($"This App visualizes {Title}"),
                Title = string.Format($"WertheApp {Title}"),
                IsLinkActive = true,
                Thumbnail = ImageSource.FromResource("WertheApp.png")

            };
            //TODO:Appindexing bug android
            if (Device.RuntimePlatform == Device.iOS)
            {
                Xamarin.Forms.Application.Current.AppLinks.RegisterLink(_appLink);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object>(this, "Unspecified");  // undo enforcing landscape mode


            // App Linking
            _appLink.IsLinkActive = false;
            //TODO:Appindexing bug android
            if (Device.RuntimePlatform == Device.iOS)
            {
                Xamarin.Forms.Application.Current.AppLinks.RegisterLink(_appLink);
            }
        }
        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BuddySystemHelp());
        }
        /**********************************************************************
        *********************************************************************/
        //Debug Fuction: prints the memory
        private static void PrintBuddySystemList(){
            //Debug.WriteLine("#Buddy System List#");
            String s = "";
            for (int i = 0; i < buddySystem.Count; i++)
            {
                int value = buddySystem[i].GetBlockSize();
                s += value + " ";
            }
            //Debug.WriteLine(s);

            String st = "";
            for (int i = 0; i < buddySystem.Count; i++)
            {
                int value = buddySystem[i].GetProcessSize();
                st += value + " ";
            }
            //Debug.WriteLine(st);
        }
	}
}