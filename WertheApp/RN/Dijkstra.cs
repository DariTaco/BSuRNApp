using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.RN
{
    public class Dijkstra: ContentPage
    {
        //VARIABLES
        private double width = 0;
        private double height = 0;
        private SKCanvasView skiaview;
        private DijkstraDraw draw;
        private bool toggleRestart;
        private Button b_Next, b_Back, b_Restart;

        //CONSTRUCTOR
        public Dijkstra()
        {
            Title = "Dijkstra";
            toggleRestart = false;

            draw = new DijkstraDraw();
            CreateContent();

            

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
            skiaview = DijkstraDraw.ReturnCanvas();
            grid.Children.Add(skiaview, 0, 0);
        }

        /**********************************************************************
         ********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10),

            };
            b_Next = new Button
            {
                Text = "Next",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Next.Clicked += B_Next_Clicked;
            b_Restart = new Button
            {
                Text = "Go to End",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Back = new Button
            {
                Text = "Back",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Back.Clicked += B_Back_Clicked;
            b_Back.IsEnabled = false;

            stackLayout.Children.Add(b_Back);
            stackLayout.Children.Add(b_Restart);
            stackLayout.Children.Add(b_Next);
            grid.Children.Add(stackLayout, 0, 1);
        }
        /**********************************************************************
        *********************************************************************/
        void B_Next_Clicked(object sender, EventArgs e)
        {
            bool disableOrEnable = DijkstraDraw.NextStep();
            b_Next.IsEnabled = disableOrEnable;
            toggleRestart = true;
            b_Restart.Text = "Restart";
            b_Back.IsEnabled = true;
            DijkstraDraw.Paint();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Restart_Clicked(object sender, EventArgs e)
        {
            if (toggleRestart)
            {
                DijkstraDraw.Restart();
                b_Restart.Text = "Go to End";
                toggleRestart = false;
                b_Back.IsEnabled = false;
                b_Next.IsEnabled = true;
            }
            else
            {
                DijkstraDraw.GoToEnd();
                b_Restart.Text = "Restart";
                toggleRestart = true;
                b_Back.IsEnabled = true;
                b_Next.IsEnabled = false;
            }
            DijkstraDraw.Paint();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Back_Clicked(object sender, EventArgs e)
        {

            bool disableOrEnable = DijkstraDraw.PreviousStep();
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
            DijkstraDraw.Paint();
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
