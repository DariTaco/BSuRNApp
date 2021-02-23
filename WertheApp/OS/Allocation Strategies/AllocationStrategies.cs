using System;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using System.Text.RegularExpressions; //Regex.IsMatch
using System.Diagnostics;


namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategies : ContentPage
    {
        private double width = 0;
        private double height = 0;

        private static SKCanvasView canvasView; // drawing View
        private static AllocationStrategiesDraw draw; // drawing object
        private static String algorithm; // chosen algorithm for memory allocation

        // important controls
        private static Button b_Next;
        private static Entry e_MemoryRequest;

        public AllocationStrategies(String p_algorithm)
        {
            // Help/ info button upper right corner
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            Title = "Allocation Strategies: " + algorithm;

            // assign variables
            algorithm = p_algorithm;

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
            draw = new AllocationStrategiesDraw();
            canvasView = new SKCanvasView();
            canvasView = AllocationStrategiesDraw.ReturnCanvas();
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
            var b_Restart = new Button
            {
                Text = "Restart",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
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
                VerticalOptions = LayoutOptions.Center
            };
            e_MemoryRequest = new Entry
            {   Keyboard = Keyboard.Numeric,  //only numbers are allowed
                VerticalOptions = LayoutOptions.Center
            };
            stackLayout.Children.Add(l_MemoryRequest);
            stackLayout.Children.Add(e_MemoryRequest);

            // next button
            b_Next = new Button
            {
                Text = "Start",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
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
            //TODO
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
                }
                // otherwise wait until a valid memory request is made by the user
                else
                {
                    e_MemoryRequest.BackgroundColor = Color.FromRgba(238, 130, 238, 30);
                }
            }
            else
            {
                AllocationStrategiesAlgorithm.Next();
                switch (algorithm)
                {
                    case "First Fit":
                        AllocationStrategiesAlgorithm.FirstFit();
                        break;
                    case "Next Fit":
                        AllocationStrategiesAlgorithm.NextFit();
                        break;
                    case "Best Fit":
                        AllocationStrategiesAlgorithm.BestFit();
                        break;
                    case "Worst Fit":
                        AllocationStrategiesAlgorithm.WorstFit();
                        break;
                    case "Combined Fit":
                        AllocationStrategiesAlgorithm.CombinedFit();
                        break;
                }
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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object>(this, "Landscape"); // enforce landscape mode
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object>(this, "Unspecified"); // undo enforcing landscape mode
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

