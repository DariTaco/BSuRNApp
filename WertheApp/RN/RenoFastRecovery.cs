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

        private Button b_Next, b_Back;
        private Label l_Cwnd, l_DupAck;


        //CONSTRUCTOR
        public RenoFastRecovery()
        {
            Title = "Reno Fast Recovery: ";
            draw = new RenoFastRecoveryDraw();

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
            l_Cwnd = new Label()
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
            l_DupAck = new Label()
            {
                Text = "0",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next = new Button
            {
                Text = "Next",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            b_Back = new Button
            {
                Text = "Back",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Back.Clicked += B_Back_Clicked;
            b_Back.IsEnabled = false;
            stackLayout.Children.Add(l_label1);
            stackLayout.Children.Add(l_Cwnd);
            stackLayout.Children.Add(l_label3);
            stackLayout.Children.Add(l_DupAck);
            stackLayout.Children.Add(b_Back);
            stackLayout.Children.Add(b_Next);
            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Next_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("next clicked");
            bool disableOrEnable = RenoFastRecoveryDraw.NextStep();
            b_Next.IsEnabled = disableOrEnable;
            Debug.WriteLine(RenoFastRecoveryDraw.GetCurrentStep());
            b_Back.IsEnabled = true;
            UpdateInfo();
            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Back_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("back cklicked");
            bool disableOrEnable = RenoFastRecoveryDraw.PreviousStep();
            b_Back.IsEnabled = disableOrEnable;
            Debug.WriteLine(RenoFastRecoveryDraw.GetCurrentStep());
            b_Next.IsEnabled = true;
            UpdateInfo();
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
        void UpdateInfo()
        {
            String textCwnd = RenoFastRecoveryDraw.GetCwnd();
            String textDupAck = RenoFastRecoveryDraw.GetDupAckCount();
            String textTresh = RenoFastRecoveryDraw.GetTresh();

            Debug.WriteLine(textCwnd + " " + textDupAck);
            l_Cwnd.Text = textCwnd;
            l_DupAck.Text = textDupAck;
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
