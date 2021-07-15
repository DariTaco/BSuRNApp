using System;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using System.Text.RegularExpressions; //Regex.IsMatch
using System.Diagnostics;
using System.Collections.Generic;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategies : ContentPage
    {
        // needed for forcing landscape mode / rotation detection
        private double width = 0;
        private double height = 0;

        // needed for UI
        private static SKCanvasView canvasView; // drawing View
        private static AllocationStrategiesDraw draw; // drawing object (!neccessary!)
        private static AllocationStrategiesAlgorithm algo; // (!neccessary!)

        // important controls
        private static Button b_Next, b_Restart;
        private static Xamarin.Forms.Entry e_MemoryRequest;

        // needed for restart
        private static List<FragmentBlock> allFragmentsList; // list representing the free and used memory fragments - algorithm
        private static String strategy; // chosen strategy for memory allocation

        //CONSTRUCTOR
        public AllocationStrategies(String p_Strategy, List<FragmentBlock> p_AllFragmentsList)
        {
            // Help/ info button upper right corner
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            // assign variables
            strategy = p_Strategy;
            allFragmentsList = new List<FragmentBlock>(p_AllFragmentsList); // copy List without reference to be able to alter it without affecting the original. NOTE: objects in list can still be affected
            algo = new AllocationStrategiesAlgorithm(strategy, allFragmentsList);

            Title = "Allocation Strategies: " + p_Strategy;

            MessagingCenter.Send<object>(this, "Landscape"); // enforce landscape mode

            // content starts only after notch
            On<iOS>().SetUseSafeArea(true);

            CreateContent();            
        }

        /**********************************************************************
        ***********************************************************************/
        void CreateContent()
        {
            //Split page in 2 parts (upper part for the drawing, lower part for the controls)
            var grid = new Grid();
            this.Content = grid;
            grid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition{ Height = new GridLength(4, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Auto)}
                };
            CreateTopHalf(grid);
            CreateBottomHalf(grid);
        }

        /**********************************************************************
        ***********************************************************************/
        void CreateTopHalf(Grid grid)
        {
            draw = new AllocationStrategiesDraw();
            canvasView = new SKCanvasView();
            canvasView = AllocationStrategiesDraw.ReturnCanvas();
            grid.Children.Add(canvasView, 0, 0);
        }

        /**********************************************************************
        ***********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            var controlsGrid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition()

                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(){ Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition(){ Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition(){ Width = new GridLength(3, GridUnitType.Star) }

                },
                VerticalOptions = LayoutOptions.Center,

            };

            
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(5),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center


            };
            

            // restart button
            b_Restart = new Button
            {
                Text = "Restart",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Restart.IsEnabled = false;
            controlsGrid.Children.Add(b_Restart, 0, 0);

            // entry for entering memory request
            var l_MemoryRequest = new Label
            {   Text = "Memory request: ",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                FontSize = App._labelFontSize
            };
            e_MemoryRequest = new Xamarin.Forms.Entry
            {   Keyboard = Keyboard.Numeric,  //only numbers are allowed
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                //BackgroundColor = Color.WhiteSmoke,
                FontSize = App._entryFontSize,
                TextColor = Color.Black,
                MaxLength = 3
            };
            e_MemoryRequest.WidthRequest = 50;
            stackLayout.Children.Add(l_MemoryRequest);
            stackLayout.Children.Add(e_MemoryRequest);
            controlsGrid.Children.Add(stackLayout, 1, 0);

            // next button
            b_Next = new Button
            {
                Text = "Confirm",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize
            };
            b_Next.Clicked += B_Next_Clicked;
            controlsGrid.Children.Add(b_Next, 2, 0);

            // add content to bottom half of grid
            grid.Children.Add(controlsGrid, 0, 1);
        }

        /**********************************************************************
        ***********************************************************************
        validates the given string. Only numbers (except 0) are allowed.
        returns true if string is valid */
        bool ValidateMemoryRequestInput(String p_s)
        {
            String s = "" + p_s; // in case parameter p_s is null
            return Regex.IsMatch(s, @"^[1-9]\d*$"); //matches only numbers(exept 0);
        }

        /**********************************************************************
        *********************************************************************/
        void B_Restart_Clicked(object sender, EventArgs e)
        {
            algo = new AllocationStrategiesAlgorithm(strategy, allFragmentsList);
            CreateContent();
            AllocationStrategiesDraw.Paint();
            foreach (FragmentBlock fb in allFragmentsList)
            {
                Debug.Write(" |" + fb.IsFree() + " " + fb.GetSize());
            }
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Next_Clicked(object sender, EventArgs e)
        {
            AllocationStrategiesAlgorithm.Status status = AllocationStrategiesAlgorithm.GetStatus();

            // if new memory request was made
            if (b_Next.Text == "Confirm" )
            {
                // if the memory request is valid
                if (ValidateMemoryRequestInput(e_MemoryRequest.Text))
                {
                    b_Next.Text = "Next";

                    // disable memory request entry and indicate that it's accepted
                    e_MemoryRequest.IsEnabled = false;
                    e_MemoryRequest.BackgroundColor = Color.FromRgba(172, 255, 47, 30);

                    // enable restart button
                    b_Restart.IsEnabled = true;

                    // check first if memory is already full before you make a new request
                    if (AllocationStrategiesAlgorithm.MemoryIsFull())
                    {
                        await DisplayAlert("Alert", "Out of memory! Please restart.", "OK");
                        b_Next.Text = "Confirm";
                        e_MemoryRequest.IsEnabled = true; // enable memory request entry 
                        e_MemoryRequest.BackgroundColor = Color.White;
                        e_MemoryRequest.Text = "";
                    }
                    else
                    {
                        AllocationStrategiesAlgorithm.Start(Int32.Parse(e_MemoryRequest.Text));
                    }
                }
                // otherwise wait until a valid memory request is made by the user
                else
                {
                    e_MemoryRequest.BackgroundColor = Color.FromRgba(238, 130, 238, 30);
                }
            }
            else
            {
                Debug.WriteLine("next button text");

                // start or still searching
                if (status == AllocationStrategiesAlgorithm.Status.searching || status == AllocationStrategiesAlgorithm.Status.start)
                {
                    Debug.WriteLine("start or Still searching ");
                    AllocationStrategiesAlgorithm.Next();
                    status = AllocationStrategiesAlgorithm.GetStatus();
                    if (status == AllocationStrategiesAlgorithm.Status.successfull)
                    {
                        b_Next.Text = "Allocate";
                    }
                    else if(status == AllocationStrategiesAlgorithm.Status.unsuccessfull)
                    {
                        b_Next.Text = "Confirm";
                        e_MemoryRequest.IsEnabled = true; // enable memory request entry 
                        e_MemoryRequest.BackgroundColor = Color.White;
                        e_MemoryRequest.Text = "";
                        await DisplayAlert("Alert", "Memory request was unsuccessfull.", "OK");

                    }
                }
                //sth found
                else if (status == AllocationStrategiesAlgorithm.Status.successfull)
                {
                    Debug.WriteLine("Sth found");
                    AllocationStrategiesAlgorithm.UpdateAllFragmentsList();
                    b_Next.Text = "Confirm";
                    e_MemoryRequest.IsEnabled = true; // enable memory request entry 
                    e_MemoryRequest.BackgroundColor = Color.White;
                    e_MemoryRequest.Text = "";

                }
                /*
                else if (status == AllocationStrategiesAlgorithm.Status.unsuccessfull)
                {
                    b_Next.Text = "Confirm";
                    e_MemoryRequest.IsEnabled = true; // enable memory request entry 
                    e_MemoryRequest.BackgroundColor = Color.White;
                    e_MemoryRequest.Text = "";
                    await DisplayAlert("Alert", "Memory request was unsuccessfull.", "OK");

                }*/
            }
            AllocationStrategiesDraw.Paint();
        }




























        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllocationStrategiesHelp()); // open info/ help page
        }

        /**********************************************************************
        ********************************************************************/
        AppLinkEntry _appLink; // App Linking
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object>(this, "Landscape"); // enforce landscape mode

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
            MessagingCenter.Send<object>(this, "Unspecified"); // undo enforcing landscape mode

            // App Linking
            _appLink.IsLinkActive = false;
            //TODO:Appindexing bug android
            if (Device.RuntimePlatform == Device.iOS)
            {
                Xamarin.Forms.Application.Current.AppLinks.RegisterLink(_appLink);
            }
        }

        /**********************************************************************
        ***********************************************************************
        this method is called everytime the device is rotated */
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                Debug.WriteLine("TRIGGERED");
                MessagingCenter.Send<object>(this, "Landscape"); // enforce landscape mode
            }

        }

    }
}

