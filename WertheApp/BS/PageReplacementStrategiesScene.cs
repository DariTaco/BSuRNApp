using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class PageReplacementStrategiesScene : CCScene
    {
		//VARIABLES
		CCLayer layer;

        //CONSTRUCTOR
        public PageReplacementStrategiesScene(CCGameView gameView): base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);
		}

        //METHODS
    }
}

