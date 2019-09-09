﻿using System;
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
        private static bool flag1; //indicates that state was changed from 1 to 0 because of 3 dupACK

        bool landscape = false; //indicates device orientation
        double StackChildSize;

        private double width = 0;
		private double height = 0;

        private SKCanvasView skiaview;
        private CongestionAvoidanceDraw draw;

        Button b_DupAck, b_Timeout, b_NewAck;
        private Color orange1 = new Color(242, 115, 0);

        public static int stateT, stateR; //0 -> slow start, 1 -> congestion avoidance, 2 -> fast recovery
        public static int dupAckCountR, dupAckCountT;
        public static int cwndR, cwndT;
        public static int currentRoundR, currentRoundT, currentIndex;
        public const int numberOfRounds = 32;
        public static int maxCwnd;
        public static int tresholdR,tresholdT;
        public static int[,] reno; //contains y values for reno [value, round]
        public static int[,] tahoe; //contains y values for tahoe
        public static int[,] sstreshR; //contains y values for treshold Reno
        public static int[,] sstreshT; //contains y values for treshold Tahoe
        public static String strategy;

        //CONSTRUCTOR
        public CongestionAvoidance(int th, bool r, bool t)
        {
            tresholdR = th;
            tresholdT = th;
            renoOn = r;
            tahoeOn = t;
            flag1 = false;
            stateT = 0;
            stateR = 0;
            dupAckCountR = 0;
            dupAckCountT = 0;
            cwndR = 1;
            cwndT = 1;
            currentRoundR = 0;
            currentRoundT = 0;
            currentIndex = 0;
            maxCwnd = 14;

            //note: numberOfRounds*4 is kinda arbitrary. Since there can be no arrays of unknown length, but it has to be long enough
            reno = new int[2,numberOfRounds*4]; 
            tahoe = new int[2,numberOfRounds*4];
            reno[1, 0] = 0;
            reno[0,0] = cwndR;
            tahoe[1, 0] = 0;
            tahoe[0,0] = cwndT;

            sstreshR = new int[2,numberOfRounds*4];
            sstreshT = new int[2,numberOfRounds*4];
            sstreshR[1, 0] = 0;
            sstreshR[0,0] = tresholdR;
            sstreshT[1, 0] = 0;
            sstreshT[0,0] = tresholdT;

            if (tahoeOn && !renoOn)
            {
                strategy = "Tahoe";
            }
            else if (renoOn && !tahoeOn)
            {
                strategy = "Reno";
            }
            else { strategy = "Reno & Tahoe"; }
            Title = "Congestion Control: " + strategy;

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
            dupAckCountR = 0;
            dupAckCountT = 0;
            currentIndex++;

            //RENO:
            switch (stateR){
                case 0:
                    currentRoundR++;
                    int cwndROld = cwndR;
                    cwndR = cwndR + cwndR; //exponential growth
                    if (cwndR >= tresholdR) 
                    { 
                        stateR = 1; //switch to congestion avoidance
                        if (cwndROld < tresholdR) { cwndR = tresholdR; } 
                    } 
                    break;
                case 1:
                    currentRoundR++;
                    cwndR++; //linear growth
                    break;
                case 2:
                    currentRoundR++; 
                    cwndR = tresholdR;
                    stateR = 1; //switch to congestion avoidance
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    currentRoundT++;
                    int cwndTOld = cwndT;
                    cwndT = cwndT + cwndT; //exponential growth
                    if (cwndT >= tresholdT) 
                    { 
                        stateT = 1; //switch to congestion avoidance
                        if(cwndTOld < tresholdT){ cwndT = tresholdT;}
                    } 
                    break;
                case 1:
                    currentRoundT++;
                    cwndT++;
                    break;
            }

            UpdateButtons();

            //save in arrays 
            if (renoOn)
            {
                sstreshR[0, currentIndex] = tresholdR;
                sstreshR[1, currentIndex] = currentRoundR;
                reno[0, currentIndex] = cwndR;
                reno[1, currentIndex] = currentRoundR;
            }
            if (tahoeOn)
            {
                sstreshT[0, currentIndex] = tresholdT;
                sstreshT[1, currentIndex] = currentRoundT;
                tahoe[0, currentIndex] = cwndT;
                tahoe[1, currentIndex] = currentRoundT;
            }

            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_DupAck_Clicked(object sender, EventArgs e)
        {
            dupAckCountR++;
            dupAckCountT++;
            currentIndex++;


            //RENO:
            switch (stateR)
            {
                case 0:
                    if (dupAckCountR == 3) 
                    {
                        //*geändert*/currentRoundR++;
                        tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                        cwndR = tresholdR + 3; 
                        stateR = 2; //switch to fast recovery
                    }
                    break;
                case 1:
                    if (dupAckCountR == 3)
                    {
                        //*geändert*/currentRoundR++;
                        tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                        cwndR = tresholdR + 3;
                        stateR = 2; //switch to fast recovery
                    }
                    break;
                case 2:
                    cwndR++;
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    if(dupAckCountT == 3 && !flag1)
                    {
                        //*geändert*/currentRoundT++;
                        tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                        cwndT = 1;
                    }
                    dupAckCountT++;
                    flag1 = false;
                    break;
                case 1:
                    if (dupAckCountT == 3)
                    {
                        //*geändert*/currentRoundT++;
                        tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                        cwndT = 1;
                        stateT = 0; //Switch to Slow Start
                        flag1 = true;
                    }
                    break;
            }

            UpdateButtons();

            //save in arrays
            if (renoOn)
            {
                sstreshR[0, currentIndex] = tresholdR;
                sstreshR[1, currentIndex] = currentRoundR;
                reno[0, currentIndex] = cwndR;
                reno[1, currentIndex] = currentRoundR;
            }
            if (tahoeOn)
            {
                sstreshT[0, currentIndex] = tresholdT;
                sstreshT[1, currentIndex] = currentRoundT;
                tahoe[0, currentIndex] = cwndT;
                tahoe[1, currentIndex] = currentRoundT;
            }


            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Timeout_Clicked(object sender, EventArgs e)
        {
            currentRoundR++;
            currentRoundT++;
            currentIndex++;
            dupAckCountR = 0;
            dupAckCountT = 0;

            //RENO:
            switch (stateR)
            {
                case 0:
                    tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                    cwndR = 1;
                    break;
                case 1:
                    tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                    cwndR = 1;
                    stateR = 0;
                    break;
                case 2:
                    tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                    cwndR = 1;
                    stateR = 0;
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                    cwndT = 1;
                    break;
                case 1:
                    tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                    cwndT = 1;
                    stateT = 0;
                    break;
            }

            UpdateButtons();

            //save in arrays
            if(renoOn){
                sstreshR[0, currentIndex] = tresholdR;
                sstreshR[1, currentIndex] = currentRoundR;
                reno[0, currentIndex] = cwndR;
                reno[1, currentIndex] = currentRoundR;
            }
            if(tahoeOn){
                sstreshT[0, currentIndex] = tresholdT;
                sstreshT[1, currentIndex] = currentRoundT;
                tahoe[0, currentIndex] = cwndT;
                tahoe[1, currentIndex] = currentRoundT;
            }

            UpdateDrawing();
        }





        /**********************************************************************
        *********************************************************************/
        void UpdateDrawing(){
            //update background
            CongestionAvoidanceDraw.stateR = stateR;
            CongestionAvoidanceDraw.stateT = stateT;
            CongestionAvoidanceDraw.Paint();
            PrintArray(reno);
            PrintArray(sstreshR);
            //PrintArray(tahoe);
        }


        /***************************************************************
        *********************************************************************/
       
        void UpdateButtons(){
            //update Button Color and Text
            b_DupAck.Text = "Dup ACK (" + dupAckCountR + ")";
            switch (dupAckCountR)
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


            //update Text
            switch (stateR){
                case 0:
                    b_NewAck.Text = "New Acks";
                    break;
                case 1:
                    b_NewAck.Text = "New Acks";
                    break;
                case 2:
                    if (!tahoeOn && renoOn) { b_NewAck.Text = "New Ack"; }
                    else if (!renoOn && tahoeOn) { b_NewAck.Text = "New Acks"; }
                    else if (renoOn && tahoeOn) { b_NewAck.Text = "New Ack(s)"; }
                    break;
            }


            //enable /disable when cwnd to high
            if (renoOn && !tahoeOn && cwndR >= maxCwnd 
                || !renoOn && tahoeOn && cwndT >= maxCwnd
                || renoOn && tahoeOn && (cwndR >= maxCwnd || cwndT >= maxCwnd) )
            {
                b_NewAck.IsEnabled = false;
                b_DupAck.IsEnabled = false;
            }else{
                b_NewAck.IsEnabled = true;
                b_DupAck.IsEnabled = true;

                /*//enable / disable when dupack count too high
                if(renoOn && !tahoeOn){
                    if(dupAckCountR >= cwndR - 1){
                        b_DupAck.IsEnabled = false;
                    }else{
                        b_DupAck.IsEnabled = true;
                    }
                }else if(tahoeOn && !renoOn){
                    if(dupAckCountT >= cwndT - 1){
                        b_DupAck.IsEnabled = false;
                    }else{
                        b_DupAck.IsEnabled = true;
                    }
                }else if(tahoeOn && renoOn){
                    if(dupAckCountT >= cwndT - 1 || dupAckCountR >= cwndR - 1){
                        b_DupAck.IsEnabled = false;
                    }else{
                        b_DupAck.IsEnabled = true;
                    }
                }*/

            }

            //enable / disbale when end is reached
            if (currentRoundR == numberOfRounds-1 && renoOn || currentRoundT == numberOfRounds-1 && tahoeOn)
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
                Text = "Dup ACK (" + dupAckCountR +")",
                TextColor = Color.Green,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = StackChildSize
            };
            b_DupAck.IsEnabled = false;
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
                Text = "New Acks",
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

        public static void PrintArray(int[,] array)
        {
            Debug.WriteLine("");
            // Loop over 2D int array and display it.
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                String s = "";
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    s += array[i, j];
                    s += " ";
                }
                Debug.WriteLine(s);
            }
        }
    }
}