﻿using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;

using System.Diagnostics;

namespace WertheApp.RN
{
    public class PipelineProtocols : ContentPage
    {
        //VARIABLES
        public static int windowSize;
        public static String strategy;

        bool isContentCreated = false; //indicates weather the Content of the page was already created

		private double width = 0;
		private double height = 0;

		//CONSTRUCTOR
		public PipelineProtocols(int a, String s)
        {
            windowSize = a;
            strategy = s;

            /*Debug.WriteLine("########");
            Debug.WriteLine("window Size: " + windowSize);
            Debug.WriteLine("strategy: "+strategy);*/

			Title = "Pipeline Protocols";

            //do only create content if device is not roated
			if (Application.Current.MainPage.Width < Application.Current.MainPage.Height)
			{
				CreateContent();
			}
			else
			{
				this.Content = new Label { Text = "please rotate your device" };
			}
        }

		//METHODS
		void CreateContent()
		{
			// This is the top-level grid, which will split our page in half
			var grid = new Grid();
			this.Content = grid;
			grid.RowDefinitions = new RowDefinitionCollection {
                    // Each half will be the same size:
                    new RowDefinition{ Height = new GridLength(8, GridUnitType.Star)},
					new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
				};
			CreateTopHalf(grid);
			CreateBottomHalf(grid);

            isContentCreated = true;
		}

		void CreateTopHalf(Grid grid)
		{
			var gameView = new CocosSharpView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				// This gets called after CocosSharp starts up:
				ViewCreated = HandleViewCreated
			};
			grid.Children.Add(gameView, 0, 0);
		}

		void CreateBottomHalf(Grid grid)
		{
			//set the size of the elements in such a way, that they all fit on the screen
			//Screen Width is divided by the amount of elements (9)
			//Screen Width -20 because Margin is 10
			double StackChildSize = (Application.Current.MainPage.Width - 20) / 2;

			//Using a Stacklayout to organize elements
			//with corresponding labels and String variables. 
			//For example l_Size, size
			var stackLayout = new StackLayout
			{
                Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            Button b_Send = new Button
            {
                Text = "Send Package",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Send.Clicked += B_Send_Clicked;
            stackLayout.Children.Add(b_Send);

            Button b_Stop = new Button
            {
                Text = "Stop",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Stop.Clicked += B_Stop_Clicked;
            stackLayout.Children.Add(b_Stop);

			grid.Children.Add(stackLayout, 0, 1);
		}

        void B_Send_Clicked(object sender, EventArgs e)
        {

        }

        void B_Stop_Clicked(object sender, EventArgs e)
        {

        }

		/// <summary> deletes all content and informs the user to rotate the device </summary>
		void DeleteContent()
		{
			this.Content = null;
			this.Content = new Label { Text = "please rotate your device" };
			isContentCreated = false;
		}

		//sets up the scene 
		void HandleViewCreated(object sender, EventArgs e)
		{
			PipelineProtocolsScene gameScene;

			var gameView = sender as CCGameView;
			if (gameView != null)
			{
				// This sets the game "world" resolution to 330x100:
				//Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
				gameView.DesignResolution = new CCSizeI(330, 100);
				// GameScene is the root of the CocosSharp rendering hierarchy:
				gameScene = new PipelineProtocolsScene(gameView);
				// Starts CocosSharp:
				gameView.RunWithScene(gameScene);
			}
		}

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
			if (width > height && isContentCreated)
			{
				DeleteContent();
			}
			else if (height > width && isContentCreated == false)
			{
				CreateContent();
			}
		}


	}
}

