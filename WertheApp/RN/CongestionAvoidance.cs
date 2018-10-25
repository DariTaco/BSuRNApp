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
        public static int treshold;
        public static bool reno;
        public static bool tahoe;

        bool landscape = false; //indicates device orientation
        double StackChildSize;

        private double width = 0;
		private double height = 0;

        private SKCanvasView skiaview;
        private CongestionAvoidanceDraw draw;

        Button b_DupAck;
        public static int dupAckCount;
        public static int state; //0 -> slow start, 1 -> congestion avoidance, 2 -> fast recovery
        private Color orange1 = new Color(242, 115, 0);

        //CONSTRUCTOR
        public CongestionAvoidance(int th, bool r, bool t)
        {
            treshold = th;
            reno = r;
            tahoe = t;

            dupAckCount = 0;
            state = 0;

			/*Debug.WriteLine("##########");
            Debug.WriteLine("error Treshold: " + errorTreshold);
			Debug.WriteLine("treshold: " + treshold);
			Debug.WriteLine("reno: " + reno);
			Debug.WriteLine("tahoe: " + tahoe);*/

			Title = "Congestion Control";

            draw = new CongestionAvoidanceDraw();

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
            if (!landscape)
            {
                StackChildSize = (Application.Current.MainPage.Height - 20) / 3;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width - 20) / 3;
            }

            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            //For example l_Size, size
            var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            b_DupAck = new Button
            {
                Text = "Dup ACK (" + dupAckCount +")",
                TextColor = Color.Green,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = StackChildSize
            };
            b_DupAck.Clicked += B_DupAck_Clicked;
            stackLayout.Children.Add(b_DupAck);

            Button b_Timeout = new Button
            {
                Text = "Timeout",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Timeout.Clicked += B_Timeout_Clicked;
            stackLayout.Children.Add(b_Timeout);

            Button b_NewAck = new Button
            {
                Text = "New Ack",
				WidthRequest = StackChildSize,
				VerticalOptions = LayoutOptions.Center
            };
            b_NewAck.Clicked += B_NewAck_Clicked;;
            stackLayout.Children.Add(b_NewAck);

            grid.Children.Add(stackLayout, 0, 1);
		}


		/**********************************************************************
        *********************************************************************/
		void B_NewAck_Clicked(object sender, EventArgs e)
        {
            dupAckCount = 0;
            b_DupAck.Text = "Dup ACK (" + dupAckCount + ")";
            b_DupAck.TextColor = Color.Green;
        }

		/**********************************************************************
        *********************************************************************/
		void B_DupAck_Clicked(object sender, EventArgs e)
        {
            dupAckCount++;
            b_DupAck.Text = "Dup ACK (" + dupAckCount + ")";
            switch(dupAckCount){
                case 0: b_DupAck.TextColor = Color.Green;
                    break;
                case 1: b_DupAck.TextColor = Color.DarkOrange;
                    break;
                case 2: b_DupAck.TextColor = Color.Crimson;
                    break;
                case 3: b_DupAck.TextColor = Color.Purple;
                    break;
                default: b_DupAck.TextColor = Color.Purple;
                    break;
            }
        }

		/**********************************************************************
        *********************************************************************/
		void B_Timeout_Clicked(object sender, EventArgs e)
        {
            state = (state + 1) % 3;
            CongestionAvoidanceDraw.state = state;
            CongestionAvoidanceDraw.Paint();
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