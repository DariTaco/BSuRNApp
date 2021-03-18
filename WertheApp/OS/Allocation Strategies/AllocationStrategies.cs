using System;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using System.Text.RegularExpressions; //Regex.IsMatch
using System.Diagnostics;
using System.Collections.Generic;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategies : ContentPage
    {
        private double width = 0;
        private double height = 0;

        private static SKCanvasView canvasView; // drawing View
        private static AllocationStrategiesDraw draw; // drawing object
        private static AllocationStrategiesAlgorithm algo; 

        // important controls
        private static Button b_Next, b_Restart;
        private static Entry e_MemoryRequest;

        // needed for restart
        private static List<int> fragmentsList; // memory fragments
        private static String strategy; // chosen strategy for memory allocation

        public AllocationStrategies(String p_Strategy, List<int> p_FragmentsList)
        {
            // Help/ info button upper right corner
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            // assign variables
            strategy = p_Strategy;
            fragmentsList = p_FragmentsList;
            algo = new AllocationStrategiesAlgorithm(strategy, fragmentsList);

            Title = "Allocation Strategies: " + p_Strategy;

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
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
                };
            CreateTopHalf(grid);
            CreateBottomHalf(grid);
        }

        /**********************************************************************
        ***********************************************************************/
        void CreateTopHalf(Grid grid)
        {
            draw = new AllocationStrategiesDraw(algo);
            canvasView = new SKCanvasView();
            canvasView = draw.ReturnCanvas();
            grid.Children.Add(canvasView, 0, 0);
        }

        /**********************************************************************
        ***********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10),

            };

            // restart button
            b_Restart = new Button
            {
                Text = "Restart",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius
            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Restart.IsEnabled = false;
            stackLayout.Children.Add(b_Restart);

            // entry for entering memory request
            var l_MemoryRequest = new Label
            {   Text = "Memory request: ",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            e_MemoryRequest = new Entry
            {   Keyboard = Keyboard.Numeric,  //only numbers are allowed
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill
            };
            e_MemoryRequest.WidthRequest = 100;
            stackLayout.Children.Add(l_MemoryRequest);
            stackLayout.Children.Add(e_MemoryRequest);

            // next button
            b_Next = new Button
            {
                Text = "Start",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius
            };
            b_Next.Clicked += B_Next_Clicked;
            stackLayout.Children.Add(b_Next);

            // add content to bottom half of grid
            grid.Children.Add(stackLayout, 0, 1);
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
            algo = new AllocationStrategiesAlgorithm(strategy, fragmentsList);
            CreateContent();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Next_Clicked(object sender, EventArgs e)
        {
            // if new memory request was made
            if(b_Next.Text == "Start" )
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

                    algo.Start(Int32.Parse(e_MemoryRequest.Text));
                }
                // otherwise wait until a valid memory request is made by the user
                else
                {
                    e_MemoryRequest.BackgroundColor = Color.FromRgba(238, 130, 238, 30);
                }
            }
            else
            {
                algo.Next();
            }
          
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
            Application.Current.AppLinks.RegisterLink(_appLink);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object>(this, "Unspecified"); // undo enforcing landscape mode

            // App Linking
            _appLink.IsLinkActive = false;
            Application.Current.AppLinks.RegisterLink(_appLink);
        }

        /**********************************************************************
        ***********************************************************************
        this method is called everytime the device is rotated */
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                MessagingCenter.Send<object>(this, "Landscape"); // enforce landscape mode
            }
        }

    }
}

