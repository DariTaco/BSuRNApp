using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace WertheApp.RN
{
    public class TCP: ContentPage
    {
        //VARIABLES
        private double width = 0;
        private double height = 0;
        bool landscape = false; //indicates device orientation

        private SKCanvasView skiaview;
        private TCPDraw draw;

        //CONSTRUCTOR
        public TCP(String example)
        {
            Title = "TCP: " + example;

            draw = new TCPDraw();

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
            skiaview = TCPDraw.ReturnCanvas();
            grid.Children.Add(skiaview, 0, 0);
        }

        /**********************************************************************
           *********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            //set the size of the elements in such a way, that they all fit on the screen
            //Screen Width is divided by the amount of elements (2)
            //Screen Width -20 because Margin is 10
            double StackChildSize;
            if (landscape)
            {
                StackChildSize = (Application.Current.MainPage.Height - 20) / 6;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width - 20) / 6;
            }

            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10),

            };
            Label l_label1 = new Label()
            {
                Text = "cwnd:",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            Label l_label2 = new Label()
            {
                Text = "-",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            Label l_label3 = new Label()
            {
                Text = "dup ACK:",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            Label l_label4 = new Label()
            {
                Text = "0",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            Button b_Next = new Button
            {
                Text = "Next",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            Button b_Back = new Button
            {
                Text = "Back",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Back_Clicked;
            stackLayout.Children.Add(l_label1);
            stackLayout.Children.Add(l_label2);
            stackLayout.Children.Add(l_label3);
            stackLayout.Children.Add(l_label4);
            stackLayout.Children.Add(b_Back);
            stackLayout.Children.Add(b_Next);
            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Next_Clicked(object sender, EventArgs e)
        {
            TCPDraw.nextStep();
           UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Back_Clicked(object sender, EventArgs e)
        {

            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void UpdateDrawing()
        {
            TCPDraw.Paint();
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
