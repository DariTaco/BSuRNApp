using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace WertheApp.CN
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
        private bool toggleRestart;


        //CONSTRUCTOR
        public RenoFastRecovery()
        {
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            Title = "Reno Fast Recovery";
            draw = new RenoFastRecoveryDraw();

            toggleRestart = false;
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
            skiaview = RenoFastRecoveryDraw.ReturnCanvas();
            skiaview.BackgroundColor = Color.WhiteSmoke;
            grid.Children.Add(skiaview, 0, 0);
        }

        /**********************************************************************
           *********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
    
            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10),

            };

            b_Next = new Button
            {
                Text = "Next",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius
            };
            b_Next.Clicked += B_Next_Clicked;
            b_Restart = new Button
            {
                Text = "Go to End",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius
            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Back = new Button
            {
                Text = "Back",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius
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
            bool disableOrEnable = RenoFastRecoveryDraw.NextStep();
            b_Next.IsEnabled = disableOrEnable;
            toggleRestart = true;
            b_Restart.Text = "Restart";
            b_Back.IsEnabled = true;
            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Restart_Clicked(object sender, EventArgs e)
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
        void B_Back_Clicked(object sender, EventArgs e)
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
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RenoFastRecoveryHelp());
        }
        /**********************************************************************
        *********************************************************************/
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object>(this, "Portrait");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object>(this, "Unspecified");
        }

        //this method is called everytime the device is rotated
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                MessagingCenter.Send<object>(this, "Portrait");

            }

        }
    }
}
