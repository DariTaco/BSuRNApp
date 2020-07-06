using System;
using Xamarin.Forms;
using System.Collections.Generic;
using SkiaSharp.Views.Forms;

namespace WertheApp.BS
{

    public class Deadlock: ContentPage
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static DeadlockDraw draw;
        private static Button b_Next;

        public Deadlock()
        {
            Title = "Deadlock";

            draw = new DeadlockDraw();
            CreateContent();
        }

        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            // This is the top-level grid, which will split our page in half
            var grid = new Grid();
            this.Content = grid;
            grid.RowDefinitions = new RowDefinitionCollection {
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

            skiaview = DeadlockDraw.ReturnCanvas();
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

            stackLayout.Children.Add(b_Next);
            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        void B_Next_Clicked(object sender, EventArgs e)
        {

        }

    }
}
