using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq; //fragmentList.ElementAt(i);
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics;

namespace WertheApp.RN
{
    public class CongestionAvoidance : ContentPage
    {
        //VARIABLES
        public static int errorTreshold;
        public static int treshold;
        public static bool reno;
        public static bool tahoe;

        bool landscape = false; //indicates device orientation

        private double width = 0;
		private double height = 0;

        private SKCanvasView skiaview;
        private CongestionAvoidanceDraw draw;

        //CONSTRUCTOR
        public CongestionAvoidance(int th, bool r, bool t)
        {
            treshold = th;
            reno = r;
            tahoe = t;

			/*Debug.WriteLine("##########");
            Debug.WriteLine("error Treshold: " + errorTreshold);
			Debug.WriteLine("treshold: " + treshold);
			Debug.WriteLine("reno: " + reno);
			Debug.WriteLine("tahoe: " + tahoe);*/

			Title = "Congestion Avoidance";

            //if orientation Horizontal
            if (Application.Current.MainPage.Width < Application.Current.MainPage.Height)
            {
                landscape = false;
                CreateContent();
                this.Content.IsVisible = false;
            }
            //if orientation Landscape
            else
            {
                landscape = true;
                CreateContent();
            }

            draw = new CongestionAvoidanceDraw();
        }

		//METHODS
        /**********************************************************************
        *********************************************************************/
        //Gets called everytime the Page is not shown anymore. For example when clicking the back navigation
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

		/**********************************************************************
        *********************************************************************/
		void CreateContent()
		{
			// This is the top-level grid, which will split our page in half
			var grid = new Grid();
			this.Content = grid;
			grid.RowDefinitions = new RowDefinitionCollection {
                    // Each half will be the same size:
                    new RowDefinition{ Height = new GridLength(4, GridUnitType.Star)},
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
            skiaview = CongestionAvoidanceDraw.ReturnCanvas();
            grid.Children.Add(skiaview, 0, 0);
        }

		/**********************************************************************
        *********************************************************************/
		void CreateBottomHalf(Grid grid)
		{
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (3)
			//Screen Width -20 because Margin is 10
			double StackChildSize = (Application.Current.MainPage.Width - 20) / 3;

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            Button b_Triple = new Button
            {
                Text = "Triple Duplicate Ack",
				WidthRequest = StackChildSize,
				VerticalOptions = LayoutOptions.Center
            };
            b_Triple.Clicked += B_Triple_Clicked;
            stackLayout.Children.Add(b_Triple);

            Button b_Set_ErrorValue = new Button
            {
                Text = "Set Error-Value",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Set_ErrorValue.Clicked += B_Set_ErrorValue_Clicked;
            stackLayout.Children.Add(b_Set_ErrorValue);

            Button b_Next = new Button
            {
                Text = "Next Step",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            stackLayout.Children.Add(b_Next);

			grid.Children.Add(stackLayout, 0, 1);
		}

		/**********************************************************************
        *********************************************************************/
		void B_Triple_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		void B_Set_ErrorValue_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		void B_Next_Clicked(object sender, EventArgs e)
        {

        }

		/**********************************************************************
        *********************************************************************/
		/// <summary> deletes all content and informs the user to rotate the device </summary>
		void DeleteContent()
		{
			this.Content = null;
			this.Content = new Label { Text = "please rotate your device" };
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
                //isContentCreated = true;
                this.Content.IsVisible = true;
            }
            else if (height > width)
            {
                this.Content.IsVisible = false;
            }
        }

	}
}