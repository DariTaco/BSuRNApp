using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;

namespace WertheApp.CN
{
    public class DijkstraSettings: TabbedPage
    {
        //VARIABLES
        private int numberOfTabs;
        private static int currentTab;
        private double width = 0;
        private double height = 0;

        //CONSTRUCTOR
        public DijkstraSettings()
        {
            Title = "Dijkstra";

            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            numberOfTabs = 4;
            CreateContent();
            currentTab = 1;

            // keep track of which tab is currently selected
            this.CurrentPageChanged += (object sender, EventArgs e) => {
                currentTab = this.Children.IndexOf(this.CurrentPage) + 1;
                
                
            };
            //ShowMyHint();
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
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
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
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = App._buttonBackground,
                    TextColor = App._buttonText,
                    CornerRadius = App._buttonCornerRadius,
                    FontSize = App._buttonFontSize

                };
                b_Start.Clicked += B_Start_Clicked;

                Button b_Default = new Button
                {
                    Text = "Default",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = App._buttonBackground,
                    TextColor = App._buttonText,
                    CornerRadius = App._buttonCornerRadius,
                    FontSize = App._smallButtonFontSize

                };
                b_Default.Clicked += B_Default_Clicked;

                Button b_Random = new Button
                {
                    Text = "Random",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = App._buttonBackground,
                    TextColor = App._buttonText,
                    CornerRadius = App._buttonCornerRadius,
                    FontSize = App._smallButtonFontSize

                };
                b_Random.Clicked += B_Random_Clicked;

                Button b_Presets = new Button
                {
                    Text = "Preset",
                    //WidthRequest = GetStackChildSize(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = App._buttonBackground,
                    TextColor = App._buttonText,
                    CornerRadius = App._buttonCornerRadius,
                    FontSize = App._smallButtonFontSize

                };
                b_Presets.Clicked += B_Presets_Clicked;

                //stacklayout
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(5),

                };
                var stackLayout1 = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(5),

                };
                stackLayout1.Children.Add(b_Default);
                stackLayout1.Children.Add(b_Presets);
                stackLayout1.Children.Add(b_Random);
                stackLayout.Children.Add(b_Start);

                //get canvas to draw on
                DijkstraSettingsDraw skiaview = new DijkstraSettingsDraw(id, this);
                SKCanvasView canvas = skiaview.ReturnCanvas();
                canvas.BackgroundColor = App._viewBackground; 

                //Grid for canvas and buttons
                var grid = new Grid();
                grid.RowDefinitions = rowDefinition;
                grid.Children.Add(stackLayout1, 0, 0);
                grid.Children.Add(canvas, 0, 1);
                grid.Children.Add(stackLayout, 0, 2);

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
            String[] a = DijkstraSettingsDraw.GetNetworkByID(currentTab).GetAllWeights();
            await Navigation.PushAsync(new Dijkstra(a, currentTab));
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DijkstraHelp());
        }
        /**********************************************************************
        *********************************************************************/
        public static void ClearNetworkList()
        {
            DijkstraSettingsDraw.ClearNetworkList();
        }
        /**********************************************************************
       *********************************************************************/
        //this method is called everytime the device is rotated
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                MessagingCenter.Send<object>(this, "Portrait");
            }
        }
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
    }
}
