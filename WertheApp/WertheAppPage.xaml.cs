using System;
using Xamarin.Forms;


namespace WertheApp
{
    public partial class WertheAppPage : ContentPage
    {
        //CONSTRUCTOR
        public WertheAppPage()
        {
            InitializeComponent();

            Title = "WertheApp Start Screen";
			// This is the top-level grid, which will split our page in half
			var grid = new Grid();
			this.Content = grid;
			grid.RowDefinitions = new RowDefinitionCollection {
            // Each half will be the same size:
            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
			new RowDefinition{ Height = new GridLength(2, GridUnitType.Star)},
            };
			CreateTopHalf(grid);
			CreateBottomHalf(grid);
        }

		//METHODS
		void CreateTopHalf(Grid grid)
		{
			var i_hsLogo = new Image { Aspect = Aspect.AspectFit };
            i_hsLogo.Source = ImageSource.FromResource("hsLogo.png");

            //add content to Toplevel grid
            grid.Children.Add(i_hsLogo, 0, 0);
		}
		void CreateBottomHalf(Grid grid)
		{
            //organize content in Stacklayout
            var stackLayout = new StackLayout
            {
                Margin = new Thickness(20)
			};
            var l_pick = new Label 
            { 
                Text = "Pick your course:"
            };
            stackLayout.Children.Add(l_pick);

            var l_space = new Label();
			stackLayout.Children.Add(l_space);

			var b_bs = new Button
			{
				Text = "Betriebssysteme"
			};
            b_bs.Clicked += B_Bs_Clicked;

            stackLayout.Children.Add(b_bs);

			var b_rn = new Button
			{
				Text = "Rechnernetze"
			};
            b_rn.Clicked += B_Rn_Clicked;
			stackLayout.Children.Add(b_rn);

            //add content to Toplevel grid
			grid.Children.Add(stackLayout, 0, 1);
		}

		async void B_Bs_Clicked(object sender, EventArgs e)
		{
            await Navigation.PushAsync(new Betriebssysteme());
		}

		async void B_Rn_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Rechnernetze());
		}
    }
}
