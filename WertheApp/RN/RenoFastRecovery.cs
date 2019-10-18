using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace WertheApp.RN
{
    public class RenoFastRecovery: ContentPage
    {
        //VARIABLES
        private double width = 0;
        private double height = 0;
        bool landscape = false; //indicates device orientation

        private SKCanvasView skiaview;
        private RenoFastRecoveryDraw draw;

        private Button b_Next, b_Back, b_Restart;
        private Label l_Tresh;

        private bool toggleRestart;


        //CONSTRUCTOR
        public RenoFastRecovery()
        {
            Title = "Reno Fast Recovery";
            draw = new RenoFastRecoveryDraw();

            toggleRestart = false;

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
        void CreateContent()
        {
            // This is the top-level grid, which will split our page in half
            var grid = new Grid();
            this.Content = grid;
            grid.RowDefinitions = new RowDefinitionCollection {
                    // Each half will be the same size:
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
            skiaview = new SKCanvasView();
            skiaview = RenoFastRecoveryDraw.ReturnCanvas();
            grid.Children.Add(skiaview, 0, 0);
        }

        /**********************************************************************
           *********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            //set the size of the elements in such a way, that they all fit on the screen
            //Screen Width is divided by the amount of elements (3)
            //Screen Width -20 because Margin is 10
            double StackChildSize;
            if (landscape)
            {
                StackChildSize = (Application.Current.MainPage.Height - 20) / 3;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width - 20) / 3;
            }

            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10),

            };
            /*Label l_label1 = new Label()
            {
                Text = "tresh:",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            l_Tresh = new Label()
            {
                Text = "-",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };*/
            b_Next = new Button
            {
                Text = "Next",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            b_Restart = new Button
            {
                Text = "Go to End",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Back = new Button
            {
                Text = "Back",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Back.Clicked += B_Back_Clicked;
            b_Back.IsEnabled = false;
            //stackLayout.Children.Add(l_label1);
            //stackLayout.Children.Add(l_Tresh);
            stackLayout.Children.Add(b_Back);
            stackLayout.Children.Add(b_Restart);
            stackLayout.Children.Add(b_Next);
            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Next_Clicked(object sender, EventArgs e)
        {
            bool disableOrEnable = RenoFastRecoveryDraw.NextStep();
            b_Next.IsEnabled = disableOrEnable;
            toggleRestart = true;
            b_Restart.Text = "Restart";
            b_Back.IsEnabled = true;
            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Restart_Clicked(object sender, EventArgs e)
        {
            if (toggleRestart)
            {
                RenoFastRecoveryDraw.Restart();
                b_Restart.Text = "Go to End";
                toggleRestart = false;
                b_Back.IsEnabled = false;
                b_Next.IsEnabled = true;
            }
            else
            {
                RenoFastRecoveryDraw.GoToEnd();
                b_Restart.Text = "Restart";
                toggleRestart = true;
                b_Back.IsEnabled = true;
                b_Next.IsEnabled = false;
            }

            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Back_Clicked(object sender, EventArgs e)
        {

            bool disableOrEnable = RenoFastRecoveryDraw.PreviousStep();
            b_Back.IsEnabled = disableOrEnable;
            if (disableOrEnable)
            {
                toggleRestart = true;
                b_Restart.Text = "Restart";
            }
            else
            {
                toggleRestart = false;
                b_Restart.Text = "Go to End";
            }
            b_Next.IsEnabled = true;
            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void UpdateDrawing()
        {
            RenoFastRecoveryDraw.Paint();
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
            }
        }
    }
}
