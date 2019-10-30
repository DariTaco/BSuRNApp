using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq; //fragmentList.ElementAt(i);
using System.Diagnostics;
using Xamarin.Forms; //Messaging Center

namespace WertheApp.RN
{
    public class DijkstraSettingsScene: CCScene
    {
        //VARIABLES
        static CCLayer layer;

        public DijkstraSettingsScene(CCGameView gameView): base(gameView){
            //add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);
        }
    }
}
