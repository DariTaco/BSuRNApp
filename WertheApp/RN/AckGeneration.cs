using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace WertheApp.RN
{
    public class AckGeneration : ContentPage
    {
        //VARIABLES
        private double width = 0;
        private double height = 0;
        bool landscape = false; //indicates device orientation

        private SKCanvasView skiaview;
        private AckGenerationDraw draw;

        private Button b_Next, b_Back;
        private Label l_Cwnd, l_DupAck;

        public AckGeneration()
        {
            Title = "Ack Generation";
            draw = new AckGenerationDraw();

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
            skiaview = AckGenerationDraw.ReturnCanvas();
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
