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
        public static bool renoOn;
        public static bool tahoeOn;

        bool landscape = false; //indicates device orientation
        double StackChildSize;

        private double width = 0;
		private double height = 0;

        private SKCanvasView skiaview;
        private CongestionAvoidanceDraw draw;

        Button b_DupAck, b_Timeout, b_NewAck;
        private Color orange1 = new Color(242, 115, 0);

        public static int stateT, stateR; //0 -> slow start, 1 -> congestion avoidance, 2 -> fast recovery
        public static int dupAckCount;
        public static int rateR, rateT;
        public static int currentStep;
        public static int numberOfSteps;
        public static int maxRate;
        public static int tresholdR,tresholdT;
        public static int[] reno; //contains y values for reno
        public static int[] tahoe; //contains y values for tahoe
        public static int[] treshR; //contains y values for treshold Reno
        public static int[] treshT; //contains y values for treshold Tahoe

        //CONSTRUCTOR
        public CongestionAvoidance(int th, bool r, bool t)
        {
            tresholdR = th;
            tresholdT = th;
            renoOn = r;
            tahoeOn = t;
            stateT = 0;
            stateR = 0;
            dupAckCount = 0;
            rateR = 1;
            rateT = 1;
            currentStep = 0;
            numberOfSteps = 32;
            maxRate = 14;

            reno = new int[numberOfSteps];
            tahoe = new int[numberOfSteps];
            reno[0] = rateR;
            tahoe[0] = rateT;

            treshR = new int[numberOfSteps];
            treshT = new int[numberOfSteps];
            treshR[0] = tresholdR;
            treshT[0] = tresholdT;

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
        void B_NewAck_Clicked(object sender, EventArgs e)
        {
            currentStep++;

            dupAckCount = 0;

            UpdateButtons();

            //RENO:
            switch (stateR){
                case 0:
                    int rateROld = rateR;
                    rateR = rateR + rateR; //exponential growth
                    if (rateR >= tresholdR) 
                    { 
                        stateR = 1; //switch to congestion avoidance
                        if (rateROld < tresholdR) { rateR = tresholdR; } 
                    } 
                    break;
                case 1: 
                    rateR++; //linear growth
                    break;
                case 2: 
                    rateR = tresholdR;
                    stateR = 1; //switch to congestion avoidance
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    int rateTOld = rateT;
                    rateT = rateT + rateT; //exponential growth
                    if (rateT >= tresholdT) 
                    { 
                        stateT = 1; //switch to congestion avoidance
                        if(rateTOld < tresholdT){ rateT = tresholdT;}
                    } 
                    break;
                case 1:
                    rateT++;
                    break;
            }

            //save in arrays 
            treshR[currentStep] = tresholdR;
            treshT[currentStep] = tresholdT;
            reno[currentStep] = rateR;
            tahoe[currentStep] = rateT;

            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_DupAck_Clicked(object sender, EventArgs e)
        {
            currentStep++;
            dupAckCount++;

            UpdateButtons();

            //RENO:
            switch (stateR)
            {
                case 0:
                    if (dupAckCount == 3) 
                    { 
                        tresholdR = rateR / 2; 
                        rateR = tresholdR; 
                        stateR = 2; //switch to fast recovery
                    }
                    break;
                case 1:
                    if (dupAckCount == 3)
                    {
                        tresholdR = rateR / 2;
                        rateR = tresholdR;
                        stateR = 2; //switch to fast recovery
                    }
                    break;
                case 2:
                    rateR++;
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    if(dupAckCount == 3)
                    {
                        tresholdT = rateT / 2;
                        rateT = 1; //switch to Congestion Avoidance
                    }
                    break;
                case 1:
                    if (dupAckCount == 3)
                    {
                        tresholdT = rateT / 2;
                        rateT = 1;
                        stateT = 0; //Switch to Slow Start
                    }
                    break;
            }

            //save in arrays
            treshR[currentStep] = tresholdR;
            reno[currentStep] = rateR;
            treshT[currentStep] = tresholdT;
            tahoe[currentStep] = rateT;

            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Timeout_Clicked(object sender, EventArgs e)
        {
            currentStep++;
            dupAckCount = 0;

            UpdateButtons();

            //RENO:
            switch (stateR)
            {
                case 0:
                    tresholdR = rateR / 2;
                    rateR = 1;
                    break;
                case 1:
                    tresholdR = rateR / 2;
                    rateR = 1;
                    stateR = 0;
                    break;
                case 2:
                    tresholdR = rateR / 2;
                    rateR = 1;
                    stateR = 0;
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    tresholdT = rateT / 2;
                    rateT = 1;
                    break;
                case 1:
                    tresholdT = rateT / 2;
                    rateT = 1;
                    stateT = 0;
                    break;
            }

            //save in arrays
            treshR[currentStep] = tresholdR;
            reno[currentStep] = rateR;
            treshT[currentStep] = tresholdT;
            tahoe[currentStep] = rateT;

            UpdateDrawing();
        }





        /**********************************************************************
        *********************************************************************/
        void UpdateDrawing(){
            //update background
            CongestionAvoidanceDraw.stateR = stateR;
            CongestionAvoidanceDraw.stateT = stateT;
            CongestionAvoidanceDraw.Paint();

            Debug.WriteLine("reno:");
            PrintArray(reno);
            Debug.WriteLine("tresh reno:");
            PrintArray(treshR);
        }


        /***************************************************************
        *********************************************************************/
       
        void UpdateButtons(){
            //update Button Color and Text
            b_DupAck.Text = "Dup ACK (" + dupAckCount + ")";
            switch (dupAckCount)
            {
                case 0:
                    b_DupAck.TextColor = Color.Green;
                    break;
                case 1:
                    b_DupAck.TextColor = Color.DarkOrange;
                    break;
                case 2:
                    b_DupAck.TextColor = Color.Crimson;
                    break;
                case 3:
                    b_DupAck.TextColor = Color.Purple;
                    break;
                default:
                    b_DupAck.TextColor = Color.Purple;
                    break;
            }

            //enable /disable when rate to high
            if (renoOn && !tahoeOn && rateR >= maxRate 
                || !renoOn && tahoeOn && rateT >= maxRate
                || renoOn && tahoeOn && (rateR >= maxRate || rateT >= maxRate) )
            {
                b_NewAck.IsEnabled = false;
                b_DupAck.IsEnabled = false;
            }else{
                b_NewAck.IsEnabled = true;
                b_DupAck.IsEnabled = true;
            }

            //enable / disbale
            if (currentStep == numberOfSteps-1)
            {
                b_DupAck.IsEnabled = false;
                b_NewAck.IsEnabled = false;
                b_Timeout.IsEnabled = false;
            }
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

            b_Timeout = new Button
            {
                Text = "Timeout",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Timeout.Clicked += B_Timeout_Clicked;
            stackLayout.Children.Add(b_Timeout);

            b_NewAck = new Button
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

        public void PrintArray(int [] a){
            String s = "";
            for (int i = 0; i < a.Length; i++){
                s += a[i];
                s += ",";
            }
            Debug.WriteLine(s);
        }

	}
}