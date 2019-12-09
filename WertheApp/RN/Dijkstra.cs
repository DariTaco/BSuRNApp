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

        private static SKCanvasView skiaview;
        private static DijkstraDraw draw;
        private static bool toggleRestart;
        private static Button b_Next, b_Back, b_Restart;
        private static int currentStep;
        private static int maxStep;


        //CONSTRUCTOR
        public Dijkstra(String[] a, int n)
        {
            Title = "Dijkstra";
            toggleRestart = false;
            currentStep = 0;
            maxStep = 30;

            draw = new DijkstraDraw(a, n);
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
                    new RowDefinition{ Height = new GridLength(9, GridUnitType.Star)},
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
            bool disableOrEnable = NextStep();
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
                Restart();
                b_Restart.Text = "Go to End";
                toggleRestart = false;
                b_Back.IsEnabled = false;
                b_Next.IsEnabled = true;
            }
            else
            {
                GoToEnd();
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

            bool disableOrEnable = PreviousStep();
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
        public DijkstraDraw GetDraw()
        {
            return draw;
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

        /**********************************************************************
      *********************************************************************/
        public static bool NextStep()
        {
            currentStep++;
            UpdateDijkstraDraw();
            if (currentStep >= maxStep)
            {
                return false;
            }
           
            return true;

        }

        /**********************************************************************
        *********************************************************************/
        public static bool PreviousStep()
        {
            currentStep--;
            UpdateDijkstraDraw();
            if (currentStep <= 0)
            {
                return false;
            }
            return true;
        }

        /**********************************************************************
        *********************************************************************/
        public static void Restart()
        {
            currentStep = 0;
            UpdateDijkstraDraw();
        }

        /**********************************************************************
        *********************************************************************/
        public static void GoToEnd()
        {
            currentStep = maxStep;
            UpdateDijkstraDraw();
        }

        public static void UpdateDijkstraDraw()
        {
            DijkstraDraw.SetCurrentStep(currentStep);
            DijkstraDraw.Paint();
        }
    }

}
