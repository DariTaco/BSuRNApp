using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic; 
using System.Linq; //fragmentList.ElementAt(i);
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.RN
{
    public class DijkstraSettingsChild1: ContentPage
    {
        //VARIABLES
        public const int gameviewWidth = 330;
        public const int gameviewHeight = 100;

        //CONSTRUCTOR
        public DijkstraSettingsChild1(String title)
        {
            Title = title;
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {

        }

        /**********************************************************************
        *********************************************************************/
        //sets up the scene 
        void HandleViewCreated(object sender, EventArgs e)
        {
            DijkstraSettingsScene gameScene;

            var gameView = sender as CCGameView;
            if (gameView != null)
            {
                // This sets the game "world" resolution to 330x100:
                //Attention: all drawn elements in the scene strongly depend ont he resolution! Better don't change it
                gameView.DesignResolution = new CCSizeI(gameviewWidth, gameviewHeight);

                // GameScene is the root of the CocosSharp rendering hierarchy:
                gameScene = new DijkstraSettingsScene(gameView);

                // Starts CocosSharp:
                gameView.RunWithScene(gameScene);

            }
        }
    }
}
