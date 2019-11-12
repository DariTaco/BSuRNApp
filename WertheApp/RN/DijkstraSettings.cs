using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
namespace WertheApp.RN
{
    public class DijkstraSettings: TabbedPage
    {
        //VARIABLES
        private int numberOfTabs;

        private double width = 0;
        private double height = 0;


        //CONSTRUCTOR
        public DijkstraSettings()
        {
            Title = "Dijkstra";
            numberOfTabs = 4;
            CreateContent();
            //navigationPage.IconImageSource = "schedule.png";
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        #region
        void CreateContent()
        {
            //split grid in two parts (7:1)
            RowDefinitionCollection rowDefinition = new RowDefinitionCollection {
                    new RowDefinition{ Height = new GridLength(4, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
                };

            for (int i = 1; i <= numberOfTabs; i++)
            {
                int id = i;
                //button
                Button b_Start = new Button
                {
                    Text = "Start",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                b_Start.Clicked += B_Start_Clicked;
                Button b_Default = new Button
                {
                    Text = "Set Default Values",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                b_Default.Clicked += B_Default_Clicked;
                //stacklayout
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(10),

                };
                stackLayout.Children.Add(b_Default);
                stackLayout.Children.Add(b_Start);

                //get canvas to draw on
                DijkstraSettingsDraw skiaview = new DijkstraSettingsDraw(id);
                SKCanvasView canvas = skiaview.ReturnCanvas();

                //Grid for canvas and buttons
                var grid = new Grid();
                grid.RowDefinitions = rowDefinition;
                grid.Children.Add(canvas, 0, 0);
                grid.Children.Add(stackLayout, 0, 1);

                //Child Page (Tab)
                var child = new ContentPage();
                child.Content = grid;
                child.Title = "network " + i;
                Children.Add(child);

            }

        }
        #endregion
        /**********************************************************************
        *********************************************************************/
        async void B_Default_Clicked(object sender, EventArgs e)
        {
            //TODO: set default values
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            if (IsLandscape())
            {
                await DisplayAlert("Alert", "Please hold your phone vertically for portrait mode", "OK");
            }

            await Navigation.PushAsync(new Dijkstra());
        }

        /**********************************************************************
        *********************************************************************/
        double GetStackChildSize()
        {
            double stackChildSize;
            if (IsLandscape())
            {
                stackChildSize = (Application.Current.MainPage.Height - 20) / 2;
            }
            else
            {
                stackChildSize = (Application.Current.MainPage.Width - 20) / 2;
            }
            return stackChildSize;
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
        }

        /**********************************************************************
        *********************************************************************/
        static bool IsLandscape()
        {
            bool isLandscape = false;
            if (Application.Current.MainPage.Width > Application.Current.MainPage.Height)
            {
                isLandscape = true;
            }
            else
            {
                isLandscape = false;
            }
            return isLandscape;
        }
    }
}
