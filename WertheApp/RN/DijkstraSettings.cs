using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WertheApp.RN
{
    public class DijkstraSettings: TabbedPage
    {
        //VARIABLES
        private int numberOfTabs;

        private double width = 0;
        private double height = 0;

        private static int currentTab;


        //CONSTRUCTOR
        public DijkstraSettings()
        {
            Title = "Dijkstra";
            numberOfTabs = 4;
            CreateContent();
            currentTab = 1;

            // keep track of which tab is currently selected
            this.CurrentPageChanged += (object sender, EventArgs e) => {
                currentTab = this.Children.IndexOf(this.CurrentPage) + 1;
                
                
            };
            ShowMyHint();
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        public async void ShowMyHint()
        {
            await DisplayAlert("Hint", "You can change the weights by tapping on them", "OK");
        }

        public async Task OpenPickerPopUp()
        {
            String action = await DisplayActionSheet("", "Cancel", null, "1", "2", "3", "4", "5", "6", "7", "8", "9");
            DijkstraSettingsDraw.GetNetworkByID(currentTab).SetAction(action);
        }

        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            //split grid in two parts (7:1)
            RowDefinitionCollection rowDefinition = new RowDefinitionCollection {
                    new RowDefinition{ Height = new GridLength(7, GridUnitType.Star)},
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
                    Text = "Default",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                b_Default.Clicked += B_Default_Clicked;

                Button b_Random = new Button
                {
                    Text = "Random",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                b_Random.Clicked += B_Random_Clicked;

                Button b_Presets = new Button
                {
                    Text = "Preset",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                b_Presets.Clicked += B_Presets_Clicked;

                //stacklayout
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(10),

                };
                stackLayout.Children.Add(b_Default);
                stackLayout.Children.Add(b_Presets);
                stackLayout.Children.Add(b_Random);
                stackLayout.Children.Add(b_Start);

                //get canvas to draw on
                DijkstraSettingsDraw skiaview = new DijkstraSettingsDraw(id, this);
                SKCanvasView canvas = skiaview.ReturnCanvas();

                //Grid for canvas and buttons
                var grid = new Grid();
                grid.RowDefinitions = rowDefinition;
                grid.Children.Add(canvas, 0, 0);
                grid.Children.Add(stackLayout, 0, 1);

                //Child Page (Tab)
                var child = new ContentPage();
                child.Content = grid;
                child.Title = "Network " + i;
                Children.Add(child);

            }

        }
        /**********************************************************************
        *********************************************************************/
        void B_Default_Clicked(object sender, EventArgs e)
        {
            DijkstraSettingsDraw.GetNetworkByID(currentTab).SetDefaultWeights();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Random_Clicked(object sender, EventArgs e)
        {
            
            DijkstraSettingsDraw.GetNetworkByID(currentTab).SetRandomWeights();

        }
        /**********************************************************************
        *********************************************************************/
        void B_Presets_Clicked(object sender, EventArgs e)
        {
           DijkstraSettingsDraw.GetNetworkByID(currentTab).SetPresetsWeights();

        }
        /**********************************************************************
        *********************************************************************/
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            if (IsLandscape())
            {
                await DisplayAlert("Alert", "Please hold your phone vertically for portrait mode", "OK");
            }
            String[] a = DijkstraSettingsDraw.GetNetworkByID(currentTab).GetAllWeights();
            await Navigation.PushAsync(new Dijkstra(a, currentTab));
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

            //reconfigure layout
            if (width > height)
            {
                this.IsVisible = false;
                //this.CurrentPage.IsVisible = false;
                
            }
            else if (height > width)
            {
                this.IsVisible = true;
                //this.CurrentPage.IsVisible = true;
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void ClearNetworkList()
        {
            DijkstraSettingsDraw.ClearNetworkList();
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
